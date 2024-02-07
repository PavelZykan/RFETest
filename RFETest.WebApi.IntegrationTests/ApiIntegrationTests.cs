using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using RFETest.Utils;
using RFETest.WebContracts;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RFETest.WebApi.IntegrationTests
{
    [TestClass]
    public partial class ApiIntegrationTests : IDisposable
    {
        private readonly HttpClient _client;

        public ApiIntegrationTests()
        {
            var app = new ApplicationUnderTest();
            
            _client = app.CreateClient();
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        [TestMethod]
        public async Task HappyPathIsOk()
        {
            var id = Guid.NewGuid().ToString();

            await PostBase64EncodedInput(id, "value to compare", true);

            await PostBase64EncodedInput(id, "value to compare", false);

            using var response = await GetDiff(id);

            response.EnsureSuccessStatusCode();

            var data = await GetData<DiffOutput>(response);

            Assert.AreEqual(EDiffResultType.Match, data.Result);
            Assert.IsTrue(data.Differences == null || data.Differences.Count() == 0);
            Assert.AreEqual("inputs were equal", data.Message);
        }

        [TestMethod]
        public async Task SizeMismatchIsOk()
        {
            var id = Guid.NewGuid().ToString();

            await PostBase64EncodedInput(id, "value to compare", true);

            await PostBase64EncodedInput(id, "value to compare very long", false);

            using var response = await GetDiff(id);

            response.EnsureSuccessStatusCode();

            var data = await GetData<DiffOutput>(response);

            Assert.AreEqual(EDiffResultType.SizeMismatch, data.Result);
            Assert.IsTrue(data.Differences == null || data.Differences.Count() == 0);
            Assert.AreEqual("inputs are of different size", data.Message);
        }

        [TestMethod]
        public async Task ContentMismatchIsOk()
        {
            var id = Guid.NewGuid().ToString();

            await PostBase64EncodedInput(id, "value to compare", true);

            await PostBase64EncodedInput(id, "value to dissect", false);

            using var response = await GetDiff(id);

            response.EnsureSuccessStatusCode();

            var data = await GetData<DiffOutput>(response);

            Assert.AreEqual(EDiffResultType.ContentMismatch, data.Result);
            Assert.IsTrue(data.Differences.Count() == 1);
            Assert.AreEqual(9, data.Differences.Single().Index);
            Assert.AreEqual(7, data.Differences.Single().Length);
            Assert.AreEqual("inputs have different content", data.Message);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow(true)]
        [DataRow(false)]
        public async Task IncompleteDiffThrows(bool? isLeft)
        {
            var id = Guid.NewGuid().ToString();

            if (isLeft != null)
            {
                await PostBase64EncodedInput(id, "value to compare", isLeft.Value);
            }

            using var response = await GetDiff(id);

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);

            var data = await GetData<ProblemDetails>(response);

            switch (isLeft)
            {
                case null: Assert.AreEqual(data.Detail, $"No value found for id '{id}'"); break;
                case true: Assert.AreEqual(data.Detail, $"Right value for id '{id}' is missing"); break;
                case false: Assert.AreEqual(data.Detail, $"Left value for id '{id}' is missing"); break;
            };
        }

        [TestMethod]
        public async Task ValidationWorks()
        {
            var id = Guid.NewGuid().ToString();

            var responseCode = await PostBase64EncodedInput(id, "", true, ensureSuccessStatusCode: false);

            Assert.AreEqual(HttpStatusCode.BadRequest, responseCode);
        }

        private async Task<T> GetData<T>(HttpResponseMessage message)
        {
            var contentString = await message.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<T>(contentString, SerializationUtils.Options);
        }

        private async Task<HttpResponseMessage> GetDiff(string id)
        {
            using var message = new HttpRequestMessage(HttpMethod.Get, $"v1/diff/{id}");

            return await _client.SendAsync(message);
        }

        private async Task<HttpStatusCode> PostBase64EncodedInput(string id, string value, bool left, bool ensureSuccessStatusCode = true)
        {
            var type = left ? "left" : "right";

            using var message = new HttpRequestMessage(HttpMethod.Post, $"v1/diff/{id}/{type}");

            var input = new DiffInput { Input = value };
            var inputSerialized = JsonSerializer.Serialize(input);
            var inputBytes = Encoding.UTF8.GetBytes(inputSerialized);
            var inputBase64 = Convert.ToBase64String(inputBytes);

            using var content = new StringContent(inputBase64, Encoding.UTF8, "application/custom");

            message.Content = content;
            message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/custom");

            using var response = await _client.SendAsync(message);

            if (ensureSuccessStatusCode)
            {
                response.EnsureSuccessStatusCode();
            }

            return response.StatusCode;
        }
    }
}
