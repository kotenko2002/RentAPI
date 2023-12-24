using NUnit.Framework;
using Rent.Entities.Photos;
using Rent.Storage.Configuration;
using Rent.Storage.Repositories.Photos;
using Rent.Tests.Infrastructure;

namespace Rent.Tests.StorageTests
{
    public class PhotoRepositoryTests : BaseUnitTest
    {
        #region BasicMethods
        [Test]
        public async Task PhotoRepository_GetAllAsync_ShouldReturnAllValues()
        {
            using var context = new ApplicationDbContext(GetUnitTestDbOptions());
            var photoRepository = new PhotoRepository(context);

            var actual = await photoRepository.FindAllAsync();

            Assert.That(actual,
                Is.EqualTo(ExpectedPhotos).Using(new PhotoEqualityComparer()), message: "GetAllAsync method works incorrect");
        }

        [Test]
        public async Task PhotoRepository_AddAsync_ShouldAddValueToDatabase()
        {
            using var context = new ApplicationDbContext(GetUnitTestDbOptions());
            var photoRepository = new PhotoRepository(context);

            var photo = new Photo { Id = "6", PropertyId = 2 };
            await photoRepository.AddAsync(photo);
            await context.SaveChangesAsync();

            Assert.That(context.Photos.Count(),
                Is.EqualTo(6), message: "AddAsync method works incorrect");
        }

        [Test]
        public async Task PhotoRepository_AddRangeAsync_ShouldAddValueToDatabase()
        {
            using var context = new ApplicationDbContext(GetUnitTestDbOptions());
            var photoRepository = new PhotoRepository(context);

            var photos = new[]
            {
                new Photo { Id = "6", PropertyId = 2 },
                new Photo { Id = "7", PropertyId = 3 }
            };
            await photoRepository.AddRangeAsync(photos);
            await context.SaveChangesAsync();

            Assert.That(context.Photos.Count(),
                Is.EqualTo(7), message: "AddRangeAsync method works incorrect");
        }

        [Test]
        public async Task PhotoRepository_RemoveAsync_ShouldRemoveValueFromDatabase()
        {
            using var context = new ApplicationDbContext(GetUnitTestDbOptions());
            var photoRepository = new PhotoRepository(context);

            var photo = new Photo { Id = "1", PropertyId = 1 };
            await photoRepository.RemoveAsync(photo);
            await context.SaveChangesAsync();

            Assert.That(context.Photos.Count(),
                Is.EqualTo(4), message: "RemoveAsync method works incorrect");
        }

        [Test]
        public async Task PhotoRepository_RemoveRangeAsync_ShouldRemoveValuesFromDatabase()
        {
            using var context = new ApplicationDbContext(GetUnitTestDbOptions());
            var photoRepository = new PhotoRepository(context);

            var photos = new[]
            {
                new Photo { Id = "1", PropertyId = 1 },
                new Photo { Id = "2", PropertyId = 1 }
            };
            await photoRepository.RemoveRangeAsync(photos);
            await context.SaveChangesAsync();

            Assert.That(context.Photos.Count(),
                Is.EqualTo(3), message: "RemoveRangeAsync method works incorrect");
        }
        #endregion

        [Test]
        public async Task PhotoRepository_GetPhotosByIds_ShouldReturnCorrectPhotos()
        {
            using var context = new ApplicationDbContext(GetUnitTestDbOptions());
            var photoRepository = new PhotoRepository(context);

            var ids = new[] { "1", "2" };
            var actual = await photoRepository.GetPhotosByIds(ids);

            var expected = ExpectedPhotos.Where(p => ids.Contains(p.Id));

            Assert.That(actual,
                Is.EqualTo(expected).Using(new PhotoEqualityComparer()), message: "GetPhotosByIds method works incorrect");
        }

        private static IEnumerable<Photo> ExpectedPhotos =>
            new[]
            {
                new Photo { Id = "1", PropertyId = 1 },
                new Photo { Id = "2", PropertyId = 1 },
                new Photo { Id = "3", PropertyId = 2 },
                new Photo { Id = "4", PropertyId = 2 },
                new Photo { Id = "5", PropertyId = 3 }
            };
    }
}
