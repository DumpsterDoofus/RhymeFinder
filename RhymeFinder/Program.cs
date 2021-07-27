using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace RhymeFinder
{
    public static class Program
    {
        public static void Main(params string[] args)
        {
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            var useSymbols = args.Length != 0 && args[0].ToLower() == "uselexicalstress";
            Console.WriteLine(useSymbols
                ? "Setting up rhyme dictionary, taking into account ARPAbet lexical stress. This will produce more exact rhyme results, but may reduce the number of results found."
                : "Setting up rhyme dictionary, and ignoring ARPAbet lexical stress. This will give more rhyme results, but the results may not be as exact. To enable lexical stress awareness, pass 'uselexicalstress' as a command-line argument.");
            var stopwatch = Stopwatch.StartNew();
            var lines = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "cmudict-0.7b"));
            var commonWords = File.ReadLines("wiki-100k.txt")
                .Where(l => !l.StartsWith('#'))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            var words = CmuDictionaryParser.Parse(lines);
            var rhymeComparer = new RhymeComparer(useSymbols);
            words.Sort(rhymeComparer);
            var lookup = words
                .Select((word, index) => (word, index))
                .ToDictionary(pair => pair.word.Value, pair => pair.index);
            var maxLength = words.Max(w => w.Value.Length);
            Console.WriteLine($"Set up rhyme dictionary in {stopwatch.ElapsedMilliseconds} milliseconds.");
            while (true)
            {
                Beginning:
                Console.WriteLine();
                Console.WriteLine("Enter a word to rhyme with.");
                var input = Console.ReadLine();
                if (input == null)
                {
                    continue;
                }
                if (!lookup.TryGetValue(input.ToUpper(), out var index))
                {
                    Console.WriteLine($"Word '{input}' was not found in the dictionary.");
                    Console.WriteLine();
                    continue;
                }
                var word = words[index];
                var results = new List<List<Word>>();
                var maxSimilarity = rhymeComparer.Similarity(word, word);
                for (var i = 0; i < maxSimilarity; i++)
                {
                    results.Add(new List<Word>());
                }
                for (var i = index - 1; i >= 0; i--)
                {
                    var otherWord = words[i];
                    var similarity = rhymeComparer.Similarity(word, otherWord);
                    if (similarity == 0)
                    {
                        break;
                    }
                    results[similarity - 1].Add(otherWord);
                }
                for (var i = index + 1; i < words.Count; i++)
                {
                    var otherWord = words[i];
                    var similarity = rhymeComparer.Similarity(word, otherWord);
                    if (similarity == 0)
                    {
                        break;
                    }
                    results[similarity - 1].Add(otherWord);
                }

                Console.WriteLine();
                for (var i = results.Count - 1; i >= 0; i--)
                {
                    var similarWords = results[i].OrderBy(w => w.Value).ToList();
                    if (similarWords.Count == 0)
                    {
                        continue;
                    }
                    Console.WriteLine($"There were {similarWords.Count} rhymes with similarity {i + 1}. Press Enter to see them, or Esc to search for another word.");
                    while (true)
                    {
                        var input2 = Console.ReadKey();
                        switch (input2.Key)
                        {
                            case ConsoleKey.Escape:
                                goto Beginning;
                            case ConsoleKey.Enter:
                                goto Print;
                        }
                    }
                    Print:
                    foreach (var similarWord in similarWords)
                    {
                        var isCommon = commonWords.Contains(similarWord.Value);
                        if (isCommon)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                        }
                        Console.Write(similarWord.Value.PadRight(maxLength));
                        if (isCommon)
                        {
                            Console.ResetColor();
                        }
                        if (useSymbols)
                        {
                            for (var j = 0; j < similarWord.Symbols.Count; j++)
                            {
                                var symbol = similarWord.Symbols[j];
                                var highlight = j + i + 1 == similarWord.Symbols.Count;
                                if (highlight)
                                {
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                }
                                Console.Write(symbol);
                                Console.Write(' ');
                            }
                        }
                        else
                        {
                            for (var j = 0; j < similarWord.Phones.Count; j++)
                            {
                                var highlight = j + i + 1 == similarWord.Symbols.Count;
                                if (highlight)
                                {
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                }
                                var phone = similarWord.Phones[j];
                                Console.Write(phone);
                                Console.Write(' ');
                            }
                        }
                        Console.ResetColor();
                        Console.WriteLine();
                    }
                    Console.WriteLine();
                }
            }
        }
    }
}