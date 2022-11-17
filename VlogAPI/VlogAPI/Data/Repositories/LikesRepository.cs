using Microsoft.EntityFrameworkCore;
using VlogAPI.Data.Entities;

namespace VlogAPI.Data.Repositories
{
    public class LikesRepository : ILikesRepository
    {
        private readonly VlogDbContext _dbContext;
        public LikesRepository(VlogDbContext vlogDbContext)
        {
            _dbContext = vlogDbContext;
        }

        public async Task<IReadOnlyList<Like>?> GetManyAsync(int postId, int commentId)
        {
            var comment = await _dbContext.Comments.FirstOrDefaultAsync(comment => comment.Id == commentId && comment.Post.Id == postId);
            if (comment == null) return null;

            return await _dbContext.Likes.Where(like => like.Comment.Id == commentId).ToListAsync();
        }

        public async Task<Like?> GetLikeAsync(int postId, int commentId, int likeId)
        {
            var like = await _dbContext.Likes.FirstOrDefaultAsync(like => like.Id == likeId && like.Comment.Id == commentId && like.Comment.Post.Id == postId);
            if (like == null) return null;

            return like;
        }

        public async Task CreateAsync(Like like)
        {
            _dbContext.Likes.Add(like);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Like like)
        {
            _dbContext.Likes.Update(like);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Like like)
        {
            _dbContext.Likes.Remove(like);
            await _dbContext.SaveChangesAsync();
        }
    }
}
