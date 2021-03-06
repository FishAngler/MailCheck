﻿/*
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
    public class EmailSuggestion
    {
        public string Address { get; set; }
        public string Domain { get; set; }
        public string Full { get; set; }
    }
}

