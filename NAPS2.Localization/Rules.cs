﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NAPS2.Localization
{
    public static class Rules
    {
        private static readonly Regex SuffixRegex = new Regex(@"[:.]+$");
        private static readonly Regex HotkeyRegex = new Regex(@"&(\w)");
        private static readonly Regex TextPropRegex = new Regex(@"(Text|Items\d+)$");

        public static bool IsTranslatable(bool winforms, string prop, ref string original, out string prefix, out string suffix)
        {
            prefix = "";
            suffix = "";
            if (prop == null || original == null || !original.Any(char.IsLetter))
            {
                return false;
            }
            var match = SuffixRegex.Match(original);
            if (match.Success)
            {
                suffix = match.Value;
                original = original.Substring(0, match.Index);
            }
            if (winforms)
            {
                if (!TextPropRegex.IsMatch(prop))
                {
                    return false;
                }
                var hotkeyMatch = HotkeyRegex.Match(original);
                if (hotkeyMatch.Success)
                {
                    prefix = "&amp;";
                    original = HotkeyRegex.Replace(original, m => m.Groups[2].Value);
                }
            }
            return true;
        }
    }
}
