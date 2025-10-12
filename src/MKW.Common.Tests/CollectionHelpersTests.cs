using NUnit.Framework.Legacy;

namespace MKW.Common.Tests
{
    public class CollectionHelpersTests
    {
        [Test]
        public void SimpleMergeTest()
        {
            List<string> left = ["1", "2"];
            List<string> right = ["2", "3"];

            List<LeftRightPair<string>> diff = CollectionHelpers.Merge(left, right, Comparer<string>.Default).ToList();

            CollectionAssert.AreEqual(
                new[]
                {
                    new LeftRightPair<string>("1", null),
                    new LeftRightPair<string>("2", "2"),
                    new LeftRightPair<string>(null, "3"),
                },
                diff);
        }

        [Test]
        public void SimpleMergeReverseTest()
        {
            List<string> left = ["2", "3"];
            List<string> right = ["1", "2"];

            List<LeftRightPair<string>> diff = CollectionHelpers.Merge(left, right, Comparer<string>.Default).ToList();

            CollectionAssert.AreEqual(
                new[]
                {
                    new LeftRightPair<string>(null, "1"),
                    new LeftRightPair<string>("2", "2"),
                    new LeftRightPair<string>("3", null),
                },
                diff);
        }

        [Test]
        public void SimpleMergeGapsTest()
        {
            List<string> left = ["1", "2", "3"];
            List<string> right = ["3", "4"];

            List<LeftRightPair<string>> diff = CollectionHelpers.Merge(left, right, Comparer<string>.Default).ToList();

            CollectionAssert.AreEqual(
                new[]
                {
                    new LeftRightPair<string>("1", null),
                    new LeftRightPair<string>("2", null),
                    new LeftRightPair<string>("3", "3"),
                    new LeftRightPair<string>(null, "4"),
                },
                diff);
        }

        [Test]
        public void SimpleMergeGapsReverseTest()
        {
            List<string> left = ["3", "4"];
            List<string> right = ["1", "2", "3"];

            List<LeftRightPair<string>> diff = CollectionHelpers.Merge(left, right, Comparer<string>.Default).ToList();

            CollectionAssert.AreEqual(
                new[]
                {
                    new LeftRightPair<string>(null, "1"),
                    new LeftRightPair<string>(null, "2"),
                    new LeftRightPair<string>("3", "3"),
                    new LeftRightPair<string>("4", null),
                },
                diff);
        }

        [Test]
        public void SimpleMergeStringsTest()
        {
            List<string> left = ["abc", "123"];
            List<string> right = ["abc", "xyz"];

            List<LeftRightPair<string>> diff = CollectionHelpers.Merge(left, right, Comparer<string>.Default).ToList();

            CollectionAssert.AreEqual(
                new[]
                {
                    new LeftRightPair<string>("123", null),
                    new LeftRightPair<string>("abc", "abc"),
                    new LeftRightPair<string>(null, "xyz"),
                },
                diff);
        }
    }
}
