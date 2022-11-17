using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Data;
using System.Security.Claims;
using VlogAPI.Auth.Model;
using VlogAPI.Data.Entities;
using VlogAPI.Data.Repositories;
using static VlogAPI.Data.DTOs.CommentsDTO;

namespace VlogAPI.Controllers
{

    /*
     *
    /api/v1/posts/{postId}/comment GET List 200
    /api/v1/posts/{postId}/comment/{commentId} GET One 200
    /api/v1/posts/{postId}/comment POST Create 201
    /api/v1/posts/{postId}/comment/{commentId} PUT/PATCH Modify 200
    /api/v1/posts/{postId}/comment/{commentId} DELETE Remove 200/204
     */
    [ApiController]
    [Route("api/posts/{postId}/comments")]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentsRepository commentsRepository;
        private readonly IMapper mapper;
        private readonly IPostsRepository postsRepository;
        private readonly IAuthorizationService _authorizationService;

        public CommentsController(ICommentsRepository commentsRepository, IMapper mapper, IPostsRepository postsRepository, IAuthorizationService authorizationService)
        {
            this.commentsRepository = commentsRepository;
            this.mapper = mapper;
            this.postsRepository = postsRepository;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CommentDTO>>> GetMany(int postId)
        {
            var post = await postsRepository.GetPostAsync(postId);
            if (post == null) return NotFound();

            var comments = await commentsRepository.GetManyAsync(postId);

            return Ok(comments.Select(comment => mapper.Map<CommentDTO>(comment)));
        }

        // "api/posts/{postId}/comments/{commentId}
        [HttpGet("{commentId}", Name = "GetComment")]
        public async Task<ActionResult<CommentDTO>> GetOne(int postId, int commentId)
        {
            var comment = await commentsRepository.GetCommentAsync(postId, commentId);
            if (comment == null) return NotFound();

            return Ok(mapper.Map<CommentDTO>(comment));
        }

        // public record SearchLikeParameters(int postId, int commentId, int likeId);

        [HttpPost]
        [Authorize(Roles = VlogRoles.VlogUser)]
        public async Task<ActionResult<CommentDTO>> Create(int postId, CreateCommentDTO createCommentDTO)
        {
            var post = await postsRepository.GetPostAsync(postId);
            if (post == null) return NotFound();

            var comment = new Comment {  Content = createCommentDTO.content, CreationDate = DateTime.UtcNow, UserId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) };
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, comment, PolicyNames.ResourceOwner);
            if (!authorizationResult.Succeeded)
            {
                // 404
                return Forbid();
            }
            comment.Post = post;
            await commentsRepository.CreateAsync(comment);

            return Created($"/api/posts/{postId}/comments/{comment.Id}", mapper.Map<CommentDTO>(comment));
        }

        [HttpPut("{commentId}")]
        [Authorize(Roles = VlogRoles.VlogUser)]
        public async Task<ActionResult<CommentDTO>> Update(int postId, int commentId, UpdateCommentDTO updateCommentDTO)
        {
            var post = await postsRepository.GetPostAsync(postId);
            if (post == null) return NotFound();

            var oldComment = await commentsRepository.GetCommentAsync(postId, commentId);
            if (oldComment == null) return NotFound();

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, oldComment, PolicyNames.ResourceOwner);
            if (!authorizationResult.Succeeded)
            {
                // 404
                return Forbid();
            }

            oldComment.Content = updateCommentDTO.content;

            await commentsRepository.UpdateAsync(oldComment);

            return Ok(mapper.Map<CommentDTO>(oldComment));
        }

        [HttpDelete("{commentId}", Name = "RemoveComment")]
        [Authorize(Roles = VlogRoles.VlogUser)]
        public async Task<ActionResult> Remove(int postId, int commentId)
        {
            var post = await postsRepository.GetPostAsync(postId);
            if (post == null) return NotFound();

            var comment = await commentsRepository.GetCommentAsync(postId, commentId);

            // 404
            if (comment == null) return NotFound();

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, comment, PolicyNames.ResourceOwner);
            if (!authorizationResult.Succeeded)
            {
                // 404
                return Forbid();
            }

            await commentsRepository.DeleteAsync(comment);

            // 204
            return NoContent();
        }
    }
}
