using FluentAssertions;
using Moq;
using NUnit.Framework;
using Rent.Entities.Comments;
using Rent.Entities.Properties;
using Rent.Entities.Responses;
using Rent.Service.Services.Comments;
using Rent.Storage.Uow;

namespace Rent.Tests.ServiceTests
{
    public class CommentServiceTests
    {
        [Test]
        public async Task CommentService_AddAsync_AddsComment()
        {
            // Arrange
            var comment = new Comment { PropertyId = 1, TenantId = "1" };
            var response = new Response { PropertyId = 1, TenantId = "1", Status = ResponseStatus.ApprovedToRent };

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(x => x.ResponseRepository.GetResponseByPropertyAndTenantIdsAsync(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(response);
            mockUnitOfWork.Setup(x => x.CommentRepository.AddAsync(It.IsAny<Comment>()));

            var commentService = new CommentService(UnitTestHelper.CreateMapperProfile(), mockUnitOfWork.Object);

            // Act
            await commentService.AddAsync(comment);

            // Assert
            mockUnitOfWork.Verify(x => x.CommentRepository.AddAsync(It.Is<Comment>(c => c.PropertyId == comment.PropertyId && c.TenantId == comment.TenantId)), Times.Once);
            mockUnitOfWork.Verify(x => x.CompleteAsync(), Times.Once);
        }

        [Test]
        public async Task CommentService_DeleteAsync_DeletesComment()
        {
            // Arrange
            var comment = new Comment { Id = 1, TenantId = "1" };

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(x => x.CommentRepository.GetFullCommentByIdAsync(It.IsAny<int>())).ReturnsAsync(comment);
            mockUnitOfWork.Setup(x => x.CommentRepository.RemoveAsync(It.IsAny<Comment>()));

            var commentService = new CommentService(UnitTestHelper.CreateMapperProfile(), mockUnitOfWork.Object);

            // Act
            await commentService.DeleteAsync(comment.Id, comment.TenantId);

            // Assert
            mockUnitOfWork.Verify(x => x.CommentRepository.RemoveAsync(It.Is<Comment>(c => c.Id == comment.Id && c.TenantId == comment.TenantId)), Times.Once);
            mockUnitOfWork.Verify(x => x.CompleteAsync(), Times.Once);
        }

        [Test]
        public async Task CommentService_GetCommentsByPropertyIdAsync_ReturnsComments()
        {
            // Arrange
            var property = new Property()
            {
                Id = 1,
                LandlordId = "1",
                CityId = 1,
                Address = "Address1",
                Description = "Description1",
                Price = 1000,
                Status = PropertyStatus.Available
            };
            var comments = new List<Comment>
            {
                new Comment { PropertyId = 1, TenantId = "1", Message = "Message1", Rate = Rate.Average },
                new Comment { PropertyId = 1, TenantId = "2", Message = "Message2", Rate = Rate.Good }
            };

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(x => x.PropertyRepository.FindAsync(It.IsAny<int>())).ReturnsAsync(property);
            mockUnitOfWork.Setup(x => x.CommentRepository.GetFullCommentsByPropertyIdAsync(It.IsAny<int>())).ReturnsAsync(comments);

            var commentService = new CommentService(UnitTestHelper.CreateMapperProfile(), mockUnitOfWork.Object);

            // Act
            var actual = await commentService.GetCommentsByPropertyIdAsync(1);

            // Assert
            actual.Should().BeEquivalentTo(comments, options => options.ExcludingMissingMembers());
        }

    }
}
