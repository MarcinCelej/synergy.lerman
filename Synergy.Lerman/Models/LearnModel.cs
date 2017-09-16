using JetBrains.Annotations;
using Synergy.Lerman.Controllers;

namespace Synergy.Lerman.Models
{
    public class LearnInput
    {
        public string Lesson { get; set; }
        public string Book { get; set; }
        public string Category { get; set; }
        public string Word { get; set; }

        public string PreviousWord { get; set; }
        public int? Result { get; set; }
        public int? Count { get; [UsedImplicitly] set; }

        public static LearnInput New(Category category)
        {
            return new LearnInput
            {
                Lesson = UniqueId.New("L"),
                Book = category.BookName,
                Category = category.Name
            };
        }

        public static LearnInput YetAnotherLesson(Lesson lesson)
        {
            return new LearnInput
            {
                Lesson = UniqueId.New("L"),
                Book = lesson.Book.Name,
                Category = lesson.Category.Name
            };
        }

        public static LearnInput Random()
        {
            return new LearnInput
            {
                Lesson = UniqueId.New("L")
            };
        }

        public static LearnInput Wrong(LearnModel model)
        {
            return new LearnInput
            {
                Lesson = model.LessonId,
                Book = model.Word.Category.BookName,
                Category = model.Word.Category.Name,
                Word = model.Lesson.NextWord().Polish,
                PreviousWord = model.Word.Polish,
                Result = (int)Action.Wrong,
            };
        }

        public static LearnInput Right(LearnModel model)
        {
            return new LearnInput
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

        public enum Action
        {
            Right = 1,
            Wrong = 0
        }
    }

    public class LearnModel
    {
        public Lesson Lesson;

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