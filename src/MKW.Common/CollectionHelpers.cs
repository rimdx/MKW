namespace MKW.Common
{
    public static class CollectionHelpers
    {
        public static IEnumerable<LeftRightPair<T>> Merge<T>(IEnumerable<T> left,
                                                             IEnumerable<T> right,
                                                             IComparer<T> comparer)
            where T : class
        {
            List<T> leftSorted = [.. left];
            List<T> rightSorted = [.. right];

            leftSorted.Sort(comparer);
            rightSorted.Sort(comparer);

            using IEnumerator<T> leftEnum = leftSorted.GetEnumerator();
            using IEnumerator<T> rightEnum = rightSorted.GetEnumerator();

            bool leftHasValue = leftEnum.MoveNext();
            bool rightHasValue = rightEnum.MoveNext();

            while (leftHasValue && rightHasValue)
            {
                int diff = comparer.Compare(leftEnum.Current, rightEnum.Current);

                if (diff == 0)
                {
                    yield return new LeftRightPair<T>(leftEnum.Current, rightEnum.Current);

                    leftHasValue = leftEnum.MoveNext();
                    rightHasValue = rightEnum.MoveNext();
                }
                else if (diff < 0)
                {
                    yield return new LeftRightPair<T>(leftEnum.Current, null);
                    leftHasValue = leftEnum.MoveNext();
                }
                else if (diff > 0)
                {
                    yield return new LeftRightPair<T>(null, rightEnum.Current);
                    rightHasValue = rightEnum.MoveNext();
                }
                else
                {
                    // never reached
                }
            }

            while (leftHasValue)
            {
                yield return new LeftRightPair<T>(leftEnum.Current, null);
                leftHasValue = leftEnum.MoveNext();
            }

            while (rightHasValue)
            {
                yield return new LeftRightPair<T>(null, rightEnum.Current);
                rightHasValue = rightEnum.MoveNext();
            }
        }
    }
}
