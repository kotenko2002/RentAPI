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
    [ApiController, Route("[controller]"), Authorize]
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

        [HttpPost("add"), Authorize(Roles = Roles.Tenant)]
        public async Task<IActionResult> AddNewProperty(AddCommentModel model)
        {
            var entity = _mapper.Map<Comment>(model);
            entity.TenantId = _httpContextAccessor.HttpContext.User.GetUserId();

            await _commentService.AddAsync(entity);

            return Ok("Added successfully!");
        }

        [HttpDelete("{commentId}"), Authorize(Roles = Roles.Tenant)]
        public async Task<IActionResult> DeleteProperty(int commentId)
        {
            string userId = _httpContextAccessor.HttpContext.User.GetUserId();
            await _commentService.DeleteAsync(commentId, userId);

            return Ok("Deleted successfully!");
        }

        [HttpGet("items/{propertyId}")]
        public async Task<IActionResult> GetCommentsBy(int propertyId)
        {
            IEnumerable<CommentView> comments = await _commentService.GetCommentsByPropertyIdAsync(propertyId);

            return Ok(comments);
        }
    }
}
