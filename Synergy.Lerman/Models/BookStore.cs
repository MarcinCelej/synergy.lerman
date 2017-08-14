using Synergy.Lerman.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;

namespace Synergy.Lerman.Controllers
{
    public class BookStore
    {
        public static Dictionary<string, Book> books = new Dictionary<string, Book>();

        public static void Read(string path)
        {
            var files = Directory.GetFiles(path, "*.txt");

            foreach(var filePath in files)
            {
                var fileContent = File.ReadAllText(filePath);
                var book = TextFileBookReader.Read(fileContent);
                books.Add(book.Name, book);
            }
        }

        public static List<Book> GetBooks()
        {
            return books.Values.ToList();
        }

        public static Book RandomBook()
        {
            var books = GetBooks();
            int bookIndex = CryptoRandom.NextIndex(books.Count);
            return books[bookIndex];
        }

        public static Book GetBook(string book)
        {
            return books[book];
        }

        public static List<Category> GetCategories()
        {
            return books.Values.SelectMany(book => book.Categories).ToList();
        }
    }
}