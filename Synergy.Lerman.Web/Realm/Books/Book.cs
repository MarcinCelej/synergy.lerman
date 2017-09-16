using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Synergy.Lerman.Realm.Infrastructure;

namespace Synergy.Lerman.Realm.Books
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
}