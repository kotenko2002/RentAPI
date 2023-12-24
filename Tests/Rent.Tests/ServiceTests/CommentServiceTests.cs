using AutoMapper;
using Moq;
using NUnit.Framework;
using Rent.Common;
using Rent.Entities.Comments;
using Rent.Entities.Properties;
using Rent.Entities.Responses;
using Rent.Entities.Users;
using Rent.Service.Services.Comments;
using Rent.Service.Services.Comments.Views;
using Rent.Storage.Uow;
using Rent.Tests.Infrastructure;

namespace Rent.Tests.ServiceTests
{
    public class CommentServiceTests : BaseUnitTest
    {
        #region AddAsync
        [Test]
        public async Task CommentService_AddAsync_ShouldAddComment()
        {
            // Arrange
            var comment = new Comment { TenantId = "1", PropertyId = 1, Message = "Message1", Rate = Rate.Average };
            var response = new Response { TenantId = "1", PropertyId = 1, Status = ResponseStatus.ApprovedToRent };
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(uow => uow.ResponseRepository.GetResponseByPropertyAndTenantIdsAsync(comment.PropertyId, comment.TenantId)).ReturnsAsync(response);
            mockUnitOfWork.Setup(uow => uow.CommentRepository.AddAsync(comment)).Returns(Task.CompletedTask);
            mockUnitOfWork.Setup(uow => uow.CompleteAsync()).Returns(Task.CompletedTask);
            var commentService = new CommentService(null, mockUnitOfWork.Object);

            // Act
            await commentService.AddAsync(comment);

            // Assert
            mockUnitOfWork.Verify(uow => uow.CommentRepository.AddAsync(comment), Times.Once);
            mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Once);
        }

        [Test]
        public void CommentService_AddAsync_ShouldThrowBusinessException_WhenResponseNotFoundOrNotApprovedToRent()
        {
            // Arrange
            var comment = new Comment { TenantId = "1", PropertyId = 1, Message = "Message1", Rate = Rate.Average };
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(uow => uow.ResponseRepository.GetResponseByPropertyAndTenantIdsAsync(comment.PropertyId, comment.TenantId)).ReturnsAsync((Response)null);
            var commentService = new CommentService(null, mockUnitOfWork.Object);

            // Act & Assert
            Assert.ThrowsAsync<BusinessException>(() => commentService.AddAsync(comment));
        }
        #endregion

        #region GetCommentsByPropertyIdAsync
        [Test]
        public async Task CommentService_GetCommentsByPropertyIdAsync_ShouldReturnAllComments()
        {
            // Arrange
            var expected = new[]
            {
                new CommentView { Id = 1, UserName = "User1", Message = "Message1", Rate = Rate.Average }
            };
            var property = new Property { Id = 1 };
            var comments = new List<Comment>
            {
                new Comment { Id = 1, Tenant = new User { UserName = "User1" }, Message = "Message1", Rate = Rate.Average }
            };
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(uow => uow.PropertyRepository.FindAsync(1)).ReturnsAsync(property);
            mockUnitOfWork.Setup(uow => uow.CommentRepository.GetFullCommentsByPropertyIdAsync(1)).ReturnsAsync(comments);
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(mapper => mapper.Map<IEnumerable<CommentView>>(comments)).Returns(expected);
            var commentService = new CommentService(mockMapper.Object, mockUnitOfWork.Object);

            // Act
            var actual = await commentService.GetCommentsByPropertyIdAsync(1);

            // Assert
            Assert.IsNotNull(actual);
            Assert.That(actual, Is.EqualTo(expected).Using(new CommentViewEqualityComparer()), message: "GetCommentsByPropertyIdAsync method works incorrect");
        }

        [Test]
        public void CommentService_GetCommentsByPropertyIdAsync_ShouldThrowBusinessException_WhenPropertyNotFound()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(uow => uow.PropertyRepository.FindAsync(1)).ReturnsAsync((Property)null);
            var commentService = new CommentService(null, mockUnitOfWork.Object);

            // Act & Assert
            Assert.ThrowsAsync<BusinessException>(() => commentService.GetCommentsByPropertyIdAsync(1));
        }
        #endregion

        #region DeleteAsync
        [Test]
        public async Task CommentService_DeleteAsync_ShouldDeleteComment()
        {
            // Arrange
            var comment = new Comment { Id = 1, TenantId = "1" };
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(uow => uow.CommentRepository.GetFullCommentByIdAsync(1)).ReturnsAsync(comment);
            mockUnitOfWork.Setup(uow => uow.CommentRepository.RemoveAsync(comment)).Returns(Task.CompletedTask);
            mockUnitOfWork.Setup(uow => uow.CompleteAsync()).Returns(Task.CompletedTask);
            var commentService = new CommentService(null, mockUnitOfWork.Object);

            // Act
            await commentService.DeleteAsync(1, "1");

            // Assert
            mockUnitOfWork.Verify(uow => uow.CommentRepository.RemoveAsync(comment), Times.Once);
            mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Once);
        }

        [Test]
        public void CommentService_DeleteAsync_ShouldThrowBusinessException_WhenCommentNotFound()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(uow => uow.CommentRepository.GetFullCommentByIdAsync(1)).ReturnsAsync((Comment)null);
            var commentService = new CommentService(null, mockUnitOfWork.Object);

            // Act & Assert
            Assert.ThrowsAsync<BusinessException>(() => commentService.DeleteAsync(1, "1"));
        }

        [Test]
        public void CommentService_DeleteAsync_ShouldThrowBusinessException_WhenTenantIdDoesNotMatch()
        {
            // Arrange
            var comment = new Comment { Id = 1, TenantId = "2" };
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(uow => uow.CommentRepository.GetFullCommentByIdAsync(1)).ReturnsAsync(comment);
            var commentService = new CommentService(null, mockUnitOfWork.Object);

            // Act & Assert
            Assert.ThrowsAsync<BusinessException>(() => commentService.DeleteAsync(1, "1"));
        }
        #endregion
    }
}
