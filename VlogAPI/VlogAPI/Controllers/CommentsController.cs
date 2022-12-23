using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Data;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using VlogAPI.Auth.Model;
using VlogAPI.Data.DTOs;
using VlogAPI.Data.Entities;
using VlogAPI.Data.Repositories;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
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
        private readonly IPostsRepository postsRepository;
        private readonly ILikesRepository likesRepository;
        private readonly IMapper mapper;
        private readonly IAuthorizationService _authorizationService;
        private readonly UserManager<VlogUser> _userManager;

        public CommentsController(IPostsRepository postsRepository, IMapper mapper, ICommentsRepository commentsRepository, ILikesRepository likesRepository, IAuthorizationService _authorizationService, UserManager<VlogUser> userManager)
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
        public async Task<ActionResult<IEnumerable<CommentDTO>>> GetMany(int postId)
        {
            var post = await postsRepository.GetPostAsync(postId);
            if (post == null) return NotFound();

            var comments = await commentsRepository.GetManyAsync(postId);
            var commentsDTOs = comments.Select(comment => new CommentDTO(comment.Id, comment.Content, comment.CreationDate, postId, 
                _userManager.Users.FirstOrDefault(user => user.Id == comment.UserId)?.NormalizedUserName,
                _userManager.Users.FirstOrDefault(user => user.Id == comment.UserId)?.Id));

            return Ok(commentsDTOs);
        }

        // "api/posts/{postId}/comments/{commentId}
        [HttpGet("{commentId}", Name = "GetComment")]
        [Authorize(Roles = VlogRoles.VlogUser)]
        public async Task<ActionResult<CommentDTO>> GetOne(int postId, int commentId)
        {
            var post = await postsRepository.GetPostAsync(postId);
            if (post == null) return NotFound();

            var comment = await commentsRepository.GetCommentAsync(postId, commentId);
            if (comment == null) return NotFound();

            var commentDTO = new CommentDTO(comment.Id, comment.Content, comment.CreationDate, postId,
                _userManager.Users.FirstOrDefault(user => user.Id == comment.UserId)?.NormalizedUserName,
                _userManager.Users.FirstOrDefault(user => user.Id == comment.UserId)?.Id);

            return Ok(commentDTO);
        }

        // public record SearchLikeParameters(int postId, int commentId, int likeId);

        [HttpPost]
        [Authorize(Roles = VlogRoles.VlogUser)]
        public async Task<ActionResult<CommentDTO>> Create(int postId, CreateCommentDTO createCommentDTO)
        {
            var post = await postsRepository.GetPostAsync(postId);
            if (post == null) return NotFound();

            var comment = new Comment {  Content = createCommentDTO.Content, CreationDate = DateTime.UtcNow, UserId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) };
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, comment, PolicyNames.ResourceOwner);
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            comment.Post = post;
            await commentsRepository.CreateAsync(comment);

            var normalizedUsername = _userManager.Users.FirstOrDefault(user => user.Id == User.FindFirstValue(JwtRegisteredClaimNames.Sub))?.NormalizedUserName;
            var userId = _userManager.Users.FirstOrDefault(user => user.Id == User.FindFirstValue(JwtRegisteredClaimNames.Sub))?.Id;
            if (normalizedUsername == null) return NotFound();

            var commentDTO = new CommentDTO(comment.Id, comment.Content, comment.CreationDate, postId, normalizedUsername, userId);

            return Created($"/api/posts/{postId}/comments/{comment.Id}", commentDTO);
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
                return Forbid();
            }

            oldComment.Content = updateCommentDTO.Content;
            await commentsRepository.UpdateAsync(oldComment);

            var normalizedUsername = _userManager.Users.FirstOrDefault(user => user.Id == oldComment.UserId)?.NormalizedUserName;
            var userId = _userManager.Users.FirstOrDefault(user => user.Id == oldComment.UserId)?.Id;

            var commentDTO = new CommentDTO(oldComment.Id, oldComment.Content, oldComment.CreationDate, postId, normalizedUsername, userId);

            return Ok(commentDTO);
        }

        [HttpDelete("{commentId}", Name = "RemoveComment")]
        [Authorize(Roles = VlogRoles.VlogUser)]
        public async Task<ActionResult> Remove(int postId, int commentId)
        {
            var post = await postsRepository.GetPostAsync(postId);
            if (post == null) return NotFound();

            var comment = await commentsRepository.GetCommentAsync(postId, commentId);
            if (comment == null) return NotFound();

            if (comment.Post.Id != post.Id) return NotFound();

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, comment, PolicyNames.ResourceOwner);
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            await commentsRepository.DeleteAsync(comment);

            // 204
            return NoContent();
        }
    }
}
