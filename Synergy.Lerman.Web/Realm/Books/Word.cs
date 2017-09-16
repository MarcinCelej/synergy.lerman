using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Synergy.Lerman.Realm.Books
{
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