namespace VlogAPI.Data.DTOs.Posts
{
    public class PostSearchParameters
    {
        // api/posts
        private int pageSize = 2;
        private const int MaxPageSize = 50;

        public int PageNumber { get; set; } = 1;

        public int PageSize
        {
            get => pageSize;
            set => pageSize = value > MaxPageSize ? MaxPageSize : value;
        }
    }
}
