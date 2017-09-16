using System.Collections.Generic;
using System.Web;
using System.Web.SessionState;
using Synergy.Contracts;
using Synergy.Lerman.Realm.Books;
using Synergy.Lerman.Realm.Lessons;

namespace Synergy.Lerman.Realm.Users
{
    public class CurrentUser
    {
        public static bool IsAuthenticated()
        {
            return HttpContext.Current.User.Identity.IsAuthenticated;
        }

        private static HttpSessionState GetSession()
        {
            return HttpContext.Current.Session;
        }

        public static List<Word> GetLearnedWords()
        {
            var learned = GetSession()["Learned"].AsOrFail<List<Word>>();
            if (learned == null)
            {
                learned = new List<Word>();
                GetSession().Add("Learned", learned);
            }

            return learned;
        }

        internal static void Learned(LearningWord learning)
        {
            if (learning.Learned == false)
                return;

            var learned = GetLearnedWords();
            learned.Add(learning.Word);
        }
    }
}