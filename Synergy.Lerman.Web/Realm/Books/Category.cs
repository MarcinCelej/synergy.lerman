using System.Collections.Generic;
using System.Linq;
using Synergy.Contracts;

namespace Synergy.Lerman.Realm.Books
{
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
}