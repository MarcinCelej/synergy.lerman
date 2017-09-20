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
        public void TryToFindPronunciations()
        {
            var path ="../../Sources/";
            BookStore.Read(path);

            //foreach (var word in book.Categories.SelectMany(c => c.Words))
            //{
            //    var translations = word.GetEnglishTexts();

            //    foreach (var t in translations)
            //    {
            //        Console.WriteLine("{0} => {1}", word.GetPolishPhrase(), t);
            //    }
            //}

            Parallel.ForEach(BookStore.GetBooks(), book =>
            {
                book.TryToFindPronunciations();
                book.Save();
            });
        }
    }
}
