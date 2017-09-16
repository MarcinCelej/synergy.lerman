using Synergy.Contracts;
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
        private List<Learning> learning;
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
            this.learning = new List<Learning>();

            this.PrepareWords(category, noOfWords);
            this.StartedOn = DateTime.Now;
        }

        private void PrepareWords(Category category, int count)
        {
            List<Word> wordsAlreadyLearned = CurrentUser.GetLearnedWords();
            var allWords = category.Words.Except(wordsAlreadyLearned).ToList();
            if (allWords.Count == 0)
            {
                // TODO: Losuj spośród pomyłek 
                allWords = category.Words.ToList();
            }

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

            Learning learning = NoticeLearningOf(word);

            if (input.UserMarkedWordAsLearned())
            {
                this.toLearn.Remove(word);
                this.learned.Add(word);
                learning.Learned = true;
            }
            else
            {
                learning.Mistakes++;
            }

            CurrentUser.Learned(learning);
        }

        internal Word GetWord(string polish)
        {
            return this.Category.GetWord(polish);
                //.FirstOrDefault(w => w.Word.Polish == polish)
                //?.Word
                //.FailIfNull("There is no '{0} in lesson {1}", polish, this.Id)
                //;
        }

        private Learning NoticeLearningOf(Word word)
        {
            var learning = this.learning.FirstOrDefault(m => m.Word.Polish == word.Polish);
            if (learning == null)
            {
                learning = new Learning(word);
                this.learning.Add(learning);
            }

            return learning;
        }

        public bool LearnedAll()
        {
            return this.toLearn.Count == 0;
        }

        internal void Finished()
        {
            this.FinishedOn = DateTime.Now;
        }
    }

    public class Learning
    {
        public Word Word;
        public int Mistakes;
        public bool Learned;

        public Learning(Word word)
        {
            this.Word = word;
        }
    }
}