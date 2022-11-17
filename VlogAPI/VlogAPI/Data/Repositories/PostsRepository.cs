using Microsoft.EntityFrameworkCore;
using VlogAPI.Data.DTOs.Posts;
using VlogAPI.Data.Entities;
using VlogAPI.Helpers;

namespace VlogAPI.Data.Repositories
{
    public class PostsRepository : IPostsRepository
    {
        private readonly VlogDbContext _dbContext;
        public PostsRepository(VlogDbContext vlogDbContext)
        {
            _dbContext = vlogDbContext;
        }

        public async Task<Post?> GetPostAsync(int postId)
        {
            return await _dbContext.Posts.FirstOrDefaultAsync(post => post.Id == postId);
        }

        public async Task<IReadOnlyList<Post>> GetManyAsync()
        {
            return await _dbContext.Posts.ToListAsync();
        }

        public async Task<PagedList<Post>> GetManyAsync(PostSearchParameters parameters)
        {
            var queryable = _dbContext.Posts.AsQueryable().OrderBy(post => post.CreationDate);

            return await PagedList<Post>.CreateAsync(queryable, parameters.PageNumber, parameters.PageSize);
        }

        public async Task CreateAsync(Post post)
        {
            _dbContext.Posts.Add(post);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Post post)
        {
            _dbContext.Posts.Update(post);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Post post)
        {
            _dbContext.Posts.Remove(post);
            await _dbContext.SaveChangesAsync();
        }
    }
}
