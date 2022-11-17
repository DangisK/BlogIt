using VlogAPI.Data.Entities;

namespace VlogAPI.Data.Repositories
{
    public interface ILikesRepository
    {
        Task CreateAsync(Like like);
        Task DeleteAsync(Like like);
        Task<Like?> GetLikeAsync(int postId, int commentId, int likeId);
        Task<IReadOnlyList<Like>?> GetManyAsync(int postId, int commentId);
        Task UpdateAsync(Like like);
    }
}