using System;

namespace Synergy.Lerman.Models
{
    public class LearnInput
    {
        public string Lesson { get; set; }
        public string Book { get; set; }
        public string Category { get; set; }
        public string PreviousWord { get; set; }
        public int? Result { get; set; }

        public static LearnInput New(Category category)
        {
            return new LearnInput
            {
                Lesson = UniqueId.New("L"),
                Book = category.BookName,
                Category = category.Name
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

        public bool UserAnsweredProperly()
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
        public string LessonId { get; set; }
        public Word Word { get; set; }
    }
}