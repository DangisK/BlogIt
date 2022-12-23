using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Data;
using System.Security.Claims;
using VlogAPI.Auth.Model;
using VlogAPI.Data.Entities;
using VlogAPI.Data.Repositories;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;
using static VlogAPI.Data.DTOs.CommentsDTO;
using static VlogAPI.Data.DTOs.LikesDTO;

namespace VlogAPI.Controllers
{
    [ApiController]
    [Route("api/posts/{postId}/comments/{commentId}/likes")]
    public class LikesController : ControllerBase
    {
        private readonly ICommentsRepository commentsRepository;
        private readonly IPostsRepository postsRepository;
        private readonly ILikesRepository likesRepository;
        private readonly IMapper mapper;
        private readonly IAuthorizationService _authorizationService;
        private readonly UserManager<VlogUser> _userManager;

        public LikesController(IPostsRepository postsRepository, IMapper mapper, ICommentsRepository commentsRepository, ILikesRepository likesRepository, IAuthorizationService _authorizationService, UserManager<VlogUser> userManager)
        {
            this.postsRepository = postsRepository;
            this.commentsRepository = commentsRepository;
            this.likesRepository = likesRepository;
            this.mapper = mapper;
            this._authorizationService = _authorizationService;
            this._userManager = userManager;
        }

        //var post = await postsRepository.GetPostAsync(postId);
        //    if (post == null) return NotFound();

        //var comments = await commentsRepository.GetManyAsync(postId);
        //var commentsDTOs = comments.Select(comment => new CommentDTO(comment.Id, comment.Content, comment.CreationDate, _userManager.Users.FirstOrDefault(user => user.Id == comment.UserId).NormalizedUserName, postId));

        //    return Ok(commentsDTOs);

        [HttpGet]
        [Authorize(Roles = VlogRoles.VlogUser)]
        public async Task<ActionResult<IEnumerable<LikeDTO>>> GetMany(int postId, int commentId)
        {
            var post = await postsRepository.GetPostAsync(postId);
            if (post == null) return NotFound();
            var comment = await commentsRepository.GetCommentAsync(postId, commentId);
            if (comment == null) return NotFound();

            var likes = await likesRepository.GetManyAsync(postId, commentId);
            var likesDTOs = likes.Select(like => new LikeDTO(like.Id, like.IsPositive, like.CreationDate, commentId, postId,
                _userManager.Users.FirstOrDefault(user => user.Id == like.UserId)?.NormalizedUserName,
                _userManager.Users.FirstOrDefault(user => user.Id == like.UserId)?.Id));

            return Ok(likesDTOs);
        }

        // "api/posts/{postId}/comments/{commentId}/likes/{likeId}
        [HttpGet("{likeId}", Name = "GetLike")]
        [Authorize(Roles = VlogRoles.VlogUser)]
        public async Task<ActionResult<LikeDTO>> GetOne(int postId, int commentId, int likeId)
        {
            var post = await postsRepository.GetPostAsync(postId);
            if (post == null) return NotFound();

            var comment = await commentsRepository.GetCommentAsync(postId, commentId);
            if (comment == null) return NotFound();

            var like = await likesRepository.GetLikeAsync(postId, commentId, likeId);
            if (like == null) return NotFound();

            var likeDTO = new LikeDTO(like.Id, like.IsPositive, like.CreationDate, commentId, postId,
                _userManager.Users.FirstOrDefault(user => user.Id == like.UserId)?.NormalizedUserName,
                _userManager.Users.FirstOrDefault(user => user.Id == like.UserId)?.Id);

            return Ok(likeDTO);
        }


        [HttpPost]
        [Authorize(Roles = VlogRoles.VlogUser)]
        public async Task<ActionResult<LikeDTO>> Create(int postId, int commentId, CreateLikeDTO createLikeDTO)
        {
            var post = await postsRepository.GetPostAsync(postId);
            if (post == null) return NotFound();

            var comment = await commentsRepository.GetCommentAsync(postId, commentId);
            if (post == null) return NotFound();

            var likes = await likesRepository.GetManyAsync(postId, commentId);
            bool hasLikedAlready = likes.Any(like => like.UserId == User.FindFirstValue(JwtRegisteredClaimNames.Sub));

            if (hasLikedAlready) return Conflict();

            var like = new Like { IsPositive = createLikeDTO.IsPositive, CreationDate = DateTime.UtcNow, UserId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) };

            var normalizedUsername = _userManager.Users.FirstOrDefault(user => user.Id == like.UserId)?.NormalizedUserName;
            var userId = _userManager.Users.FirstOrDefault(user => user.Id == like.UserId)?.Id;
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, like, PolicyNames.ResourceOwner);
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }


            like.Comment = comment;
            await likesRepository.CreateAsync(like);

            var likeDTO = new LikeDTO(like.Id, like.IsPositive, like.CreationDate, commentId, postId, normalizedUsername, userId);

            return Created($"/api/posts/{postId}/comments/{commentId}/likes/{like.Id}", likeDTO);
        }

        [HttpPut("{likeId}")]
        [Authorize(Roles = VlogRoles.VlogUser)]
        public async Task<ActionResult<LikeDTO>> Update(int postId, int commentId, int likeId, UpdateLikeDTO updateLikeDTO)
        {
            var post = await postsRepository.GetPostAsync(postId);
            if (post == null) return NotFound();

            var comment = await commentsRepository.GetCommentAsync(postId, commentId);
            if (comment == null) return NotFound();

            var oldLike = await likesRepository.GetLikeAsync(postId, commentId, likeId);
            if (oldLike == null) return NotFound();

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, oldLike, PolicyNames.ResourceOwner);
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            oldLike.IsPositive = !updateLikeDTO.IsPositive;
            await likesRepository.UpdateAsync(oldLike);

            var normalizedUsername = _userManager.Users.FirstOrDefault(user => user.Id == oldLike.UserId)?.NormalizedUserName;
            var userId = _userManager.Users.FirstOrDefault(user => user.Id == oldLike.UserId)?.Id;

            var likeDTO = new LikeDTO(oldLike.Id, oldLike.IsPositive, oldLike.CreationDate, commentId, postId, normalizedUsername, userId);

            return Ok(likeDTO);
        }

        [HttpDelete("{likeId}", Name = "RemoveLike")]
        [Authorize(Roles = VlogRoles.VlogUser)]
        public async Task<ActionResult> Remove(int postId, int commentId, int likeId)
        {
            var post = await postsRepository.GetPostAsync(postId);
            if (post == null) return NotFound();

            var comment = await commentsRepository.GetCommentAsync(postId, commentId);
            if (comment == null) return NotFound();

            var like = await likesRepository.GetLikeAsync(postId, commentId, likeId);
            if (like == null) return NotFound();

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, like, PolicyNames.ResourceOwner);
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            await likesRepository.DeleteAsync(like);

            return NoContent();
        }
    }
}
