using RFETest.Core.Values;
using System;
using System.Collections.Generic;
using System.Text;

namespace RFETest.Core.Diff
{
    /// <summary>
    /// Compares values of the given comparison ID
    /// </summary>
    public class DiffProvider : IDiffProvider
    {
        private readonly IDiffValueStorage _diffValueStorage;

        public DiffProvider(IDiffValueStorage diffValueStorage)
        {
            _diffValueStorage = diffValueStorage;
        }

        public async Task<DiffResult> GetDiffAsync(string id)
        {
            var values = await _diffValueStorage.GetValuesAsync(id);

            // simple case sensitive comparison
            if (values.Left == values.Right)
            {
                return new DiffResult(EDiffResultType.Match);
            }

            // sizes do not match => SizeMismatch
            if (values.Left.Length != values.Right.Length)
            {
                return new DiffResult(EDiffResultType.SizeMismatch);
            }

            // now we know the sizes match, and by the nature of the values they are not null, so calculate differences
            return new DiffResult(EDiffResultType.ContentMismatch, GetDifferences(values.Left, values.Right));
        }

        private IEnumerable<DiffLocation> GetDifferences(string left, string right)
        {
            var inMismatch = false;
            var mismatchStartIdx = -1;
            var mismatchLength = -1;

            // iterate over the string (lengths should be the same for both)
            for (var i = 0; i < left.Length; i++)
            {
                if (left[i] != right[i])
                {
                    // strings are mismatched at this position, and it is the beginning of a difference
                    if (!inMismatch)
                    {
                        inMismatch = true;
                        mismatchStartIdx = i;
                        mismatchLength = 0;
                    }

                    // in all cases increase the length of the difference
                    mismatchLength++;
                }
                else
                {
                    // the strings are the same at this position, but the previous substring was somehow different
                    // so yield the difference location
                    if (inMismatch)
                    {
                        yield return new DiffLocation(mismatchStartIdx, mismatchLength);
                        inMismatch = false;
                    }
                }
            }

            // in case the difference ended at the end of the whole string, we need to check and potentially return the difference
            if (inMismatch)
                yield return new DiffLocation(mismatchStartIdx, mismatchLength);
        }
    }
}
