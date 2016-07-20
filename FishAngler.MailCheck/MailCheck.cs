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

using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace FishAngler.MailCheck
{
    public class MailCheck : IMailCheck
    {
        static int domainThreshold = 2, secondLevelThreshold = 2, topLevelThreshold = 2;

        static string[] defaultDomains = { "msn.com", "bellsouth.net", "telus.net", "comcast.net", "optusnet.com.au",
            "earthlink.net", "qq.com", "sky.com", "icloud.com", "mac.com", "sympatico.ca", "googlemail.com",
            "att.net", "xtra.co.nz", "web.de", "cox.net", "gmail.com", "ymail.com", "aim.com", "rogers.com", "verizon.net",
            "rocketmail.com", "google.com", "optonline.net", "sbcglobal.net", "aol.com", "me.com", "btinternet.com",
            "charter.net", "shaw.ca" };

        static string[] defaultSecondLevelDomains = { "yahoo", "hotmail", "mail", "live", "outlook", "gmx" };

        static string[] defaultTopLevelDomains = { "com", "com.ar", "com.au", "com.tw", "ca", "co.nz", "co.uk", "de",
            "fr", "it", "ru", "net", "org", "edu", "gov", "jp", "nl", "kr", "se", "eu",
            "ie", "co.il", "us", "at", "be", "dk", "hk", "es", "gr", "ch", "no", "cz",
            "in", "net", "net.au", "info", "biz", "mil", "co.jp", "sg", "hu", "uk" };

        string[] _domains;
        string[] _secondLevelDomains;
        string[] _topLevelDomains;

        public MailCheck (string[] domains, string[] secondLevelDomains, string[] topLevelDomains)
        {
            _domains = domains;
            _secondLevelDomains = secondLevelDomains;
            _topLevelDomains = topLevelDomains;
        }

        public MailCheck() : this(defaultDomains, defaultSecondLevelDomains, defaultTopLevelDomains)
        {
        }

        public EmailSuggestion Suggest(string email)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentException("Invalid email!", nameof(email));
            
            email = email.ToLower();

            var emailParts = SplitEmail(email);

            if (emailParts == null)
                throw new ArgumentException("Invalid email!", nameof(email));

            if (_secondLevelDomains != null && _topLevelDomains != null)
            {
                // If the email is a valid 2nd-level + top-level, do not suggest anything.
                if (_secondLevelDomains.Where(d => d == emailParts.SecondLevelDomain).Any() && _topLevelDomains.Where(d => d == emailParts.TopLevelDomain).Any())
                {
                    return null;
                }
            }

            var closestDomain = FindClosestDomain(emailParts.Domain, _domains, domainThreshold);

            if (!string.IsNullOrEmpty(closestDomain))
            {
                if (closestDomain == emailParts.Domain)
                {
                    // The email address exactly matches one of the supplied domains; do not return a suggestion.
                    return null;
                }
                else 
                {
                    // The email address closely matches one of the supplied domains; return a suggestion
                    return new EmailSuggestion { Address = emailParts.Address, Domain = closestDomain, Full = $"{emailParts.Address}@{closestDomain}" };
                }
            }

            // The email address does not closely match one of the supplied domains
            var closestSecondLevelDomain = FindClosestDomain(emailParts.SecondLevelDomain, _secondLevelDomains, secondLevelThreshold);
            var closestTopLevelDomain = FindClosestDomain(emailParts.TopLevelDomain, _topLevelDomains, topLevelThreshold);

            if (!string.IsNullOrEmpty(emailParts.Domain))
            {
                closestDomain = emailParts.Domain;
                var rtrn = false;

                if (closestSecondLevelDomain != null && closestSecondLevelDomain != emailParts.SecondLevelDomain)
                {
                    // The email address may have a mispelled second-level domain; return a suggestion
                    closestDomain = closestDomain.Replace(emailParts.SecondLevelDomain, closestSecondLevelDomain);
                    rtrn = true;
                }

                if (closestTopLevelDomain != null && closestTopLevelDomain != emailParts.TopLevelDomain)
                {
                    // The email address may have a mispelled top-level domain; return a suggestion
                    var regex = new Regex($"{emailParts.TopLevelDomain}$");
                    closestDomain = regex.Replace(closestDomain, closestTopLevelDomain);
                    rtrn = true;
                }

                if (rtrn)
                {
                    return new EmailSuggestion { Address = emailParts.Address, Domain = closestDomain, Full = $"{emailParts.Address}@{closestDomain}" };
                }
            }

            /* The email address exactly matches one of the supplied domains, does not closely
             * match any domain and does not appear to simply have a mispelled top-level domain,
             * or is an invalid email address; do not return a suggestion.
             */
            return null;
        }

        string FindClosestDomain(string domain, string[] domains, int threshold)
        {
            int dist;
            var minDist = Int32.MaxValue;
            string closestDomain = null;

            if (string.IsNullOrEmpty(domain) || domains == null || domains.Length == 0)
            {
                return null;
            }

            for (var i = 0; i < domains.Length; i++)
            {
                if (domain == domains[i])
                {
                    return domain;
                }
                dist = Sift3Distance(domain, domains[i]);
                if (dist < minDist)
                {
                    minDist = dist;
                    closestDomain = domains[i];
                }
            }

            if (minDist <= threshold && closestDomain != null)
            {
                return closestDomain;
            }
            else 
            {
                return null;
            }
        }

        int Sift3Distance(string s1, string s2)
        {
            // sift3: http://siderite.blogspot.com/2007/04/super-fast-and-accurate-string-distance.html
            if (string.IsNullOrEmpty(s1))
            {
                if (string.IsNullOrEmpty(s2))
                    return 0;
                else 
                    return s2.Length;
            }

            if (string.IsNullOrEmpty(s2))
                return s1.Length;

            int c = 0, offset1 = 0, offset2 = 0, lcs = 0, maxOffset = 5;

            while ((c + offset1 < s1.Length) && (c + offset2 < s2.Length))
            {
                if (s1[c + offset1] == s2[c + offset2])
                    lcs++;
                else 
                {
                    offset1 = 0;
                    offset2 = 0;
                    for (var i = 0; i < maxOffset; i++)
                    {
                        if ((c + i < s1.Length) && (s1[c + i] == s2[c]))
                        {
                            offset1 = i;
                            break;
                        }
                        if ((c + i < s2.Length) && (s1[c] == s2[c + i]))
                        {
                            offset2 = i;
                            break;
                        }
                    }
                }
                c++;
            }
            return (s1.Length + s2.Length) / 2 - lcs;
        }

        public EmailComposition SplitEmail(string email)
        {
            var parts = email.Trim().Split('@');

            if (parts.Length < 2)
            {
                return null;
            }

            for (var i = 0; i < parts.Length; i++)
            {
                if (string.IsNullOrEmpty(parts[i]))
                {
                    return null;
                }
            }

            var domain = parts[parts.Length - 1];
            var domainParts = domain.Split('.');
            var sld = "";
            var tld = "";

            if (domainParts.Length == 0)
            {
                // The address does not have a top-level domain
                return null;
            }
            else if (domainParts.Length == 1)
            {
                // The address has only a top-level domain (valid under RFC)
                tld = domainParts[0];
            }
            else 
            {
                // The address has a domain and a top-level domain
                sld = domainParts[0];
                for (var i = 1; i < domainParts.Length; i++)
                {
                    tld += domainParts[i] + ".";
                }
                tld = tld.Substring(0, tld.Length - 1);
            }

            return new EmailComposition
            {
                TopLevelDomain = tld,
                SecondLevelDomain = sld,
                Domain = domain,
                Address = parts.Length > 2 ? string.Join("@", parts.Take(parts.Length - 1).ToArray()) : parts[0]
            };
        }
    }
}

