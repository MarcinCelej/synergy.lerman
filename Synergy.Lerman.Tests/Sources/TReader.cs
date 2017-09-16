using Microsoft.VisualStudio.TestTools.UnitTesting;
using Synergy.Lerman.Controllers;
using Synergy.Lerman.Tests.Properties;
using System;
using System.Linq;
using Synergy.Lerman.Realm.Books.Reading;

namespace Synergy.Lerman.Tests.Sources
{
    [TestClass]
    public class TReader
    {
        // REPLACE PATTERN: ^(Unit\s+.*?)\r\n  =>  [$1]\n

        [TestMethod]
        public void Read()
        {
            var file = Resources.longman;
            var book = TextFileBookReader.Read(file);

            foreach (var word in book.Categories.SelectMany(c => c.Words))
            {
                var translations = word.GetEnglishPhrases();

                foreach (var t in translations)
                {
                    Console.WriteLine("{0} => {1}", word.GetPolishPhrase(), t);
                }
            }
        }
    }
}
