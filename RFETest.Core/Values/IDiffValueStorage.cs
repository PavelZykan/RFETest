namespace RFETest.Core.Values
{
    /// <summary>
    /// Provides storage for diff values
    /// </summary>
    public interface IDiffValueStorage
    {
        /// <summary>
        /// Store value for the given id and value type (left or right)
        /// </summary>
        /// <param name="id">Comparison ID</param>
        /// <param name="value">String value to be stored</param>
        /// <param name="type">Type of value</param>
        /// <returns>Awaitable task</returns>
        Task StoreValueAsync(string id, string value, EDiffValueType type);

        /// <summary>
        /// Get left and right values for the given comparison ID
        /// </summary>
        /// <param name="id">Comparison ID</param>
        /// <returns>Instance of DiffValues with left or right values; 
        /// throws ComparisonIDIncompleteException if one or both values are missing</returns>
        Task<DiffValues> GetValuesAsync(string id);
    }
}
