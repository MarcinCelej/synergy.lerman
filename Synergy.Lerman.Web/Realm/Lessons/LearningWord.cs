using Synergy.Lerman.Realm.Books;

namespace Synergy.Lerman.Realm.Lessons
{
    public class LearningWord
    {
        public readonly Word Word;
        public int Mistakes;
        public bool Learned;

        public LearningWord(Word word)
        {
            this.Word = word;
        }
    }
}