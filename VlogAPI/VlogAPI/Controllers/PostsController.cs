using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;
using VlogAPI.Auth.Model;
using VlogAPI.Data.DTOs;
using VlogAPI.Data.Entities;
using VlogAPI.Data.Repositories;
using static VlogAPI.Data.DTOs.LikesDTO;

namespace VlogAPI.Controllers
{
    /*    
    /api/v1/posts GET List 200
    /api/v1/posts/{id} GET One 200
    /api/v1/posts POST Create 201
    /api/v1/posts/{id} PUT/PATCH Modify 200
    /api/v1/posts/{id} DELETE Remove 200/204
    */
    [ApiController]
    [Route("api/posts")]
    public class PostsController : ControllerBase
    {
        private readonly ICommentsRepository commentsRepository;
        private readonly IPostsRepository postsRepository;
        private readonly ILikesRepository likesRepository;
        private readonly IMapper mapper;
        private readonly IAuthorizationService _authorizationService;

        public PostsController(IPostsRepository postsRepository, IMapper mapper, ICommentsRepository commentsRepository, ILikesRepository likesRepository, IAuthorizationService _authorizationService)
        {
            this.postsRepository = postsRepository;
            this.commentsRepository = commentsRepository;
            this.likesRepository = likesRepository;
            this.mapper = mapper;
            this._authorizationService = _authorizationService;
        }

        [HttpGet]
        public async Task<IEnumerable<PostDTO>> GetMany()
        {
            var posts = await postsRepository.GetManyAsync();

            return posts.Select(post => mapper.Map<PostDTO>(post));
        }

        // api/posts/{postId}
        [HttpGet("{postId}", Name = "GetPost")]
        public async Task<IActionResult> GetOne(int postId)
        {
            var post = await postsRepository.GetPostAsync(postId);

            // 404
            if (post == null) return NotFound();

            //var links = CreateLinksForPost(postId);

            var postDTO = mapper.Map<PostDTO>(post);

            return Ok(postDTO);

            //return Ok(new { Resource = postDTO, Links = links });

        }

        // api/posts/
        [HttpPost]
        [Authorize(Roles = VlogRoles.VlogUser)]
        public async Task<ActionResult<PostDTO>> Create(CreatePostDTO createPostDTO)
        {
            var post = new Post { Name = createPostDTO.name, Body = createPostDTO.body, CreationDate = DateTime.UtcNow, UserId = User.FindFirstValue(JwtRegisteredClaimNames.Sub)};
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, post, PolicyNames.ResourceOwner);
            if (!authorizationResult.Succeeded)
            {
                // 404
                return Forbid();
            }
            await postsRepository.CreateAsync(post);

            // 201
            return Created("$/api/posts/{post.Id}", mapper.Map<PostDTO>(post));
        }

        // api/posts/
        [HttpPut("{postId}")]
        [Authorize(Roles = VlogRoles.VlogUser)]
        public async Task<ActionResult<PostDTO>> Update(int postId, UpdatePostDTO updatePostDTO)
        {
            var post = await postsRepository.GetPostAsync(postId);

            // 404
            if (post == null) return NotFound();

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, post, PolicyNames.ResourceOwner);
            if (!authorizationResult.Succeeded)
            {
                // 404
                return Forbid();
            }

            //post.Body = updatePostDTO.body;
            mapper.Map(updatePostDTO, post);

            await postsRepository.UpdateAsync(post);

            return Ok(mapper.Map<PostDTO>(post));
        }

        // api/posts/
        [HttpDelete("{postId}", Name = "DeletePost")]
        [Authorize(Roles = VlogRoles.VlogUser)]
        public async Task<ActionResult> Remove(int postId)
        {
            var post = await postsRepository.GetPostAsync(postId);

            // 404
            if (post == null) return NotFound();

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, post, PolicyNames.ResourceOwner);
            if (!authorizationResult.Succeeded)
            {
                // 404
                return Forbid();
            }

            await postsRepository.DeleteAsync(post);

            // 204
            return NoContent();
        }

        [HttpGet("{postId}/likes")]
        public async Task<ActionResult<IEnumerable<LikeDTO>>> GetPostLikes(int postId)
        {
            var post = await postsRepository.GetPostAsync(postId);
            if (post == null) return NotFound();

            List<LikeDTO> postLikes = new List<LikeDTO>();

            var comments = await commentsRepository.GetManyAsync(postId);
            if (comments.Count == 0) return Ok(postLikes);


            foreach (Comment comment in comments)
            {
                var likes = await likesRepository.GetManyAsync(postId, comment.Id);
                if (likes.Count != 0)
                {
                    foreach (Like like in likes)
                    {
                        postLikes.Add(new LikeDTO(like.Id, like.IsPositive, like.CreationDate));
                    }
                }
            }
            return Ok(postLikes);
        }

        //private IEnumerable<LinkDTO> CreateLinksForPost(int postId)
        //{
        //    yield return new LinkDTO { Href = Url.Link("GetPost", new { postId }), Rel = "self", Method = "GET" };
        //    yield return new LinkDTO { Href = Url.Link("DeletePost", new { postId }), Rel = "delete_post", Method = "DELETE" };
        //}

        //private string? CreatePostsResourceUri(
        //    PostSearchParameters topicSearchParametersDto,
        //    ResourceUriType type)
        //{
        //    return type switch
        //    {
        //        ResourceUriType.PreviousPage => Url.Link("GetPosts",
        //            new
        //            {
        //                pageNumber = topicSearchParametersDto.PageNumber - 1,
        //                pageSize = topicSearchParametersDto.PageSize,
        //            }),
        //        ResourceUriType.NextPage => Url.Link("GetPosts",
        //            new
        //            {
        //                pageNumber = topicSearchParametersDto.PageNumber + 1,
        //                pageSize = topicSearchParametersDto.PageSize,
        //            }),
        //        _ => Url.Link("GetPosts",
        //            new
        //            {
        //                pageNumber = topicSearchParametersDto.PageNumber,
        //                pageSize = topicSearchParametersDto.PageSize,
        //            })
        //    };
        //}

        // posts?pageNumber=1&pageSize=5
        //[HttpGet(Name = "GetPosts")]
        //public async Task<IEnumerable<PostDTO>> GetManyPaging([FromQuery] PostSearchParameters parameters)
        //{
        //    var posts = await postsRepository.GetManyAsync(parameters);

        //    var previousPageLink = posts.HasPrevious ?
        //    CreatePostsResourceUri(parameters,
        //        ResourceUriType.PreviousPage) : null;

        //    var nextPageLink = posts.HasNext ?
        //        CreatePostsResourceUri(parameters,
        //            ResourceUriType.NextPage) : null;

        //    var paginationMetadata = new
        //    {
        //        totalCount = posts.TotalCount,
        //        pageSize = posts.PageSize,
        //        currentPage = posts.CurrentPage,
        //        totalPages = posts.TotalPages,
        //        previousPageLink,
        //        nextPageLink
        //    };

        //    { "resource": { }, "paging":{ } }

        //    Response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationMetadata));

        //    return posts.Select(post => new PostDTO(post.Id, post.Name, post.Body, post.CreationDate));
        //}
    }
}
