using Synergy.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Synergy.Lerman.Models
{
    public class Book
    {
        public String Name { get; }
        public String Code => this.Name.Replace(" ", "");
        public string FilePath { get; set; }

        public Book(string name)
        {
            this.Name = name;
            this.Categories = new List<Category>();
        }

        public List<Category> Categories { get; }

        public string GetDisplayCategoriesCount()
        {
            return this.Categories.Count.ToString();
        }

        internal void WasReadFromFile(string filePath)
        {
            this.FilePath = filePath;
        }

        public Category GetCategory(string category)
        {
            return this.Categories.First(c => c.Name == category);
        }

        public Category RandomCategory()
        {
            int bookIndex = CryptoRandom.NextIndex(Categories.Count);
            return Categories[bookIndex];
        }

        internal void Save()
        {
            StringBuilder content = new StringBuilder();
            content.AppendLine($"[{this.Name}]");

            foreach (var category in this.Categories)
            {
                content.AppendLine();
                content.AppendLine($"[{category.Name}]");

                foreach (var word in category.Words)
                {
                    var en = String.Join("|", word.GetEnglishPhrases());
                    var pl = word.GetPolishPhrase();
                    content.AppendLine($"{en} = {pl}");
                }
            }

            File.WriteAllText(this.FilePath, content.ToString(), Encoding.UTF8);
        }
    }

    public class Category
    {
        public Book Book { get; }
        public string BookName => this.Book.Name;
        public string Name { get; }

        public List<Word> Words { get; }
        public int WordsCount => this.Words.Count;

        public string GetDisplayWordsCount()
        {
            if (this.WordsCount > 100)
                return "100+";

            return this.WordsCount.ToString();
        }

        public Category(Book book, string name)
        {
            this.Book = book;
            this.Name = name;
            this.Words = new List<Word>();
        }

        public Word GetWord(string polish)
        {
            return this.Words
                .FirstOrDefault(w => w.Polish == polish)
                .FailIfNull("There is no '{0}' in [{1}/{2}]", polish, this.BookName, this.Name);
        }

        internal Word GetPreviousWord(Word word)
        {
            var index = this.Words.IndexOf(word) - 1;
            if (index < 0)
                index = this.Words.Count - 1;

            return this.Words[index];
        }

        internal Word GetNextWord(Word word)
        {
            var index = this.Words.IndexOf(word) + 1;
            if (index >= this.Words.Count)
                index = 0;

            return this.Words[index];
        }
    }

    public class Word
    {
        public Word(string en, string pl, Category category)
        {
            this.Polish = pl.Trim();
            this.English = en.Trim();
            this.Phrases = this.CalculateEnglishPhrases();
            this.Category = category;
        }

        public Category Category { get; }

        public string Polish { get; private set; }
        private string English { get; set; }
        private List<string> Phrases { get; set; }

        private List<string> CalculateEnglishPhrases()
        {
            List<string> phrases = new List<string>();
            var versions = this.English.Split('|');
            foreach (var version in versions)
            {
                Regex regex = new Regex(@"\w+\s*\/\s*\w+");
                MatchCollection matches = regex.Matches(version);
                foreach (var match in matches)
                {
                    var optionalPart = ((Match) match).Value;
                    var options = optionalPart.Split('/');
                    foreach (var option in options)
                    {
                        var phrase = version.Replace(optionalPart, option.Trim());
                        phrases.Add(phrase);
                    }
                }

                if (matches.Count == 0)
                    phrases.Add(version);
            }

            return phrases;
        }

        public void Edit(string polish, List<string> phrases)
        {
            this.Polish = polish;
            this.Phrases = phrases;
            this.English = String.Join("|", phrases);
        }

        public string GetPolishPhrase()
        {
            return this.Polish.Replace("/", " / ").Replace("  ", " ");
        }

        public List<string> GetEnglishPhrases()
        {
            return this.Phrases;
        }

        public string GetEnglishPhrase(int index)
        {
            var en = this.GetEnglishPhrases();
            if (index >= en.Count)
                return null;

            return en[index];
        }
    }


}