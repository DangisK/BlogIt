using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
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
        private readonly UserManager<VlogUser> _userManager;

        public PostsController(IPostsRepository postsRepository, IMapper mapper, ICommentsRepository commentsRepository, ILikesRepository likesRepository, IAuthorizationService _authorizationService, UserManager<VlogUser> userManager)
        {
            this.postsRepository = postsRepository;
            this.commentsRepository = commentsRepository;
            this.likesRepository = likesRepository;
            this.mapper = mapper;
            this._authorizationService = _authorizationService;
            this._userManager = userManager;
        }

        [HttpGet]
        [Authorize(Roles = VlogRoles.VlogUser)]
        public async Task<ActionResult<IEnumerable<Post>>> GetMany()
        {
            var posts = await postsRepository.GetManyAsync();

            var postsDTOs = posts.Select(post => new PostDTO(post.Id, post.Name, post.Body, post.CreationDate, _userManager.Users.FirstOrDefault(user => user.Id == post.UserId)?.NormalizedUserName, _userManager.Users.FirstOrDefault(user => user.Id == post.UserId)?.Id));

            return Ok(postsDTOs);
            //return Ok(posts.Select(post => mapper.Map<PostDTO>(post)));
        }

        // api/posts/{postId}
        [HttpGet("{postId}", Name = "GetPost")]
        [Authorize(Roles = VlogRoles.VlogUser)]
        public async Task<IActionResult> GetOne(int postId)
        {
            var post = await postsRepository.GetPostAsync(postId);
            if (post == null) return NotFound();

            var user = _userManager.Users.FirstOrDefault(user => user.Id == User.FindFirstValue(JwtRegisteredClaimNames.Sub));
            var normalizedUsername = _userManager.Users.FirstOrDefault(user => user.Id == post.UserId)?.NormalizedUserName;
            var userId = _userManager.Users.FirstOrDefault(user => user.Id == post.UserId)?.Id;

            var postDTO = new PostDTO(post.Id, post.Name, post.Body, post.CreationDate, normalizedUsername, userId);
            if (user.NormalizedUserName == "ADMIN") return Ok(postDTO);

            return Ok(postDTO);
        }

        // api/posts/
        [HttpPost]
        [Authorize(Roles = VlogRoles.VlogUser)]
        public async Task<ActionResult<PostDTO>> Create(CreatePostDTO createPostDTO)
        {
            var post = new Post { Name = createPostDTO.Name, Body = createPostDTO.Body, CreationDate = DateTime.UtcNow, UserId = User.FindFirstValue(JwtRegisteredClaimNames.Sub)};
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, post, PolicyNames.ResourceOwner);
            if (!authorizationResult.Succeeded)
            {
                // 404
                return Forbid();
            }
            await postsRepository.CreateAsync(post);

            var normalizedUsername = _userManager.Users.FirstOrDefault(user => user.Id == User.FindFirstValue(JwtRegisteredClaimNames.Sub))?.NormalizedUserName;
            if (normalizedUsername == null) return NotFound();

            var postDTO = new PostDTO(post.Id, post.Name, post.Body,  post.CreationDate, normalizedUsername, _userManager.Users.FirstOrDefault(user => user.Id == post.UserId)?.Id);

            // 201
            return Created("$/api/posts/{post.Id}", postDTO);
        }

        // api/posts/
        [HttpPut("{postId}")]
        [Authorize(Roles = VlogRoles.VlogUser)]
        public async Task<ActionResult<PostDTO>> Update(int postId, UpdatePostDTO updatePostDTO)
        {
            var post = await postsRepository.GetPostAsync(postId);
            if (post == null) return NotFound();

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, post, PolicyNames.ResourceOwner);
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            //var user = _userManager.Users.FirstOrDefault(user => user.Id == User.FindFirstValue(JwtRegisteredClaimNames.Sub));
            //if (user.Id != post.UserId && user.NormalizedUserName != "ADMIN") return NotFound();

            post.Name = updatePostDTO.Name;
            post.Body = updatePostDTO.Body;
            //mapper.Map(updatePostDTO, post);

            await postsRepository.UpdateAsync(post);

            var normalizedUsername = _userManager.Users.FirstOrDefault(user => user.Id == post.UserId)?.NormalizedUserName;
            var userId = _userManager.Users.FirstOrDefault(user => user.Id == post.UserId)?.Id;

            PostDTO updatedPostDTO = new PostDTO(post.Id, updatePostDTO.Name, updatePostDTO.Body, post.CreationDate, normalizedUsername, userId);
            return Ok(updatedPostDTO);
        }

        // api/posts/
        [HttpDelete("{postId}", Name = "DeletePost")]
        [Authorize(Roles = VlogRoles.VlogUser)]
        public async Task<ActionResult> Remove(int postId)
        {
            var post = await postsRepository.GetPostAsync(postId);
            if (post == null) return NotFound();

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, post, PolicyNames.ResourceOwner);
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            //var user = _userManager.Users.FirstOrDefault(user => user.Id == User.FindFirstValue(JwtRegisteredClaimNames.Sub));
            //if (user.Id != tournament.UserId && user.NormalizedUserName != "ADMIN") return NotFound();

            await postsRepository.DeleteAsync(post);

            // 204
            return NoContent();
        }

        //[HttpGet("{postId}/likes")]
        //public async Task<ActionResult<IEnumerable<LikeDTO>>> GetPostLikes(int postId)
        //{
        //    var post = await postsRepository.GetPostAsync(postId);
        //    if (post == null) return NotFound();

        //    List<LikeDTO> postLikes = new List<LikeDTO>();

        //    var comments = await commentsRepository.GetManyAsync(postId);
        //    if (comments.Count == 0) return Ok(postLikes);


        //    foreach (Comment comment in comments)
        //    {
        //        var likes = await likesRepository.GetManyAsync(postId, comment.Id);
        //        if (likes.Count != 0)
        //        {
        //            foreach (Like like in likes)
        //            {
        //                postLikes.Add(new LikeDTO(like.Id, like.IsPositive, like.CreationDate));
        //            }
        //        }
        //    }
        //    return Ok(postLikes);
        //}

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
