using System.Collections.Generic;
using System.IO;
using System.Linq;
using Synergy.Lerman.Realm.Books.Reading;
using Synergy.Lerman.Realm.Infrastructure;

namespace Synergy.Lerman.Realm.Books
{
    public static class BookStore
    {
        private static readonly Dictionary<string, Book> bookStore = new Dictionary<string, Book>();

        public static void Read(string path)
        {
            var files = Directory.GetFiles(path, "*.txt");

            foreach(var filePath in files)
            {
                var book = BookReader.Read(filePath);
                bookStore.Add(book.Name, book);
            }
        }

        public static List<Book> GetBooks()
        {
            return bookStore.Values.ToList();
        }

        public static Book RandomBook()
        {
            var books = GetBooks();
            int bookIndex = CryptoRandom.NextIndex(books.Count);
            return books[bookIndex];
        }

        public static Book GetBook(string book)
        {
            return bookStore[book];
        }

        public static List<Category> GetCategories()
        {
            return bookStore.Values.SelectMany(book => book.Categories).ToList();
        }
    }
}