using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Synergy.Lerman.Realm.Books.Reading;
using Synergy.Lerman.Realm.Infrastructure;

namespace Synergy.Lerman.Realm.Books
{
    public static class BookStore
    {
        private static readonly List<Book> bookStore = new List<Book>();

        public static void Read(string path)
        {
            var files = Directory.GetFiles(path, "*.txt");

            foreach(var filePath in files)
            {
                var book = BookReader.Read(filePath);
                bookStore.Add(book);
            }
        }

        [NotNull, ItemNotNull, Pure]
        public static List<Book> GetBooks()
        {
            return bookStore;
        }

        [NotNull, Pure]
        public static Book RandomBook()
        {
            var books = GetBooks();
            int bookIndex = CryptoRandom.NextIndex(books.Count);
            return books[bookIndex];
        }

        [CanBeNull, Pure]
        public static Book FindBook(string bookName)
        {
            return bookStore.FirstOrDefault(book => book.Name == bookName);
        }

        [NotNull, Pure]
        public static Book GetBook(string name)
        {
            return bookStore.First(book => book.Name == name);
        }

        [NotNull, Pure]
        public static Book GetBookById(Guid id)
        {
            return bookStore.First(book => book.Id == id);
        }

        [NotNull, Pure]
        public static Category GetCategory(Guid id)
        {
            return bookStore.SelectMany(b => b.Categories).First(c => c.Id == id);
        }

        public static void BookWasChanged(Book book)
        {
            if (bookStore.Contains(book))
                return;

            bookStore.Add(book);
        }
    }
}