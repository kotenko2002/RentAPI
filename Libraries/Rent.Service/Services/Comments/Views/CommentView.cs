using Rent.Entities.Comments;

namespace Rent.Service.Services.Comments.Views
{
    public class CommentView
    {
        public string UserName { get; set; }
        public string Message { get; set; }
        public Rate Rate { get; set; }
    }
}
