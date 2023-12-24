using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using Rent.Common;
using RentAPI.Infrastructure.Extensions;

namespace Rent.Tests.ExtensionTests
{
    public class FileExtensionsTests
    {
        private Mock<IFormFile> _mockFile;

        [SetUp]
        public void Setup()
        {
            _mockFile = new Mock<IFormFile>();
        }

        [Test]
        public void FileExtensions_IsPhoto_ShouldThrowBusinessException_WhenFileIsNull()
        {
            // Arrange
            IFormFile file = null;

            // Act & Assert
            Assert.Throws<BusinessException>(() => file.IsPhoto());
        }

        [TestCase("image/jpeg")]
        [TestCase("image/png")]
        [TestCase("image/webp")]
        public void FileExtensions_IsPhoto_ShouldReturnTrue(string fileType)
        {
            // Arrange
            _mockFile.Setup(f => f.ContentType).Returns(fileType);

            // Act
            var result = _mockFile.Object.IsPhoto();

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void FileExtensions_IsPhoto_ShouldReturnFalse()
        {
            // Arrange
            _mockFile.Setup(f => f.ContentType).Returns("image/gif");

            // Act
            var result = _mockFile.Object.IsPhoto();

            // Assert
            Assert.IsFalse(result);
        }
    }
}
