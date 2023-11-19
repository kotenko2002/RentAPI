﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rent.Entities.Properties;
using Rent.Entities.Users;
using Rent.Service.Services.Properties;
using Rent.Service.Services.Properties.Descriptors;
using RentAPI.Infrastructure.Extensions;
using RentAPI.Models.Properties;

namespace RentAPI.Controllers
{
    [Authorize(Roles = Roles.Landlord)]
    [ApiController]
    [Route("[controller]")]
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
        public async Task<IActionResult> AddNewProperty(AddPropertyModel model)
        {
            var entity = _mapper.Map<Property>(model);
            entity.UserId = _httpContextAccessor.HttpContext.User.GetUserId();

            await _propertyService.Add(entity);

            return Ok("added successfully!");
        }

        [HttpPatch("edit")]
        public async Task<IActionResult> EditProperty(EditPropertyModel model)
        {
            var descriptor = _mapper.Map<EditPropertyDescriptor>(model);
            string userId = _httpContextAccessor.HttpContext.User.GetUserId();

            await _propertyService.Edit(descriptor, userId);

            return Ok("edited successfully!");
        }

        [HttpDelete("{propertyId}")]
        public async Task<IActionResult> DeleteProperty(int propertyId)
        {
            string userId = _httpContextAccessor.HttpContext.User.GetUserId();
            await _propertyService.Delete(propertyId, userId);

            return Ok();
        }
    }
}