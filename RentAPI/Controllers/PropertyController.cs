using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rent.Entities.Users;
using Rent.Service.Services.Properties;
using Rent.Service.Services.Properties.Descriptors;
using Rent.Service.Services.Properties.Views;
using RentAPI.Infrastructure.Extensions;
using RentAPI.Models.Properties;

namespace RentAPI.Controllers
{
    [ApiController, Route("[controller]"), Authorize]
    public class PropertyController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPropertyService _propertyService;

        public PropertyController(
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            IPropertyService propertyService )
        {
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _propertyService = propertyService;
        }

        [HttpPost("landlord"), Authorize(Roles = Roles.Landlord)]
        public async Task<IActionResult> AddNewProperty([FromForm] AddPropertyModel model)
        {
            if (model.Photos.Any(photo => !photo.IsPhoto()))
            {
                return BadRequest("An unsupported file type was detected");
            }

            var descriptor = _mapper.Map<AddPropertyDescriptor>(model);
            descriptor.LandlordId = _httpContextAccessor.HttpContext.User.GetUserId();

            await _propertyService.AddAsync(descriptor);

            return Ok("Added successfully!");
        }

        [HttpPatch("landlord"), Authorize(Roles = Roles.Landlord)]
        public async Task<IActionResult> EditProperty([FromForm] EditPropertyModel model)
        {
            var descriptor = _mapper.Map<EditPropertyDescriptor>(model);
            string userId = _httpContextAccessor.HttpContext.User.GetUserId();

            await _propertyService.EditAsync(descriptor, userId);

            return Ok("Edited successfully!");
        }

        [HttpGet("tenant/items/{cityId}"), Authorize(Roles = Roles.Tenant)]
        public async Task<IActionResult> GetPropertiesByCityId(int cityId)
        {
            IEnumerable<PropertyView> views = await _propertyService.GetPropertiesByCityIdAsync(cityId);

            return Ok(views);
        }

        [HttpGet("landlord/items"), Authorize(Roles = Roles.Landlord)]
        public async Task<IActionResult> GetPropertiesByLandlordId()
        {
            string userId = _httpContextAccessor.HttpContext.User.GetUserId();
            IEnumerable<PropertyView> views = await _propertyService.GetPropertiesByLandlordIdAsync(userId);

            return Ok(views);
        }

        [HttpGet("item/{propertyId}")]
        public async Task<IActionResult> GetPropertyFullInfoById(int propertyId)
        {
            return Ok(await _propertyService.GetFullInfoByIdAsync(propertyId));
        }

        [HttpDelete("landlord/{propertyId}"), Authorize(Roles = Roles.Landlord)]
        public async Task<IActionResult> DeleteProperty(int propertyId)
        {
            string userId = _httpContextAccessor.HttpContext.User.GetUserId();
            await _propertyService.DeleteAsync(propertyId, userId);

            return Ok("Deleted successfully!");
        }
    }
}
