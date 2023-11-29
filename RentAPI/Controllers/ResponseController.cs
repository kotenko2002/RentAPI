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
    [ApiController, Route("[controller]"), Authorize]
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

        [HttpPost("tenant/add"), Authorize(Roles = Roles.Tenant)]
        public async Task<IActionResult> AddNewResponse(AddResponseModel model)
        {
            var entity = _mapper.Map<Response>(model);
            entity.TenantId = _httpContextAccessor.HttpContext.User.GetUserId();

            await _responseService.AddAsync(entity);

            return Ok("Added successfully!");
        }

        [HttpGet("landlord/items/{propertyId}"), Authorize(Roles = Roles.Landlord)]
        public async Task<IActionResult> GetResponseByPropertyId(int propertyId)
        {
            string landlordId = _httpContextAccessor.HttpContext.User.GetUserId();

            var responses = await _responseService.GetAllResponsesByPropertyIdAsync(propertyId, landlordId);

            return Ok(responses);
        }

        [HttpPatch("landlord/process"), Authorize(Roles = Roles.Landlord)]
        public async Task<IActionResult> Process(ProcessResponseModel model)
        {
            var descriptor = _mapper.Map<ProcessResponseDescriptor>(model);
            descriptor.LandlordId = _httpContextAccessor.HttpContext.User.GetUserId();

            await _responseService.ProcessAsync(descriptor);

            return Ok("Status updated successfully!");
        }
    }
}
