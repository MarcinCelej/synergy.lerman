using Synergy.Lerman.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Synergy.Lerman.Controllers
{
    public class Lesson
    {
        public string Id { get; set; }
        public Book Book { get; set; }
        public Category Category { get; set; }

        private List<Word> toLearn;
        private List<Word> learned;
        private List<Mistake> mistakes;
        private Word lastWord;

        public DateTime StartedOn { get; }
        public DateTime? FinishedOn { get; private set; }
        public int WordCount => this.toLearn.Count + this.learned.Count;
        public int ElapsedMinutes
        {
            get
            {
                var end = this.FinishedOn ?? DateTime.Now;
                var minutes = end.Subtract(this.StartedOn).TotalMinutes;
                if (minutes < 1)
                    minutes = 1;
                return (int)minutes;
            }
        }

        public Lesson(string id, Book book, Category category, int? count)
        {
            this.Id = id;
            this.Book = book;
            this.Category = category;
            int noOfWords = count ?? 30;
            this.toLearn = new List<Word>(noOfWords);
            this.learned = new List<Word>(noOfWords);
            this.mistakes = new List<Mistake>();

            this.PrepareWords(category, noOfWords);
            this.StartedOn = DateTime.Now;
        }

        private void PrepareWords(Category category, int count)
        {
            var allWords = category.Words.ToList();
            var noOfWordsInLesson = Math.Min(count, allWords.Count);
            for (int i = 0; i < noOfWordsInLesson; i++)
            {
                int randomIndex = CryptoRandom.NextIndex(allWords.Count);
                var word = allWords[randomIndex];
                allWords.Remove(word);
                this.toLearn.Add(word);
            }
        }

        public Word NextWord()
        {
            if (this.toLearn.Count == 0)
                return null;

            var unknownWords = this.toLearn.ToList();
            if (this.toLearn.Count > 1)
            {
                unknownWords.Remove(this.lastWord);
            }

            int randomIndex = CryptoRandom.NextIndex(unknownWords.Count);
            this.lastWord = unknownWords[randomIndex];
            return this.lastWord;
        }

        public string GetProgress()
        {
            return $"{this.learned.Count} / {this.WordCount}";
        }

        public int GetSpeed()
        {
            return (int)(this.learned.Count / this.ElapsedMinutes);
        }

        public void NoticeAnswer(LearnInput input)
        {
            if (input.PreviousWord == null)
                return;

            var word = this.toLearn.FirstOrDefault(w => w.Polish == input.PreviousWord);
            if (word == null)
                return;

            if (input.UserMarkedWordAsLearned())
            {
                this.toLearn.Remove(word);
                this.learned.Add(word);
            }
            else 
            {
                var mistake = this.mistakes.FirstOrDefault(m => m.Word.Polish == input.PreviousWord);
                if (mistake == null)
                {
                    mistake = new Mistake(word);
                    this.mistakes.Add(mistake);
                }

                mistake.Count++;
            }
        }

        public bool LearnedAll()
        {
            return this.toLearn.Count == 0;
        }

        internal void Finished()
        {
            this.FinishedOn = DateTime.Now;
        }

        private class Mistake
        {
            public Word Word;
            public int Count;

            public Mistake(Word word)
            {
                this.Word = word;
            }
        }
    }
}