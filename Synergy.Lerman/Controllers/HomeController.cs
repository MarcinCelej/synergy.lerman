using Synergy.Contracts;
using Synergy.Lerman.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Synergy.Lerman.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var books = BookStore.GetBooks();

            return View(books);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Learn(LearnInput input)
        {
            Fail.IfNull(input, nameof(input));

            this.PrepareLearnInput(input);
            this.NoticePreviousAnswer(input);

            var book = BookStore.GetBook(input.Book);
            var category = book.GetCategory(input.Category);
            int nextWordIndex = CryptoRandom.NextIndex(category.WordsCount);
            var word = category.Words[nextWordIndex];

            var model = new LearnModel()
            {
                LessonId = input.Lesson,
                Word = word
            };

            return View(model);
        }

        private void PrepareLearnInput(LearnInput input)
        {
            if (input.Book == null)
            {
                var randomBook = BookStore.RandomBook();

                input.Book = randomBook.Name;
                input.Category = null;
                input.PreviousWord = null;
            }

            if (input.Category == null)
            {
                var book = BookStore.GetBook(input.Book);

                input.Category = book.RandomCategory().Name;
                input.PreviousWord = null;
            }
        }


        private void NoticePreviousAnswer(LearnInput input)
        {
            if (input.UserAnsweredProperly() && input.PreviousWord != null)
            {
                var answered = this.Session[input.Lesson] as List<string> ?? new List<string>(50);
                answered.Add(input.PreviousWord);
                this.Session[input.Lesson] = answered;
            }
        }
    }
}