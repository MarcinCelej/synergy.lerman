using System.Linq;
using Synergy.Contracts;
using Synergy.Lerman.Models;
using System.Web.Mvc;
using System.Web.Security;
using Synergy.Lerman.Realm.Books;
using Synergy.Lerman.Realm.Lessons;

namespace Synergy.Lerman.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var books = BookStore.GetBooks();

            return View(books);
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
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

        public ActionResult Learn(LearnInputModel inputModel)
        {
            Fail.IfNull(inputModel, nameof(inputModel));

            var lesson = this.GetLesson(inputModel);
            if (string.IsNullOrWhiteSpace(inputModel.Word))
            {
                inputModel.Word = lesson.NextWord().Polish;
                return this.RedirectToAction(nameof(Learn), inputModel);
            }

            lesson.NoticeAnswer(inputModel);
            var model = new LearnModel(lesson, inputModel.Word);

            if (lesson.LearnedAll())
            {
                lesson.Finished();

                return this.View("Success", model);
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult Edit(EditInputModel input)
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

            return RedirectToAction("Edit", new EditInputModel(word));
        }

        private Lesson GetLesson(LearnInputModel inputModel)
        {
            // TODO: dodaj aktualne słowo do input - żeby BACK przechodził na to samo słowo i żeby można było poprawić błąd
            var lesson = this.Session[inputModel.Lesson] as Lesson;
            if (lesson != null)
                return lesson;

            var book = this.GetBookForLearning(inputModel);
            var category = this.GetCategoryForLearning(inputModel, book);

            lesson = new Lesson(inputModel.Lesson, book, category, inputModel.Count);
            this.Session[inputModel.Lesson] = lesson;

            return lesson;
        }

        private Book GetBookForLearning(LearnInputModel inputModel)
        {
            Book book;
            if (inputModel.Book == null)
            {
                book = BookStore.RandomBook();
                inputModel.Book = book.Name;
                inputModel.Category = null;
            }
            else
            {
                book = BookStore.GetBook(inputModel.Book);
            }
            return book;
        }

        private Category GetCategoryForLearning(LearnInputModel inputModel, Book book)
        {
            Category category;

            if (inputModel.Category == null)
            {
                category = book.RandomCategory();
                inputModel.Category = category.Name;
                inputModel.PreviousWord = null;
            }
            else
            {
                category = book.GetCategory(inputModel.Category);
            }
            return category;
        }
    }

}