using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using JetBrains.Annotations;
using Synergy.Contracts;
using Synergy.Lerman.Models;
using Synergy.Lerman.Realm.Books;

namespace Synergy.Lerman.Controllers
{
    public class EditController : Controller
    {
        [HttpGet]
        public ActionResult Book(string name)
        {
            Fail.IfArgumentWhiteSpace(name, nameof(name));

            var book = BookStore.GetBook(name);
            return RedirectToBookEditor(book);
        }

        private RedirectToRouteResult RedirectToBookEditor(Book book)
        {
            return RedirectToAction(nameof(BookId), new {id = book.Id});
        }

        [HttpGet]
        public ActionResult BookId(Guid id)
        {
            Fail.IfArgumentEmpty(id, nameof(id));

            var book = BookStore.GetBookById(id);
            var model = new EditBookModel(book);

            return View("Book", model);
        }

        [HttpPost]
        public ActionResult BookSave([NotNull] EditBookModel model)
        {
            Fail.IfArgumentNull(model, nameof(model));
            Fail.IfArgumentEmpty(model.Id, nameof(model.Id));
            Fail.IfArgumentWhiteSpace(model.Name, nameof(model.Name));

            // TODO: nazwa książki musi być unikalna - wyjątek
            // TODO: Dodaj obsługę wyjątków
            var book = BookStore.GetBookById(model.Id);
            var duplicate = BookStore.FindBook(model.Name);
            if (duplicate != null && duplicate != book)
                throw new ApplicationException($"There already is book named '{model.Name}'");
            book.Rename(model.Name);
            book.Save();

            return RedirectToBookEditor(book);
        }

        [HttpGet]
        public ActionResult CategoryNew(Guid bookId)
        {
            Fail.IfArgumentEmpty(bookId, nameof(bookId));

            var category = BookStore.GetBookById(bookId);
            var model = new NewCategoryModel(category);

            return View(model);
        }

        [HttpPost]
        public ActionResult CategoryCreate([NotNull] NewCategoryModel model)
        {
            Fail.IfArgumentNull(model, nameof(model));
            Fail.IfArgumentEmpty(model.BookId, nameof(model.BookId));
            Fail.IfArgumentWhiteSpace(model.Name, nameof(model.Name));

            var book = BookStore.GetBookById(model.BookId);
            var category = book.CreateCategory(model.Name);
            book.Save();

            return RedirectToCategoryEditor(category);
        }

        private RedirectToRouteResult RedirectToCategoryEditor(Category category)
        {
            return RedirectToAction(nameof(Category), new { id = category.Id });
        }

        [HttpGet]
        public ActionResult Category(Guid id)
        {
            Fail.IfArgumentEmpty(id, nameof(id));

            var category = BookStore.GetCategory(id);
            var model = new EditCategoryModel(category);

            return View(model);
        }

        [HttpPost]
        public ActionResult CategoryUpdate([NotNull] EditCategoryModel model)
        {
            Fail.IfArgumentNull(model, nameof(model));
            Fail.IfArgumentEmpty(model.Id, nameof(model.Id));
            Fail.IfArgumentWhiteSpace(model.Name, nameof(model.Name));

            var category = BookStore.GetCategory(model.Id);
            category.Rename(model.Name);
            category.Book.Save();

            return RedirectToCategoryEditor(category);
        }

        [HttpGet]
        public ActionResult WordNew(Guid categoryId)
        {
            Fail.IfArgumentEmpty(categoryId, nameof(categoryId));

            var category = BookStore.GetCategory(categoryId);
            var model = new NewWordModel(category);

            return View(model);
        }

        [HttpPost]
        public ActionResult WordCreate([NotNull] NewWordModel model)
        {
            Fail.IfArgumentNull(model, nameof(model));
            Fail.IfArgumentEmpty(model.CategoryId, nameof(model.CategoryId));

            var category = BookStore.GetCategory(model.CategoryId);
            var book = category.Book;

            var word = new Word(category, model.Polish, model.GetEnglishPhrases().ToList());
            category.Words.Add(word);
            book.Save();

            //var modelNew = new NewWordModel(category);
            //return View("WordNew", modelNew);
            return RedirectToAction(nameof(WordNew), new { categoryId = category.Id });
        }

        [HttpGet]
        public ActionResult Word(Guid categoryId, string name)
        {
            Fail.IfArgumentEmpty(categoryId, nameof(categoryId));
            Fail.IfArgumentWhiteSpace(name, nameof(name));

            var category = BookStore.GetCategory(categoryId);
            var word = category.GetWord(name);
            var model = new EditModel(word);

            return View(model);
        }
    }

    public class EditBookModel
    {
        public Guid Id { get; set; }

        [CanBeNull]
        [Required]
        public string Name { get; set; }

        public List<Category> Categories { get; }

        [UsedImplicitly]
        public EditBookModel()
        {
        }

        public EditBookModel(Book book)
        {
            Id = book.Id;
            Name = book.Name;
            Categories = book.Categories;
        }
    }

    public class NewCategoryModel
    {
        [UsedImplicitly]
        public Guid BookId { get; set; }
        
        [NotNull]
        public string BookName { get; }

        [CanBeNull]
        [Required]
        public string Name { get; set; }

        [UsedImplicitly]
        public NewCategoryModel()
        {
        }

        public NewCategoryModel(Book book)
        {
            this.BookId = book.Id;
            this.BookName = book.Name;
        }
    }

    public class EditCategoryModel
    {
        [NotNull]
        public string BookName { get; }

        public Guid Id { get; set; }

        [CanBeNull]
        [Required]
        public string Name { get; set; }

        public List<Word> Words { get; }

        [UsedImplicitly]
        public EditCategoryModel()
        {
        }

        public EditCategoryModel(Category category)
        {
            this.Id = category.Id;
            this.Name = category.Name;
            this.Words = category.Words;
            this.BookName = category.BookName;
        }
    }

    public class NewWordModel
    {
        [UsedImplicitly]
        public Guid CategoryId { get; set; }

        public string Polish { get; set; }
        public string English1 { get; set; }
        public string English2 { get; set; }
        public string English3 { get; set; }
        public string English4 { get; set; }
        public string English5 { get; set; }
        public string English6 { get; set; }

        [UsedImplicitly]
        public NewWordModel()
        {
        }

        public NewWordModel(Category category)
        {
            this.CategoryId = category.Id;
        }

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
    }
}