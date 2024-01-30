using Moq;
using NUnit.Framework;
using Rent.Entities.Responses;
using Rent.Service.Services.Responses.Descriptors;
using Rent.Service.Services.Responses;
using Rent.Tests.Infrastructure;
using Rent.Entities.Properties;
using Rent.Entities.Users;
using Rent.Common;
using Rent.Storage.Uow;
using Rent.Service.Services.Responses.Views;

namespace Rent.Tests.ServiceTests
{
    public class ResponseServiceTests : BaseUnitTest
    {
        #region AddAsync
        [Test]
        public async Task ResponseService_AddAsync_ShouldAddResponse()
        {
            // Arrange
            var response = new Response { TenantId = "1", PropertyId = 1, Message = "Message1" };

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(uow => uow.ResponseRepository.AddAsync(response)).Returns(Task.CompletedTask);
            mockUnitOfWork.Setup(uow => uow.CompleteAsync()).Returns(Task.CompletedTask);

            var responseService = new ResponseService(mockUnitOfWork.Object);

            // Act
            await responseService.AddAsync(response);

            // Assert
            mockUnitOfWork.Verify(uow => uow.ResponseRepository.AddAsync(response), Times.Once);
            mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Once);
        }
        #endregion

        #region GetAllResponsesByPropertyIdAsync
        [Test]
        public async Task ResponseService_GetAllResponsesByPropertyIdAsync_ShouldReturnAllResponses()
        {
            // Arrange
            var expected = new[]
            {
                new ResponseView()
                {
                    Id = 1,
                    Email = "Email1",
                    PhoneNumber = "Phone1",
                    Message = "Message1",
                    Status = ResponseStatus.NotReviewed
                }
            };

            var property = new Property {
                Id = 1,
                LandlordId = "1",
                Responses = new List<Response>
                {
                    new Response
                    {
                        Id = 1,
                        Tenant = new User
                        {
                            Email = "Email1",
                            PhoneNumber = "Phone1"
                        },
                        Message = "Message1",
                        Status = ResponseStatus.NotReviewed
                    }
                }
            };
            
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(uow => uow.PropertyRepository.GetFullPropertyByIdAsync(1)).ReturnsAsync(property);

            var responseService = new ResponseService(mockUnitOfWork.Object);

            // Act
            var actual = await responseService.GetAllResponsesByPropertyIdAsync(1, "1");

            // Assert
            Assert.IsNotNull(actual);
            Assert.That(actual,
               Is.EqualTo(expected).Using(new ResponseViewEqualityComparer()), message: "GetAllResponsesByPropertyIdAsync method works incorrect");
        }

        [Test]
        public void ResponseService_GetAllResponsesByPropertyIdAsync_ShouldThrowBusinessException_WhenPropertyNotFound()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(uow => uow.PropertyRepository.GetFullPropertyByIdAsync(1)).ReturnsAsync((Property)null);

            var responseService = new ResponseService(mockUnitOfWork.Object);

            // Act & Assert
            Assert.ThrowsAsync<BusinessException>(() => responseService.GetAllResponsesByPropertyIdAsync(1, "1"));
        }

        [Test]
        public void ResponseService_GetAllResponsesByPropertyIdAsync_ShouldThrowBusinessException_WhenLandlordIdDoesNotMatch()
        {
            // Arrange
            var property = new Property
            {
                Id = 1,
                LandlordId = "2"
            };

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(uow => uow.PropertyRepository.GetFullPropertyByIdAsync(1)).ReturnsAsync(property);

            var responseService = new ResponseService(mockUnitOfWork.Object);

            // Act & Assert
            Assert.ThrowsAsync<BusinessException>(() => responseService.GetAllResponsesByPropertyIdAsync(1, "1"));
        }
        #endregion

        #region ProcessAsync
        [Test]
        public async Task ResponseService_ProcessAsync_ShouldProcessResponse()
        {
            // Arrange
            var descriptor = new ProcessResponseDescriptor
            {
                ResponseId = 1,
                LandlordId = "1",
                Status = ResponseStatus.ApprovedToRent
            };
            var response = new Response
            {
                Id = 1,
                Property = new Property
                {
                    LandlordId = "1"
                }
            };

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(uow => uow.ResponseRepository.GetFullResponseByIdAsync(1)).ReturnsAsync(response);
            mockUnitOfWork.Setup(uow => uow.CompleteAsync()).Returns(Task.CompletedTask);

            var responseService = new ResponseService(mockUnitOfWork.Object);

            // Act
            await responseService.ProcessAsync(descriptor);

            // Assert
            Assert.That(response.Status, Is.EqualTo(ResponseStatus.ApprovedToRent));
            mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Once);
        }

        [Test]
        public void ResponseService_ProcessAsync_ShouldThrowBusinessException_WhenLandlordIdDoesNotMatch()
        {
            // Arrange
            var descriptor = new ProcessResponseDescriptor
            {
                ResponseId = 1,
                LandlordId = "2",
                Status = ResponseStatus.ApprovedToRent
            };
            var response = new Response
            {
                Id = 1,
                Property = new Property
                {
                    LandlordId = "1"
                }
            };

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(uow => uow.ResponseRepository.GetFullResponseByIdAsync(1)).ReturnsAsync(response);

            var responseService = new ResponseService(mockUnitOfWork.Object);

            // Act & Assert
            Assert.ThrowsAsync<BusinessException>(() => responseService.ProcessAsync(descriptor));
        }
        #endregion
    }
}
