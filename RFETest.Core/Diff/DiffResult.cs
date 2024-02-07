namespace RFETest.Core.Diff
{
    /// <summary>
    /// Holds result of comparison
    /// </summary>
    public class DiffResult
    {
        /// <summary>
        /// Type of result
        /// </summary>
        public EDiffResultType Result { get; }

        /// <summary>
        /// Locations (offset+length) of differences; only relevant for Result == ContentMismatch; it is empty otherwise
        /// </summary>
        public IEnumerable<DiffLocation> Differences { get; }

        public DiffResult(EDiffResultType result, IEnumerable<DiffLocation>? differences = null)
        {
            Result = result;
            Differences = differences ?? Enumerable.Empty<DiffLocation>();
        }
    }
}