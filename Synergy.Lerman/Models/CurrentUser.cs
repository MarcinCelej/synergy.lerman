using System.Web;

namespace Synergy.Lerman.Models
{
    public class CurrentUser
    {
        public static bool IsAuthenticated()
        {
            return HttpContext.Current.User.Identity.IsAuthenticated;
        }
    }
}