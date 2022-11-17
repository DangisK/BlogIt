using VlogAPI.Data.Entities;

namespace VlogAPI.Data.Repositories
{
    public interface ICommentsRepository
    {
        Task CreateAsync(Comment comment);
        Task DeleteAsync(Comment comment);
        Task<Comment?> GetCommentAsync(int postId, int commentId);
        Task<IReadOnlyList<Comment>?> GetManyAsync(int postId);
        Task UpdateAsync(Comment comment);
    }
}