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

namespace FishAngler.MailCheck.Tests
{
    [TestFixture]
    public class SplitEmailTests
    {
        MailCheck check;

        [SetUp]
        public void Setup()
        {
            check = new MailCheck();
        }

        [TestCase("test@example.com", "test", "example.com", "com", "example")]
        [TestCase("test@example.co.uk", "test", "example.co.uk", "co.uk", "example")]
        [TestCase("test@mail.randomsmallcompany.co.uk", "test", "mail.randomsmallcompany.co.uk", "randomsmallcompany.co.uk", "mail")]
        public void it_returns_a_hash_of_the_address_the_domain_and_the_top_level_domain(string email, string address, string domain, string topLevelDomain, string secondLevelDomain)
        {
            // Act 
            var ec = check.SplitEmail(email);

            // Assert
            Assert.AreEqual(ec.Address, address);
            Assert.AreEqual(ec.Domain, domain);
            Assert.AreEqual(ec.TopLevelDomain, topLevelDomain);
            Assert.AreEqual(ec.SecondLevelDomain, secondLevelDomain);
        }

        [TestCase(@"""foo@bar""@example.com", @"""foo@bar""", "example.com", "com", "example")]
        [TestCase("containsnumbers1234567890@example.com", "containsnumbers1234567890", "example.com", "com", "example")]
        [TestCase("contains+symbol@example.com", "contains+symbol", "example.com", "com", "example")]
        [TestCase("contains-symbol@example.com", "contains-symbol", "example.com", "com", "example")]
        [TestCase("contains.symbol@example.com", "contains.symbol", "example.com", "com", "example")]
        [TestCase(@"""contains.and\ symbols""@example.com", @"""contains.and\ symbols""", "example.com", "com", "example")]
        [TestCase(@"""contains.and.@.symbols.com""@example.com", @"""contains.and.@.symbols.com""", "example.com", "com", "example")]
        [TestCase(@"""()<>[]:;@,\\\""!#$%&\'*+-/=?^_`{}|\ \ \ \ \ ~\ \ \ \ \ \ \ ?\ \ \ \ \ \ \ \ \ \ \ \ ^_`{}|~.a""@allthesymbols.com", @"""()<>[]:;@,\\\""!#$%&\'*+-/=?^_`{}|\ \ \ \ \ ~\ \ \ \ \ \ \ ?\ \ \ \ \ \ \ \ \ \ \ \ ^_`{}|~.a""", "allthesymbols.com", "com", "allthesymbols")]
        [TestCase("postbox@com", "postbox", "com", "com", "")]
        public void it_splits_RFC_compliant_emails(string email, string address, string domain, string topLevelDomain, string secondLevelDomain)
        {
            // Act 
            var ec = check.SplitEmail(email);

            // Assert
            Assert.AreEqual(ec.Address, address);
            Assert.AreEqual(ec.Domain, domain);
            Assert.AreEqual(ec.TopLevelDomain, topLevelDomain);
            Assert.AreEqual(ec.SecondLevelDomain, secondLevelDomain);
        }

        [TestCase("example.com")]
        [TestCase("abc.example.com")]
        [TestCase("@example.com")]
        [TestCase("test@")]
        public void it_returns_null_for_email_addresses_that_are_not_RFC_compliant(string email)
        {
            // Act 
            var ec = check.SplitEmail(email);

            // Assert
            Assert.IsNull(ec);
        }

        [TestCase(" postbox@com ")]
        [TestCase("postbox@com ")]
        [TestCase(" postbox@com")]
        public void it_trims_spaces_from_start_and_end_of_the_string(string email)
        {
            // Act 
            var ec = check.SplitEmail(email);

            // Assert
            Assert.AreEqual(ec.Address, "postbox");
            Assert.AreEqual(ec.Domain, "com");
            Assert.AreEqual(ec.TopLevelDomain, "com");
            Assert.AreEqual(ec.SecondLevelDomain, "");
        }
    }
}

