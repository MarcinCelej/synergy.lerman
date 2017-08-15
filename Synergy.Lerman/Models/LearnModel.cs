using Synergy.Lerman.Controllers;

namespace Synergy.Lerman.Models
{
    public class LearnInput
    {
        public string Lesson { get; set; }
        public string Book { get; set; }
        public string Category { get; set; }
        public string PreviousWord { get; set; }
        public int? Result { get; set; }
        public int? Count { get; set; }

        public static LearnInput New(Category category)
        {
            return new LearnInput
            {
                Lesson = UniqueId.New("L"),
                Book = category.BookName,
                Category = category.Name
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
        private Lesson lesson;

        public string LessonId => this.lesson.Id;
        public Book Book => this.lesson.Book;
        public Category Category => this.lesson.Category;
        public Word Word { get; }

        public string ProgresDisplay => this.lesson.GetProgress();
        public string SpeedDisplay => $"{this.lesson.GetSpeed()} / min";
        public int WordsCount => this.lesson.WordCount;
        public int ElapsedMinutes => this.lesson.ElapsedMinutes;
        public int PercentageSuccess => 6;

        public LearnModel(Lesson lesson)
        {
            this.lesson = lesson;
            this.Word = lesson.NextWord();
        }
    }
}