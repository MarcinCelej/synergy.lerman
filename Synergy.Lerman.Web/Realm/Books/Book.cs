using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Synergy.Contracts;
using Synergy.Lerman.Realm.Books.Reading;
using Synergy.Lerman.Realm.Infrastructure;

namespace Synergy.Lerman.Realm.Books
{
    public class Book
    {
        [JsonProperty(PropertyName = "id")]
        public String Id { get; private set; }

        [JsonProperty(PropertyName = "name")]
        public String Name { get; private set; }

        [JsonProperty(PropertyName = "categories")]
        public List<Category> Categories { get; private set; }

        private IBookReader reader;

        public Book()
        {
        }

        public Book(string name)
        {
            this.Id = name;
            this.Name = name;
            this.Categories = new List<Category>();
        }

        public string GetDisplayCategoriesCount()
        {
            return this.Categories.Count.ToString();
        }

        internal void WasReadBy(IBookReader reader)
        {
            this.reader = reader;
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

        public void Save()
        {
            this.reader.Save(this);
        }

        public void Rename([NotNull] string name)
        {
            Fail.IfArgumentWhiteSpace(name, nameof(name));

            this.Name = name;
        }

        public void TryToFindPronunciations()
        {
            this.Categories.ForEach(phrase => phrase.TryToFindPronunciations());
        }
    }
}