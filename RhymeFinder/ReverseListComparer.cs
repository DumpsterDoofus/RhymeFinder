using System;
using System.Collections.Generic;

namespace RhymeFinder
{
    public class RhymeComparer : IComparer<Word>
    {
        private readonly bool _useSymbols;

        public RhymeComparer(bool useSymbols) =>
            _useSymbols = useSymbols;

        public int Compare(Word? x, Word? y)
        {
            if (x == null)
            {
                throw new ArgumentNullException(nameof(x));
            }

            if (y == null)
            {
                throw new ArgumentNullException(nameof(y));
            }

            return _useSymbols
                ? Compare(x.Symbols, y.Symbols)
                : Compare(x.Phones, y.Phones);
        }

        public int Similarity(Word x, Word y) =>
            _useSymbols
                ? SimilarityInternal(x.Symbols, y.Symbols)
                : SimilarityInternal(x.Phones, y.Phones);

        private static int Compare<TComparable>(IReadOnlyList<TComparable> x, IReadOnlyList<TComparable> y)
        where TComparable : IComparable
        {
            if (x.Count == 0)
            {
                return y.Count == 0
                    ? 0
                    : -1;
            }

            if (y.Count == 0)
            {
                return 1;
            }
            var xIndex = x.Count - 1;
            var yIndex = y.Count - 1;
            while (xIndex >= 0 && yIndex >= 0)
            {
                var compare = x[xIndex].CompareTo(y[yIndex]);
                if (compare != 0)
                {
                    return compare;
                }
                xIndex--;
                yIndex--;
            }

            if (xIndex == -1)
            {
                return yIndex == -1
                    ? 0
                    : -1;
            }

            return 1;
        }

        private static int SimilarityInternal<TComparable>(IReadOnlyList<TComparable> x, IReadOnlyList<TComparable> y)
        where TComparable : IComparable
        {
            var xIndex = x.Count - 1;
            var yIndex = y.Count - 1;
            var similarity = 0;
            while (xIndex >= 0 && yIndex >= 0)
            {
                var compare = x[xIndex].CompareTo(y[yIndex]);
                if (compare != 0)
                {
                    break;
                }
                xIndex--;
                yIndex--;
                similarity++;
            }

            return similarity;
        }
    }
}