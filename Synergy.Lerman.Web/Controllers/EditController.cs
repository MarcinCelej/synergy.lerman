using System.Web.Mvc;
using JetBrains.Annotations;
using Synergy.Contracts;
using Synergy.Lerman.Realm.Books;

namespace Synergy.Lerman.Controllers
{
    public class EditController : Controller
    {
        [HttpGet]
        public ActionResult Book(string id)
        {
            Fail.IfArgumentWhiteSpace(id, nameof(id));

            var book = BookStore.GetBook(id);
            var model = new EditBookModel(book);

            return View(model);
        }

        [HttpPost]
        public ActionResult BookSave([NotNull] EditBookModel model)
        {
            Fail.IfArgumentNull(model, nameof(model));
            Fail.IfArgumentWhiteSpace(model.Id, nameof(model.Id));
            Fail.IfArgumentWhiteSpace(model.Name, nameof(model.Name));

            var book = BookStore.GetBook(model.Id);
            book.Rename(book.Name);
            book.Save();

            return RedirectToAction(nameof(this.Book), book.Name);
        }
    }

    public class EditBookModel
    {
        [UsedImplicitly]
        public EditBookModel()
        {
        }

        public EditBookModel(Book book)
        {
            this.Id = book.Id;
            this.Name = book.Name;
        }

        [CanBeNull]
        public string Id { get; set; }

        [CanBeNull]
        public string Name { get; set; }
    }
}