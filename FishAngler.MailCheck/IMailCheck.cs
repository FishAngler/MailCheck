/*
 * Mailcheck https://github.com/FishAngler/MailCheck
 * Author
 * Jonas Stawski (@jstawski)
 *
 * Released under the MIT License.
 *
 * v 1.0.0
 * 
 * C# Adaptation of https://github.com/mailcheck/mailcheck
 */

namespace FishAngler.MailCheck
{
    public interface IMailCheck
    {
        EmailSuggestion Suggest(string email);
        EmailComposition SplitEmail(string email);
    }
}
