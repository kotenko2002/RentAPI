using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rent.Service.Services.Authorization;
using Rent.Service.Services.Authorization.Descriptors;
using Rent.Service.Services.Authorization.Views;
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
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var descriptor = _mapper.Map<RegisterDescriptor>(model);

            await _authService.Register(descriptor);

            return Ok("User created successfully!");
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var descriptor = _mapper.Map<LoginDescriptor>(model);

            TokensPairView tokens = await _authService.Login(descriptor);

            return Ok(tokens);
        }

        [HttpGet]
        [Authorize]
        public void Test()
        {
            Console.WriteLine("HI");
        }
    }
}
