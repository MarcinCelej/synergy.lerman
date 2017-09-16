using JetBrains.Annotations;
using Synergy.Lerman.Controllers;
using System.Collections.Generic;
using Synergy.Lerman.Realm.Books;

namespace Synergy.Lerman.Models
{
    public class EditInputModel
    {
        public string Book { get; [UsedImplicitly] set; }
        public string Category { get; set; }
        public string Word { get; set; }

        public EditInputModel()
        {
        }

        public EditInputModel(Word word)
        {
            this.Book = word.Category.BookName;
            this.Category = word.Category.Name;
            this.Word = word.Polish;
        }
    }

    public class EditModel
    {
        public EditModel()
        {
        }

        public EditModel(Word word)
        {
            Book = word.Category.BookName;
            Category = word.Category.Name;
            Word = word.Polish;

            Polish = word.GetPolishPhrase();
            English1 = word.GetEnglishPhrase(0);
            English2 = word.GetEnglishPhrase(1);
            English3 = word.GetEnglishPhrase(2);
            English4 = word.GetEnglishPhrase(3);
            English5 = word.GetEnglishPhrase(4);
            English6 = word.GetEnglishPhrase(5);
        }

        public string Book { get; set; }
        public string Category { get; set; }
        public string Word { get; set; }

        public string Polish { get; set; }
        public string English1 { get; set; }
        public string English2 { get; set; }
        public string English3 { get; set; }
        public string English4 { get; set; }
        public string English5 { get; set; }
        public string English6 { get; set; }

        public IEnumerable<string> GetEnglishPhrases()
        {
            if (string.IsNullOrWhiteSpace(this.English1) == false)
                yield return this.English1;
            if (string.IsNullOrWhiteSpace(this.English2) == false)
                yield return this.English2;
            if (string.IsNullOrWhiteSpace(this.English3) == false)
                yield return this.English3;
            if (string.IsNullOrWhiteSpace(this.English4) == false)
                yield return this.English4;
            if (string.IsNullOrWhiteSpace(this.English5) == false)
                yield return this.English5;
            if (string.IsNullOrWhiteSpace(this.English6) == false)
                yield return this.English6;
        }

        public static EditModel GoToPreviousWord(EditModel model)
        {
            var b = BookStore.GetBook(model.Book);
            var c = b.GetCategory(model.Category);
            var w = c.GetWord(model.Word);
            var nextWord = c.GetPreviousWord(w);

            return new EditModel(nextWord);
        }

        public static EditModel GoToNextWord(EditModel model)
        {
            var b = BookStore.GetBook(model.Book);
            var c = b.GetCategory(model.Category);
            var w = c.GetWord(model.Word);
            var nextWord = c.GetNextWord(w);

            return new EditModel(nextWord);
        }
    }
}