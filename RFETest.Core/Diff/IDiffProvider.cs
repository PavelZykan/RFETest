using System;
using System.Collections.Generic;
using System.Text;

namespace RFETest.Core.Diff
{
    /// <summary>
    /// Provides results of difference comparisons
    /// </summary>
    public interface IDiffProvider
    {
        /// <summary>
        /// Return comparison result for the given ID
        /// </summary>
        /// <param name="id">Comparison ID</param>
        /// <returns>Instance of DiffResult</returns>
        Task<DiffResult> GetDiffAsync(string id);
    }
}
