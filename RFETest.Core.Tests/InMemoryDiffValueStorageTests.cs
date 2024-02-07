using RFETest.Core.Values;
using System.ComponentModel.DataAnnotations;

namespace RFETest.Core.Tests
{
    [TestClass]
    public class InMemoryDiffValueStorageTests
    {
        [TestMethod]
        [DataRow("id1", EDiffValueType.Left, "foo")]
        [DataRow("id2", EDiffValueType.Right, "bar")]
        public async Task StoreIsOk(string id, EDiffValueType type, string value)
        {
            var storage = new InMemoryDiffValueStorage();

            await storage.StoreValueAsync(id, value, type);
        }

        [TestMethod]
        [DataRow("id1", "foo", "bar")]
        public async Task StoreAndGetIsOk(string id, string valueLeft, string valueRight)
        {
            var storage = new InMemoryDiffValueStorage();

            await storage.StoreValueAsync(id, valueLeft, EDiffValueType.Left);
            await storage.StoreValueAsync(id, valueRight, EDiffValueType.Right);

            var result = await storage.GetValuesAsync(id);

            Assert.AreEqual(valueLeft, result.Left);
            Assert.AreEqual(valueRight, result.Right);
        }

        [TestMethod]
        [DataRow(EDiffValueType.Left)]
        [DataRow(EDiffValueType.Right)]
        public async Task StoreAndGetIncompleteThrows(EDiffValueType type)
        {
            var storage = new InMemoryDiffValueStorage();

            await storage.StoreValueAsync("id1", "bar", type);

            await Assert.ThrowsExceptionAsync<ComparisonIDIncompleteException>(() => storage.GetValuesAsync("id1"));
        }

        [TestMethod]
        public async Task StoreAndGetIdMissingThrows()
        {
            var storage = new InMemoryDiffValueStorage();

            await Assert.ThrowsExceptionAsync<ComparisonIDIncompleteException>(() => storage.GetValuesAsync("notexists"));
        }
    }
}