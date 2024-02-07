using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RFETest.Core.Values
{
    /// <summary>
    /// In memory storage for diff values.
    /// Due to the nature of this storage the app will not be resilient to restarts or other ddisruptions.
    /// Concurrency resolution is "last one wins"
    /// </summary>
    public class InMemoryDiffValueStorage : IDiffValueStorage
    {
        internal class InMemoryDiffValues
        {
            public string? Left { get; set; }

            public string? Right { get; set; }
        }

        private ConcurrentDictionary<string, InMemoryDiffValues> _db = new ConcurrentDictionary<string, InMemoryDiffValues>();

        public async Task<DiffValues> GetValuesAsync(string id)
        {
            await Task.Yield();

            if (_db.TryGetValue(id, out var values))
            {
                return new DiffValues(
                    values.Left ?? throw new ComparisonIDIncompleteException($"Left value for id '{id}' is missing"), 
                    values.Right ?? throw new ComparisonIDIncompleteException($"Right value for id '{id}' is missing"));
            }

            throw new ComparisonIDIncompleteException($"No value found for id '{id}'");
        }

        public async Task StoreValueAsync(string id, string value, EDiffValueType type)
        {
            await Task.Yield();

            _db.AddOrUpdate(id, new InMemoryDiffValues
            {
                Left = type == EDiffValueType.Left ? value : null,
                Right = type == EDiffValueType.Right ? value : null,
            },
            (id, existingVal) =>
            {
                return new InMemoryDiffValues
                {
                    Left = type == EDiffValueType.Left ? value : existingVal.Left,
                    Right = type == EDiffValueType.Right ? value : existingVal.Right,
                };
            });
        }
    }
}
