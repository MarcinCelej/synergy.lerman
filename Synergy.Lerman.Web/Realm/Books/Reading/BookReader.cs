using System.IO;

namespace Synergy.Lerman.Realm.Books.Reading
{
    public static class BookReader
    {
        public static Book Read(string filePath)
        {
            var fileContent = File.ReadAllText(filePath);
            IBookReader reader;
            if (fileContent.StartsWith("{"))
            {
                reader = new JsonFileBookReader(filePath);
            }
            else
            {
                reader = new TextFileBookReader(filePath);
            }

            Book book = reader.Read(fileContent);
            book.WasReadBy(new JsonFileBookReader(filePath));

            return book;
        }
    }

    public interface IBookReader
    {
        Book Read(string fileContent);
        void Save(Book book);
    }
}