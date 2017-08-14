using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Synergy.Lerman.Models
{
    public class Book
    {
        public String Name { get; }

        public Book(string name)
        {
            this.Name = name;
            this.Categories = new List<Category>();
        }

        public List<Category> Categories { get; }

        public Category GetCategory(string category)
        {
            return this.Categories.First(c => c.Name == category);
        }

        public Category RandomCategory()
        {
            int bookIndex = CryptoRandom.NextIndex(Categories.Count);
            return Categories[bookIndex];
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

        public string Polish { get; }
        public string English { get; }
        public List<string> Phrases { get; }
        public Category Category { get;}

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
                    var optionalPart = (match as Match).Value;
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

        public string GetPolishPhrase()
        {
            return this.Polish;
        }

        public List<string> GetEnglishPhrases()
        {
            return this.Phrases;
        }
    }
}