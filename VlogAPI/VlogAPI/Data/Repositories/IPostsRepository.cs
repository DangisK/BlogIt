using VlogAPI.Data.DTOs.Posts;
using VlogAPI.Data.Entities;
using VlogAPI.Helpers;

namespace VlogAPI.Data.Repositories
{
    public interface IPostsRepository
    {
        Task CreateAsync(Post post);
        Task DeleteAsync(Post post);
        Task<IReadOnlyList<Post>> GetManyAsync();
        Task<PagedList<Post>> GetManyAsync(PostSearchParameters parameters);
        Task<Post?> GetPostAsync(int postId);
        Task UpdateAsync(Post post);
    }
}