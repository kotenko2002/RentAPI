using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rent.Entities.Users;
using Rent.Service.Services.Properties;
using Rent.Service.Services.Properties.Descriptors;
using RentAPI.Infrastructure.Extensions;
using RentAPI.Models.Properties;

namespace RentAPI.Controllers
{
    [ApiController, Route("[controller]"), Authorize(Roles = Roles.Landlord)]
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

        [HttpPost("add")]
        public async Task<IActionResult> AddNewProperty([FromForm] AddPropertyModel model)
        {
            var descriptor = _mapper.Map<AddPropertyDescriptor>(model);
            descriptor.LandlordId = _httpContextAccessor.HttpContext.User.GetUserId();

            await _propertyService.AddAsync(descriptor);

            return Ok("Added successfully!");
        }

        [HttpPatch("edit")]
        public async Task<IActionResult> EditProperty(EditPropertyModel model)
        {
            var descriptor = _mapper.Map<EditPropertyDescriptor>(model);
            string userId = _httpContextAccessor.HttpContext.User.GetUserId();

            await _propertyService.EditAsync(descriptor, userId);

            return Ok("Edited successfully!");
        }

        [HttpDelete("{propertyId}")]
        public async Task<IActionResult> DeleteProperty(int propertyId)
        {
            string userId = _httpContextAccessor.HttpContext.User.GetUserId();
            await _propertyService.DeleteAsync(propertyId, userId);

            return Ok("Deleted successfully!");
        }
    }
}
