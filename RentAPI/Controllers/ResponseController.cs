using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rent.Entities.Responses;
using Rent.Entities.Users;
using Rent.Service.Services.Responses;
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
        [HttpPost]
        public async Task<IActionResult> AddNewResponse(AddResponseModel model)
        {
            var entity = _mapper.Map<Response>(model);
            entity.UserId = _httpContextAccessor.HttpContext.User.GetUserId();

            await _responseService.Add(entity);

            return Ok();
        }

        [Authorize(Roles = Roles.Landlord)]
        [HttpGet("{propertyId}")]
        public async Task<IActionResult> GetResponseByPropertyId(int propertyId)
        {
            string landlordId = _httpContextAccessor.HttpContext.User.GetUserId();

            await _responseService.GetAllResponsesByPropertyId(propertyId, landlordId);

            return Ok();
        }
    }
}
