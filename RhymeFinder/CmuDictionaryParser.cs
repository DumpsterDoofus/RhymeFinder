using System;
using System.Collections.Generic;
using System.Linq;

namespace RhymeFinder
{
    public static class CmuDictionaryParser
    {
        private const char Separator = ' ';
        private const string CommentToken = ";;;";

        public static List<Word> Parse(IEnumerable<string> lines) => lines
            .Where(line => !line.StartsWith(CommentToken) && !string.IsNullOrEmpty(line))
            .Select(Parse)
            .ToList();

        /// <summary>
        /// Parses a CMU phonetic dictionary line. Example line:
        /// AARDVARKS  AA1 R D V AA2 R K S
        /// </summary>
        public static Word Parse(string line)
        {
            var index = line.IndexOf(Separator, StringComparison.Ordinal);
            var value = line.Substring(0, index);
            var phones = new List<Phone>();
            var symbols = new List<Symbol>();
            foreach (var symbolText in line.Substring(index + 2).Split())
            {
                var phoneText = symbolText.Substring(0, Math.Min(2, symbolText.Length));
                if (!Enum.TryParse(phoneText, out Phone phone))
                {
                    throw new Exception($"Symbol {symbolText} could not be parsed into a phone.");
                }
                if (!Enum.TryParse(symbolText, out Symbol symbol))
                {
                    throw new Exception($"Symbol {symbolText} could not be parsed into a phone.");
                }
                phones.Add(phone);
                symbols.Add(symbol);
            }
            return new Word(value, phones, symbols);
        }
    }
}