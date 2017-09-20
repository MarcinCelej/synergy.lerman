using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Synergy.Contracts;

namespace Synergy.Lerman.Realm.Books
{
    public class Category
    {
        [JsonIgnore]
        public Book Book { get; private set; }

        [JsonIgnore]
        public string BookName => this.Book.Name;

        [JsonProperty(PropertyName = "name")]
        public string Name { get; private set; }

        [JsonProperty(PropertyName = "words")]
        public List<Word> Words { get; private set; }

        private int WordsCount => this.Words.Count;

        public Category()
        {
        }

        public Category(Book book, string name)
        {
            this.Book = book;
            this.Name = name;
            this.Words = new List<Word>();
        }

        public string GetDisplayWordsCount()
        {
            if (this.WordsCount > 100)
                return "100+";

            return this.WordsCount.ToString();
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

        public void BelongsTo([NotNull] Book book)
        {
            this.Book = book;
        }

        public void TryToFindPronunciations()
        {
            this.Words.ForEach(phrase => phrase.TryToFindPronunciations());
        }
    }
}