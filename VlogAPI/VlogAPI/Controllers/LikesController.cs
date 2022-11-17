using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Data;
using System.Security.Claims;
using VlogAPI.Auth.Model;
using VlogAPI.Data.Entities;
using VlogAPI.Data.Repositories;
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


        public LikesController(ICommentsRepository commentsRepository, IMapper mapper, IPostsRepository postsRepository, ILikesRepository likesRepository, IAuthorizationService authorizationService)
        {
            this.commentsRepository = commentsRepository;
            this.mapper = mapper;
            this.postsRepository = postsRepository;
            this.likesRepository = likesRepository;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikeDTO>>> GetMany(int postId, int commentId)
        {
            var comment = await commentsRepository.GetCommentAsync(postId, commentId);
            if (comment == null) return NotFound();

            var likes = await likesRepository.GetManyAsync(postId, commentId);

            return Ok(likes.Select(like => mapper.Map<LikeDTO>(like)));
        }

        // "api/posts/{postId}/comments/{commentId}/likes/{likeId}
        [HttpGet("{likeId}", Name = "GetLike")]
        public async Task<ActionResult<LikeDTO>> GetOne(int postId, int commentId, int likeId)
        {
            var like = await likesRepository.GetLikeAsync(postId, commentId, likeId);
            if (like == null) return NotFound();

            return Ok(mapper.Map<LikeDTO>(like));
        }


        [HttpPost]
        [Authorize(Roles = VlogRoles.VlogUser)]
        public async Task<ActionResult<LikeDTO>> Create(int postId, int commentId, CreateLikeDTO createLikeDTO)
        {
            var post = await postsRepository.GetPostAsync(postId);
            if (post == null) return NotFound();

            var comment = await commentsRepository.GetCommentAsync(postId, commentId);
            if (post == null) return NotFound();

            var like = new Like { IsPositive = createLikeDTO.isPositive, CreationDate = DateTime.UtcNow, UserId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) };

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, like, PolicyNames.ResourceOwner);
            if (!authorizationResult.Succeeded)
            {
                // 404
                return Forbid();
            }

            like.Comment = comment;
            await likesRepository.CreateAsync(like);

            return Created($"/api/posts/{postId}/comments/{commentId}/likes/{like.Id}", mapper.Map<LikeDTO>(like));
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
                // 404
                return Forbid();
            }

            oldLike.IsPositive = updateLikeDTO.isPositive;

            await likesRepository.UpdateAsync(oldLike);

            return Ok(mapper.Map<LikeDTO>(oldLike));
        }

        [HttpDelete("{likeId}", Name = "RemoveLike")]
        [Authorize(Roles = VlogRoles.VlogUser)]
        public async Task<ActionResult> Remove(int postId, int commentId, int likeId)
        {
            var post = await postsRepository.GetPostAsync(postId);
            if (post == null) return NotFound();

            var comment = await commentsRepository.GetCommentAsync(postId, commentId);
            if (post == null) return NotFound();

            var like = await likesRepository.GetLikeAsync(postId, commentId, likeId);
            // 404
            if (like == null) return NotFound();

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, like, PolicyNames.ResourceOwner);
            if (!authorizationResult.Succeeded)
            {
                // 404
                return Forbid();
            }

            await likesRepository.DeleteAsync(like);

            // 204
            return NoContent();
        }
    }
}
