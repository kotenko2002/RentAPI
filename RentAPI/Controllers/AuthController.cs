using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Rent.Service.Authorization;
using Rent.Service.Authorization.Descriptors;
using RentAPI.Models.Auth;

namespace RentAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IAuthService _authService;

        public AuthController(
            IMapper mapper,
            IAuthService authService)
        {
            _mapper = mapper;
            _authService = authService;
        }

        [HttpPost]
        public void TestEndpoint(LoginModel model)
        {
            var descriptor = _mapper.Map<LoginDescriptor>(model);

            _authService.Test(descriptor);
        }
    }
}
