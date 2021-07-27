using System;
using System.Collections.Generic;

namespace RhymeFinder
{
    public class Word
    {
        public readonly string Value;
        public readonly IReadOnlyList<Phone> Phones;
        public readonly IReadOnlyList<Symbol> Symbols;

        public Word(string value, IReadOnlyList<Phone> phones, IReadOnlyList<Symbol> symbols)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
            Phones = phones ?? throw new ArgumentNullException(nameof(phones));
            Symbols = symbols ?? throw new ArgumentNullException(nameof(symbols));
        }
    }
}