using Moq;
using RFETest.Core.Diff;
using RFETest.Core.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFETest.Core.Tests
{
    /// <summary>
    /// This test class is using Moq, despite the fact that in latest versions the library somehow pushes monetization in potentially non-safe way
    /// The version used here should be safe, however an alternative should be picked for production code.
    /// Moq is used due to the authoe being familiar with it.
    /// </summary>
    [TestClass]
    public class DiffProviderTests
    {
        [TestMethod]
        public async Task MatchIsOk()
        {
            var storageMock = new Mock<IDiffValueStorage>();

            var provider = new DiffProvider(storageMock.Object);

            storageMock.Setup(x => x.GetValuesAsync("id1")).ReturnsAsync(new DiffValues("value", "value"));

            var result = await provider.GetDiffAsync("id1");

            Assert.AreEqual(EDiffResultType.Match, result.Result);
        }

        [TestMethod]
        public async Task SizeMismatchIsOk()
        {
            var storageMock = new Mock<IDiffValueStorage>();

            var provider = new DiffProvider(storageMock.Object);

            storageMock.Setup(x => x.GetValuesAsync("id1")).ReturnsAsync(new DiffValues("short", "veryverylong"));

            var result = await provider.GetDiffAsync("id1");

            Assert.AreEqual(EDiffResultType.SizeMismatch, result.Result);
        }


        [TestMethod]
        public async Task ContentMismatchIsOk()
        {
            var storageMock = new Mock<IDiffValueStorage>();

            var provider = new DiffProvider(storageMock.Object);

            var value1 = "same-111-endswithnumber6";
            var value2 = "same-222-endswithnumber7";

            storageMock.Setup(x => x.GetValuesAsync("id1")).ReturnsAsync(new DiffValues(value1, value2));

            var result = await provider.GetDiffAsync("id1");

            Assert.AreEqual(EDiffResultType.ContentMismatch, result.Result);

            var differences = result.Differences.OrderBy(x => x.Index).ToList();

            Assert.AreEqual(2, differences.Count);
            Assert.AreEqual(5, differences[0].Index);
            Assert.AreEqual(3, differences[0].Length);
            Assert.AreEqual(value1.Length-1, differences[1].Index);
            Assert.AreEqual(1, differences[1].Length);
        }
    }
}
