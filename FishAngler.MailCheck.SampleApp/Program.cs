using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishAngler.MailCheck.SampleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            DefaultRun();
            CustomRun();
            Console.ReadLine();
        }

        static void DefaultRun()
        {
            var mailCheck = new MailCheck();
            var suggestion = mailCheck.Suggest("test@hotmal.con");
            if (suggestion != null)
            {
                Debug.WriteLine($"Suggested email: {suggestion.Full}");
            }
            
        }

        static void CustomRun()
        {
            string[] domains = { "custom-domain.com", "hotmail.com", "gmail.com", "yahoo.com" };
            string[] secondLevelDomains = { "custom-domain", "hotmail", "gmail", "yahoo" };
            string[] topLevelDomains = { "com" };

            var mailCheck = new MailCheck(domains, secondLevelDomains, topLevelDomains);
            var suggestion = mailCheck.Suggest("test@hotmal.con");
            if (suggestion != null)
            {
                Debug.WriteLine($"Suggested email: {suggestion.Full}");
            }

            suggestion = mailCheck.Suggest("test@custom-doman.con");
            if (suggestion != null)
            {
                Debug.WriteLine($"Suggested email: {suggestion.Full}");
            }
        }
    }
}
