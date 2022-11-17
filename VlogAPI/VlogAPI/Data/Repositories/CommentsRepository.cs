using Microsoft.EntityFrameworkCore;
using VlogAPI.Data.Entities;
using VlogAPI.Helpers;

namespace VlogAPI.Data.Repositories
{
    public class CommentsRepository : ICommentsRepository
    {
        private readonly VlogDbContext _dbContext;
        public CommentsRepository(VlogDbContext vlogDbContext)
        {
            _dbContext = vlogDbContext;
        }

        public async Task<IReadOnlyList<Comment>?> GetManyAsync(int postId)
        {
            var post = await _dbContext.Posts.FirstOrDefaultAsync(post => post.Id == postId);
            if (post == null) return null;

            return await _dbContext.Comments.Where(comment => comment.Post.Id == postId).ToListAsync();
        }

        public async Task<Comment?> GetCommentAsync(int postId, int commentId)
        {
            var comment = await _dbContext.Comments.FirstOrDefaultAsync(comment => comment.Id == commentId && comment.Post.Id == postId);
            if (comment == null) return null;

            return comment;
        }

        public async Task CreateAsync(Comment comment)
        {
            _dbContext.Comments.Add(comment);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Comment comment)
        {
            _dbContext.Comments.Update(comment);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Comment comment)
        {
            _dbContext.Comments.Remove(comment);
            await _dbContext.SaveChangesAsync();
        }

        //public async Task<PagedList<Comment>> GetManyAsync(CommentSearchParameters parameters)
        //{
        //    var queryable = _dbContext.Comments.AsQueryable().OrderBy(comment => comment.CreationDate);

        //    return await PagedList<Comment>.CreateAsync(queryable, parameters.PageNumber, parameters.PageSize);
        //}
    }
}
