using System;
using System.IO;
using System.Linq;
using System.Text;
using Synergy.Contracts;

namespace Synergy.Lerman.Realm.Books.Reading
{
    public class TextFileBookReader: IBookReader
    {
        private readonly string filePath;

        public TextFileBookReader(string filePath)
        {
            this.filePath = filePath;
        }

        public Book Read(string fileContent)
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
                    book = CreateBook(line);
                    continue;
                }

                if (line.StartsWith("["))
                {
                    inverted = line.EndsWith("-");
                    category = CreateCategory(book, line);
                    continue;
                }

                CreateWord(category, inverted, line);
            }

            book.Categories.TrimExcess();
            book.Categories.ForEach(c => c.Words.TrimExcess());

            return book;
        }

        private static Book CreateBook(string line)
        {
            Book book;
            var bookName = ReadName(line);
            book = new Book(bookName);
            return book;
        }

        private static Category CreateCategory(Book book, string line)
        {
            Category category;
            var name = ReadName(line);
            category = new Category(book, name);
            book.Categories.Add(category);
            return category;
        }

        private static void CreateWord(Category category, bool inverted, string line)
        {
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

        private static string ReadName(string line)
        {
            return line.TrimEnd('-').Trim('[', ']');
        }

        public void Save(Book book)
        {
            StringBuilder content = new StringBuilder();
            content.AppendLine($"[{book.Name}]");

            foreach (var category in book.Categories)
            {
                content.AppendLine();
                content.AppendLine($"[{category.Name}]");

                foreach (var word in category.Words)
                {
                    var en = String.Join("|", word.GetEnglishTexts());
                    var pl = word.GetPolishPhrase();
                    content.AppendLine($"{en} = {pl}");
                }
            }

            File.WriteAllText(this.filePath, content.ToString(), Encoding.UTF8);
        }
    }
}