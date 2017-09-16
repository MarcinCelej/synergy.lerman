using System.Linq;
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
            if (string.IsNullOrWhiteSpace(input.Word))
            {
                input.Word = lesson.NextWord().Polish;
                return this.RedirectToAction(nameof(Learn), input);
            }

            lesson.NoticeAnswer(input);
            var model = new LearnModel(lesson, input.Word);

            if (lesson.LearnedAll())
            {
                lesson.Finished();

                return this.View("Success", model);
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult Edit(EditInput input)
        {
            var book = BookStore.GetBook(input.Book);
            var category = book.GetCategory(input.Category);
            var word = category.GetWord(input.Word);

            var model = new EditModel(word);

            return View(model);
        }

        [HttpPost]
        public ActionResult Save(EditModel model)
        {
            var book = BookStore.GetBook(model.Book);
            var category = book.GetCategory(model.Category);
            var word = category.GetWord(model.Word);

            word.Edit(model.Polish, model.GetEnglishPhrases().ToList());
            book.Save();

            return RedirectToAction("Edit", new EditInput(word));
        }

        private Lesson GetLesson(LearnInput input)
        {
            // TODO: dodaj aktualne słowo do input - żeby BACK przechodził na to samo słowo i żeby można było poprawić błąd
            var lesson = this.Session[input.Lesson] as Lesson;
            if (lesson != null)
                return lesson;

            var book = this.GetBookForLearning(input);
            var category = this.GetCategoryForLearning(input, book);

            lesson = new Lesson(input.Lesson, book, category, input.Count);
            this.Session[input.Lesson] = lesson;

            return lesson;
        }

        private Book GetBookForLearning(LearnInput input)
        {
            Book book;
            if (input.Book == null)
            {
                book = BookStore.RandomBook();
                input.Book = book.Name;
                input.Category = null;
            }
            else
            {
                book = BookStore.GetBook(input.Book);
            }
            return book;
        }

        private Category GetCategoryForLearning(LearnInput input, Book book)
        {
            Category category;

            if (input.Category == null)
            {
                category = book.RandomCategory();
                input.Category = category.Name;
                input.PreviousWord = null;
            }
            else
            {
                category = book.GetCategory(input.Category);
            }
            return category;
        }
    }

}