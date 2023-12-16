using NUnit.Framework;
using Rent.Entities.Photos;
using Rent.Storage.Configuration;
using Rent.Storage.Repositories.Photos;

namespace Rent.Tests.StorageTests
{
    public class PhotoRepositoryTests
    {
        #region BasicMethods
        [Test]
        public async Task PhotoRepository_GetAllAsync_ReturnsAllValues()
        {
            using var context = new ApplicationDbContext(UnitTestHelper.GetUnitTestDbOptions());
            var photoRepository = new PhotoRepository(context);

            var actual = await photoRepository.FindAllAsync();

            Assert.That(actual,
                Is.EqualTo(ExpectedPhotos).Using(new PhotoEqualityComparer()), message: "GetAllAsync method works incorrect");
        }

        [Test]
        public async Task PhotoRepository_AddAsync_AddsValueToDatabase()
        {
            using var context = new ApplicationDbContext(UnitTestHelper.GetUnitTestDbOptions());
            var photoRepository = new PhotoRepository(context);

            var photo = new Photo { Id = "6", PropertyId = 2 };
            await photoRepository.AddAsync(photo);
            await context.SaveChangesAsync();

            Assert.That(context.Photos.Count(),
                Is.EqualTo(6), message: "AddAsync method works incorrect");
        }

        [Test]
        public async Task PhotoRepository_AddRangeAsync_AddsValueToDatabase()
        {
            using var context = new ApplicationDbContext(UnitTestHelper.GetUnitTestDbOptions());
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
        public async Task PhotoRepository_RemoveAsync_RemovesValueFromDatabase()
        {
            using var context = new ApplicationDbContext(UnitTestHelper.GetUnitTestDbOptions());
            var photoRepository = new PhotoRepository(context);

            var photo = new Photo { Id = "1", PropertyId = 1 };
            await photoRepository.RemoveAsync(photo);
            await context.SaveChangesAsync();

            Assert.That(context.Photos.Count(),
                Is.EqualTo(4), message: "RemoveAsync method works incorrect");
        }

        [Test]
        public async Task PhotoRepository_RemoveRangeAsync_RemovesValuesFromDatabase()
        {
            using var context = new ApplicationDbContext(UnitTestHelper.GetUnitTestDbOptions());
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
