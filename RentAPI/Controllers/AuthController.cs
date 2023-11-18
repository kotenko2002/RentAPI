using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rent.Service.Services.Authorization;
using Rent.Service.Services.Authorization.Descriptors;
using Rent.Service.Services.Authorization.Views;
using RentAPI.Infrastructure.Extensions;
using RentAPI.Models.Auth;

namespace RentAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthService _authService;

        public AuthController(
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            IAuthService authService)
        {
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
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

        [HttpPost]
        [Route("refresh-tokens")]
        public async Task<IActionResult> RefreshTokens(RefreshTokensModel model)
        {
            var descriptor = _mapper.Map<RefreshTokensDescriptor>(model);
            TokensPairView newTokens = await _authService.RefreshTokens(descriptor);

            return Ok(newTokens);
        }

        [HttpDelete]
        [Route("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            string username = _httpContextAccessor.HttpContext.User.GetUsername();
            await _authService.Logout(username);

            return Ok();
        }
    }
}
