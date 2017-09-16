using System;
using System.Web;
using System.Web.SessionState;
using Synergy.Lerman.Controllers;
using Synergy.Contracts;
using System.Collections.Generic;

namespace Synergy.Lerman.Models
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

        internal static void Learned(Learning learning)
        {
            if (learning.Learned == false)
                return;

            var learned = GetLearnedWords();
            learned.Add(learning.Word);
        }
    }
}