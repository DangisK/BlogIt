using AutoMapper;
using VlogAPI.Data.DTOs;
using VlogAPI.Data.Entities;
using static VlogAPI.Data.DTOs.CommentsDTO;
using static VlogAPI.Data.DTOs.LikesDTO;

namespace VlogAPI.Data
{
    public class RestProfile : Profile
    {
        public RestProfile()
        {
            CreateMap<Post, PostDTO>().ForMember(dest => dest.Username, act => act.MapFrom(src => src.User.NormalizedUserName));
            CreateMap<CreatePostDTO, Post>();
            CreateMap<UpdatePostDTO, Post>();

            CreateMap<Comment, CommentDTO>();
            CreateMap<CreateCommentDTO, Comment>();
            CreateMap<UpdateCommentDTO, Comment>();

            CreateMap<Like, LikeDTO>();
            CreateMap<CreateLikeDTO, Like>();
            CreateMap<UpdateLikeDTO, Like>();
        }
    }
}
