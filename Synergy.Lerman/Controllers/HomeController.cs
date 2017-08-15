using Synergy.Contracts;
using Synergy.Lerman.Models;
using System.Web.Mvc;
using System.Web.Security;

namespace Synergy.Lerman.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var books = BookStore.GetBooks();

            return View(books);
        }

        public ActionResult Login(string user)
        {
            FormsAuthentication.SetAuthCookie(user, true);

            return RedirectToAction(nameof(Index));
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page. ";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Books()
        {
            var books = BookStore.GetBooks();

            return View(books);
        }

        public ActionResult Book(string name)
        {
            var book = BookStore.GetBook(name);

            return View(book);
        }

        public ActionResult Learn(LearnInput input)
        {
            Fail.IfNull(input, nameof(input));

            var lesson = this.GetLesson(input);
            lesson.NoticeAnswer(input);
            var model = new LearnModel(lesson);

            if (lesson.LearnedAll())
            {
                lesson.Finished();

                return this.View("Success", model);
            }

            return View(model);
        }

        private Lesson GetLesson(LearnInput input)
        {
            var lesson = this.Session[input.Lesson] as Lesson;
            if (lesson != null)
                return lesson;

            Book book;
            Category category;
            if (input.Book == null)
            {
                book = BookStore.RandomBook();
                input.Category = null;
            }
            else
            {
                book = BookStore.GetBook(input.Book);
            }

            if (input.Category == null)
            {
                category = book.RandomCategory();
                input.PreviousWord = null;
            }
            else
            {
                category = book.GetCategory(input.Category);
            }

            lesson = new Lesson(input.Lesson, book, category, input.Count);
            this.Session[input.Lesson] = lesson;

            return lesson;
        }
    }

}