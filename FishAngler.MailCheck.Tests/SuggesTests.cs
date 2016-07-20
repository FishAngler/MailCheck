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

using NUnit.Framework;
using System;
namespace FishAngler.MailCheck.Tests
{
    [TestFixture]
    public class SuggestTests
    {
        MailCheck check;
        string[] domains = { "google.com", "gmail.com", "emaildomain.com", "comcast.net", "facebook.com", "msn.com", "gmx.de" };
        string[] secondLevelDomains = { "yahoo", "hotmail", "mail", "live", "outlook", "gmx" };
        string[] topLevelDomains = { "co.uk", "com", "org", "info", "fr" };

        [SetUp]
        public void Setup()
        {
            check = new MailCheck(domains, secondLevelDomains, topLevelDomains);
        }

        [TestCase("test@gmailc.om", "gmail.com")]
        [TestCase("test@emaildomain.co", "emaildomain.com")]
        [TestCase("test@gmail.con", "gmail.com")]
        [TestCase("test@gnail.con", "gmail.com")]
        [TestCase("test@GNAIL.con", "gmail.com")]
        [TestCase("test@#gmail.com", "gmail.com")]
        [TestCase("test@comcast.nry", "comcast.net")]
        [TestCase("test@homail.con", "hotmail.com")]
        [TestCase("test@hotmail.co", "hotmail.com")]
        [TestCase("test@yajoo.com", "yahoo.com")]
        [TestCase("test@randomsmallcompany.cmo", "randomsmallcompany.com")]
        [TestCase("test@con-artists.con", "con-artists.com")]
        public void it_should_have_suggestion(string email, string expectedDomain)
        {
            // Act
            var suggestion = check.Suggest(email);

            // Assert
            Assert.AreEqual(expectedDomain, suggestion.Domain);
        }

        [TestCase("test@some-random-domain.com")]
        [TestCase("test@mail.randomsmallcompany.cmo")]
        public void it_should_not_have_suggestion(string email) 
        {
            // Act
            var suggestion = check.Suggest(email);

            // Assert
            Assert.IsNull(suggestion);
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("test")]
        [TestCase("test@")]
        public void it_should_throw_invalid_argument_exception_when_there_is_invalid_email(string email)
        {
            Assert.Throws<ArgumentException>(() => check.Suggest(email));
        }

        [Test]
        public void it_will_not_offer_a_suggestion_that_itself_leads_to_another_suggestion()
        {
            // Arrange
            var email = "test@yahooo.cmo";

            // Act 
            var suggestion = check.Suggest(email);

            // Assert
            Assert.AreEqual(suggestion.Domain, "yahoo.com");
        }

        [Test]
        public void it_will_not_offer_suggestion_for_a_valid_2ld_tld_combination()
        {
            // Arrange
            var email = "test@yahoo.co.uk";

            // Act 
            var suggestion = check.Suggest(email);

            // Assert
            Assert.IsNull(suggestion);
        }

        [Test]
        public void it_will_not_offer_suggestion_for_2ld_tld_even_if_there_is_a_close_fully_specified_domain()
        {
            // Arrange
            var email = "test@gmx.fr";

            // Act 
            var suggestion = check.Suggest(email);

            // Assert
            Assert.IsNull(suggestion);
        }
    }
}

