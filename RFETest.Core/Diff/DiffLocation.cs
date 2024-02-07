namespace RFETest.Core.Diff
{
    /// <summary>
    /// Holds offset and length of a found difference
    /// </summary>
    public class DiffLocation
    {
        /// <summary>
        /// 0-based index of difference location (beginning)
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// Length of the substring
        /// </summary>
        public int Length { get; }

        public DiffLocation(int index, int length)
        {
            Index = index;
            Length = length;
        }
    }
}