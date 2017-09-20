using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Synergy.Lerman.Realm.Books.Reading
{
    public class JsonFileBookReader: IBookReader
    {
        private readonly string filePath;

        public JsonFileBookReader(string filePath)
        {
            this.filePath = filePath;
        }

        public Book Read(string fileContent)
        {
            var book = JsonConvert.DeserializeObject<Book>(fileContent);
            foreach (var category in book.Categories)
            {
                category.BelongsTo(book);

                foreach (var word in category.Words)
                {
                    word.BelongsTo(category);
                }
            }

            return book;
        }

        public void Save(Book book)
        {
            var json = JsonConvert.SerializeObject(book, Formatting.Indented);
            File.WriteAllText(this.filePath, json, Encoding.UTF8);
        }
    }
}