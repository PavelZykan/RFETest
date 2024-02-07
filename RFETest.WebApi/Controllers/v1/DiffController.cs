using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RFETest.Core.Diff;
using RFETest.Core.Values;
using RFETest.WebContracts;

namespace RFETest.WebApi.Controllers.v1
{
    /// <summary>
    /// Controller for storing values to be compared, and to retrieve diff of stored values
    /// </summary>
    [ApiController]
    [Route("v1/diff")]
    public class DiffController : ControllerBase
    {
        private readonly IDiffValueStorage _storage;
        private readonly IDiffProvider _diffProvider;
        private readonly IMapper _mapper;

        public DiffController(IDiffValueStorage storage, IDiffProvider diffProvider, IMapper mapper)
        {
            _storage = storage;
            _diffProvider = diffProvider;
            _mapper = mapper;
        }

        /// <summary>
        /// Endpoint for storing "left" value to be compared.
        /// </summary>
        /// <param name="id">ID of the comparison (is used to pair the left and right values)</param>
        /// <param name="input">Input</param>
        /// <returns>HTTP 200, no response body</returns>
        [HttpPost("{id}/left")]
        public async Task<ActionResult> PostLeftValue(string id, [FromBody] DiffInput input)
        {
            await _storage.StoreValueAsync(id, input.Input, EDiffValueType.Left);

            return Ok();
        }

        /// <summary>
        /// Endpoint for storing "right" value to be compared.
        /// </summary>
        /// <param name="id">ID of the comparison (is used to pair the left and right values)</param>
        /// <param name="input">Input</param>
        /// <returns>HTTP 200, no response body</returns>
        [HttpPost("{id}/right")]
        public async Task<ActionResult> PostRightValue(string id, [FromBody] DiffInput input)
        {
            await _storage.StoreValueAsync(id, input.Input, EDiffValueType.Right);

            return Ok();
        }

        /// <summary>
        /// Retrieve diff of "left" and "right" values belonging to the given ID
        /// </summary>
        /// <param name="id">Comparison ID</param>
        /// <returns>HTT 200, body with result. HTTP 404 in case one or both values are missing</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<DiffOutput>> GetDiff(string id)
        {
            var result = await _diffProvider.GetDiffAsync(id);

            return Ok(_mapper.Map<DiffOutput>(result));
        }
    }
}
