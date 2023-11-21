using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rent.Entities.Responses;
using Rent.Entities.Users;
using Rent.Service.Services.Responses;
using Rent.Service.Services.Responses.Descriptors;
using RentAPI.Infrastructure.Extensions;
using RentAPI.Models.Responses;

namespace RentAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ResponseController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IResponseService _responseService;

        public ResponseController(
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            IResponseService responseService)
        {
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _responseService = responseService;
        }

        [Authorize(Roles = Roles.Tenant)]
        [HttpPost("add")]
        public async Task<IActionResult> AddNewResponse(AddResponseModel model)
        {
            var entity = _mapper.Map<Response>(model);
            entity.TenantId = _httpContextAccessor.HttpContext.User.GetUserId();

            await _responseService.Add(entity);

            return Ok("Added successfully!");
        }

        [Authorize(Roles = Roles.Landlord)]
        [HttpGet("items/{propertyId}")]
        public async Task<IActionResult> GetResponseByPropertyId(int propertyId)
        {
            string landlordId = _httpContextAccessor.HttpContext.User.GetUserId();

            var responses = await _responseService.GetAllResponsesByPropertyId(propertyId, landlordId);

            return Ok(responses);
        }

        [Authorize(Roles = Roles.Landlord)]
        [HttpPatch("process")]
        public async Task<IActionResult> Process(ProcessResponseModel model)
        {
            var descriptor = _mapper.Map<ProcessResponseDescriptor>(model);
            descriptor.LandlordId = _httpContextAccessor.HttpContext.User.GetUserId();

            await _responseService.Process(descriptor);

            return Ok("Status updated successfully!");
        }
    }
}
