using JetBrains.Annotations;
using Synergy.Lerman.Controllers;
using Synergy.Lerman.Realm.Books;
using Synergy.Lerman.Realm.Infrastructure;
using Synergy.Lerman.Realm.Lessons;

namespace Synergy.Lerman.Models
{
    public class LearnInputModel
    {
        public string Lesson { get; set; }
        public string Book { get; set; }
        public string Category { get; set; }
        public string Word { get; set; }

        public string PreviousWord { get; set; }
        public int? Result { get; set; }
        public int? Count { get; [UsedImplicitly] set; }

        public static LearnInputModel New(Category category)
        {
            return new LearnInputModel
            {
                Lesson = UniqueId.New("L"),
                Book = category.BookName,
                Category = category.Name
            };
        }

        public static LearnInputModel YetAnotherLesson(Lesson lesson)
        {
            return new LearnInputModel
            {
                Lesson = UniqueId.New("L"),
                Book = lesson.Book.Name,
                Category = lesson.Category.Name
            };
        }

        public static LearnInputModel Random()
        {
            return new LearnInputModel
            {
                Lesson = UniqueId.New("L")
            };
        }

        public static LearnInputModel Wrong(LearnModel model)
        {
            return new LearnInputModel
            {
                Lesson = model.LessonId,
                Book = model.Word.Category.BookName,
                Category = model.Word.Category.Name,
                Word = model.Lesson.NextWord().Polish,
                PreviousWord = model.Word.Polish,
                Result = (int)Action.Wrong,
            };
        }

        public static LearnInputModel Right(LearnModel model)
        {
            return new LearnInputModel
            {
                Lesson = model.LessonId,
                Book = model.Word.Category.BookName,
                Category = model.Word.Category.Name,
                Word = model.Lesson.NextWord().Polish,
                PreviousWord = model.Word.Polish,
                Result = (int)Action.Right,
            };
        }

        public bool UserMarkedWordAsLearned()
        {
            return this.Result == (int)Action.Right;
        }

        private enum Action
        {
            Right = 1,
            Wrong = 0
        }
    }

    public class LearnModel
    {
        public readonly Lesson Lesson;

        public string LessonId => this.Lesson.Id;
        public Book Book => this.Lesson.Book;
        public Category Category => this.Lesson.Category;
        public Word Word { get; }

        public string ProgresDisplay => this.Lesson.GetProgress();
        public string SpeedDisplay => $"{this.Lesson.GetSpeed()} / min";
        public int WordsCount => this.Lesson.WordCount;
        public int ElapsedMinutes => this.Lesson.ElapsedMinutes;
        public int PercentageSuccess => 6;

        public LearnModel(Lesson lesson, string word)
        {
            this.Lesson = lesson;
            this.Word = lesson.GetWord(word);
        }
    }
}