using NUnit.Framework;
using Rent.Entities.Comments;
using Rent.Storage.Configuration;
using Rent.Storage.Repositories.Comments;

namespace Rent.Tests.StorageTests
{
    public class CommentRepositoryTests
    {
        #region BasicMethods
        [TestCase(1)]
        [TestCase(2)]
        public async Task CommentRepository_FindAsync_ReturnsSingleValue(int id)
        {
            using var context = new ApplicationDbContext(UnitTestHelper.GetUnitTestDbOptions());
            var commentRepository = new CommentRepository(context);

            var actual = await commentRepository.FindAsync(id);
            var expected = ExpectedComments.FirstOrDefault(x => x.Id == id);

            Assert.That(actual,
                Is.EqualTo(expected).Using(new CommentEqualityComparer()), message: "FindAsync method works incorrect");
        }

        [Test]
        public async Task CommentRepository_GetAllAsync_ReturnsAllValues()
        {
            using var context = new ApplicationDbContext(UnitTestHelper.GetUnitTestDbOptions());
            var commentRepository = new CommentRepository(context);

            var actual = await commentRepository.FindAllAsync();

            Assert.That(actual,
                Is.EqualTo(ExpectedComments).Using(new CommentEqualityComparer()), message: "GetAllAsync method works incorrect");
        }

        [Test]
        public async Task CommentRepository_AddAsync_AddsValueToDatabase()
        {
            using var context = new ApplicationDbContext(UnitTestHelper.GetUnitTestDbOptions());
            var commentRepository = new CommentRepository(context);

            var comment = new Comment { Id = 3, TenantId = "2", PropertyId = 1, Message = "Message1", Rate = Rate.Excellent };
            await commentRepository.AddAsync(comment);
            await context.SaveChangesAsync();

            Assert.That(context.Comments.Count(),
                Is.EqualTo(3), message: "AddAsync method works incorrect");
        }

        [Test]
        public async Task CommentRepository_AddRangeAsync_AddsValueToDatabase()
        {
            using var context = new ApplicationDbContext(UnitTestHelper.GetUnitTestDbOptions());
            var commentRepository = new CommentRepository(context);

            var comments = new[]
            {
                new Comment { Id = 3, TenantId = "2", PropertyId = 1, Message = "Message1", Rate = Rate.Poor },
                new Comment { Id = 4, TenantId = "2", PropertyId = 3, Message = "Message1", Rate = Rate.Excellent }
            };
            await commentRepository.AddRangeAsync(comments);
            await context.SaveChangesAsync();

            Assert.That(context.Comments.Count(),
                Is.EqualTo(4), message: "AddRangeAsync method works incorrect");
        }

        [Test]
        public async Task CommentRepository_RemoveAsync_RemovesValueFromDatabase()
        {
            using var context = new ApplicationDbContext(UnitTestHelper.GetUnitTestDbOptions());
            var commentRepository = new CommentRepository(context);

            var comment = new Comment { Id = 1, TenantId = "2", PropertyId = 1, Message = "Message1", Rate = Rate.Average };
            await commentRepository.RemoveAsync(comment);
            await context.SaveChangesAsync();

            Assert.That(context.Comments.Count(),
                Is.EqualTo(1), message: "RemoveAsync method works incorrect");
        }

        [Test]
        public async Task CommentRepository_RemoveRangeAsync_RemovesValuesFromDatabase()
        {
            using var context = new ApplicationDbContext(UnitTestHelper.GetUnitTestDbOptions());
            var commentRepository = new CommentRepository(context);

            var comments = new[]
            {
                new Comment { Id = 1, TenantId = "2", PropertyId = 1, Message = "Message1", Rate = Rate.Average },
                new Comment { Id = 2, TenantId = "2", PropertyId = 3, Message = "Message2", Rate = Rate.Average }
            };
            await commentRepository.RemoveRangeAsync(comments);
            await context.SaveChangesAsync();

            Assert.That(context.Comments.Count(),
                Is.EqualTo(0), message: "RemoveRangeAsync method works incorrect");
        }
        #endregion

        private static IEnumerable<Comment> ExpectedComments =>
            new[]
            {
                 new Comment { Id = 1, TenantId = "2", PropertyId = 1, Message = "Message1", Rate = Rate.Average },
                 new Comment { Id = 2, TenantId = "2", PropertyId = 3, Message = "Message2", Rate = Rate.Average }
            };
    }
}
