using Microsoft.VisualStudio.TestTools.UnitTesting;
using Synergy.Lerman.Tests.Properties;
using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Synergy.Lerman.Realm.Books;
using Synergy.Lerman.Realm.Books.Reading;

namespace Synergy.Lerman.Tests.Sources
{
    [TestClass]
    public class TReader
    {
        // REPLACE PATTERN: ^(Unit\s+.*?)\r\n  =>  [$1]\n
        [TestMethod]
        public void read()
        {
            var path = "../../Sources/";
            BookStore.Read(path);
        }


        [TestMethod]
        public void TryToFindPronunciations()
        {
            var path ="../../Sources/";
            BookStore.Read(path);
            var book = BookStore.GetBook("PrimeTime Preintermediate");

            Parallel.ForEach(book.Categories, category =>
            {
                category.TryToFindPronunciations();
            });

            book.Save();
        }
    }
}
