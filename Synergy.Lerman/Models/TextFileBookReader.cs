using Synergy.Contracts;
using Synergy.Lerman.Models;
using System;
using System.Linq;

namespace Synergy.Lerman.Controllers
{
    public class TextFileBookReader
    {
        public static Book Read(string fileContent)
        {
            var lines = fileContent.Split('\n');
            Book book = null;
            Category category = null;
            bool inverted = false;
            foreach (var line in lines.Select(l => l.Trim()))
            {
                if (String.IsNullOrWhiteSpace(line))
                    continue;

                if (book == null)
                {
                    var bookName = ReadName(line);
                    book = new Book(bookName);
                    continue;
                }

                if (line.StartsWith("["))
                {
                    inverted = line.EndsWith("-");
                    var name = ReadName(line);
                    category = new Category(book, name);
                    book.Categories.Add(category);
                    continue;
                }

                var translations = line.Split('=');
                Fail.IfFalse(translations.Length == 2, "Something wrong with line '{0}'", line);

                string en = translations[0];
                string pl = translations[1];

                if (inverted)
                {
                    pl = translations[0];
                    en = translations[1];
                }

                var word = new Word(en, pl, category);
                category.Words.Add(word);
            }

            return book;
        }

        private static string ReadName(string line)
        {
            return line.TrimEnd('-').Trim('[', ']');
        }
    }
}