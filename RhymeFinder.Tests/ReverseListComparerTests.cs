using FluentAssertions;
using Xunit;
using static RhymeFinder.Phone;

namespace RhymeFinder.Tests
{
    public class ReverseListComparerTests
    {
        private readonly ReverseListComparer<Phone> _reverseListComparer = new ReverseListComparer<Phone>();

        [Theory]
        [InlineData(new Phone[0], new Phone[0], 0)]
        [InlineData(new[]{AA}, new[]{AA}, 0)]
        [InlineData(new[]{B}, new[]{AA}, 1)]
        [InlineData(new[]{AA, AA}, new[]{AA}, 1)]
        [InlineData(new[]{B}, new[]{CH, AA}, 1)]
        public void Can_compare_phones(Phone[] left, Phone[] right, int expected)
        {
            var value = _reverseListComparer.Compare(left, right);
            var symmetricValue = _reverseListComparer.Compare(right, left);

            value.Should().Be(expected);
            symmetricValue.Should().Be(-expected);
        }

        [Theory]
        [InlineData(new Phone[0], new Phone[0], 0)]
        [InlineData(new Phone[0], new[]{G}, 0)]
        [InlineData(new[]{G, N}, new[]{G, N}, 2)]
        [InlineData(new[]{AA, G, N}, new[]{G, N}, 2)]
        [InlineData(new[]{B, G}, new[]{B, G, ZH}, 0)]
        public void Can_compare_similarity(Phone[] left, Phone[] right, int expected)
        {
            var similarity = _reverseListComparer.Similarity(left, right);
            var symmetricSimilarity = _reverseListComparer.Similarity(right, left);

            similarity.Should().Be(expected);
            symmetricSimilarity.Should().Be(expected);
        }
    }
}