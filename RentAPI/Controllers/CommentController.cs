using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rent.Entities.Comments;
using Rent.Entities.Users;
using Rent.Service.Services.Comments;
using Rent.Service.Services.Comments.Views;
using RentAPI.Infrastructure.Extensions;
using RentAPI.Models.Comments;

namespace RentAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICommentService _commentService;

        public CommentController(
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ICommentService commentService)
        {
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _commentService = commentService;
        }

        [Authorize(Roles = Roles.Tenant)]
        [HttpPost("add")]
        public async Task<IActionResult> AddNewProperty(AddCommentModel model)
        {
            var entity = _mapper.Map<Comment>(model);
            entity.TenantId = _httpContextAccessor.HttpContext.User.GetUserId();

            await _commentService.Add(entity);

            return Ok("Added successfully!");
        }

        [Authorize(Roles = Roles.Tenant)]
        [HttpDelete("{commentId}")]
        public async Task<IActionResult> DeleteProperty(int commentId)
        {
            string userId = _httpContextAccessor.HttpContext.User.GetUserId();
            await _commentService.Delete(commentId, userId);

            return Ok("Deleted successfully!");
        }

        [Authorize]
        [HttpGet("items/{propertyId}")]
        public async Task<IActionResult> GetCommentsBy(int propertyId)
        {
            IEnumerable<CommentView> comments = await _commentService.GetCommentsByPropertyId(propertyId);

            return Ok(comments);
        }
    }
}
