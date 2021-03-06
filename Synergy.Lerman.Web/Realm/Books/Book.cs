﻿using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Synergy.Contracts;
using Synergy.Lerman.Realm.Books.Reading;
using Synergy.Lerman.Realm.Infrastructure;

namespace Synergy.Lerman.Realm.Books
{
    public class Book
    {
        // TODO: Zamiast GUID'a lepiej dodać swoją klasę BookId
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; private set; }

        [JsonProperty(PropertyName = "name")]
        public String Name { get; private set; }

        [JsonProperty(PropertyName = "categories")]
        public List<Category> Categories { get; private set; }

        private IBookReader reader;

        public Book()
        {
            this.Id = Guid.NewGuid();
        }

        public Book(string name):this()
        {
            this.Name = name;
            this.Categories = new List<Category>();
        }

        public string GetDisplayCategoriesCount()
        {
            return this.Categories.Count.ToString();
        }

        internal void WasReadBy([NotNull] IBookReader reader)
        {
            this.reader = reader;
        }

        public List<Category> GetCategoriesToLearn()
        {
            return this.Categories.Where(c => c.Words.Count > 0).ToList();
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

            BookStore.BookWasChanged(this);
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

        public Category CreateCategory(string categoryName)
        {
            var category = new Category(this, categoryName);
            this.Categories.Add(category);
            return category;
        }
    }
}