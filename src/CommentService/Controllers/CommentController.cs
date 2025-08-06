using AutoMapper;
using CommentService.Data;
using CommentService.Data.Model;
using CommentService.DTOs;
using MassTransit;
using Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CommentService.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class CommentController(CommentDbContext dbContext, IMapper mapper, IPublishEndpoint publish) : ControllerBase
    { 
        private readonly CommentDbContext _dbContext = dbContext;
        private readonly IMapper _mapper = mapper;
        private readonly IPublishEndpoint _publish = publish;

        [Authorize(Policy = "HasAnyRole")]
        [HttpPost]
        [Route("comments")]
        public async Task<ActionResult<CommentDto>> AddComment(AddCommentDto comment)
        {
            var userId = User.FindFirst("UserId")?.Value;
            
            if(userId == null)
            {
                return Unauthorized();
            }

            var userInfo = await _dbContext.UsersInfo.Where(u => u.UserId.Contains(userId)).FirstOrDefaultAsync();
            var userInfoId = userInfo.Id;

            var addComment = new Comment();
            _mapper.Map(comment, addComment);

            addComment.UserInfoId = userInfoId;

            _dbContext.Add(addComment);
            await _dbContext.SaveChangesAsync();

            var sendComment = new CommentAdded
            {
                CommentId = addComment.Id,
                Text = addComment.Text
            };

            await _publish.Publish(sendComment);

            var result = new CommentDto();
            _mapper.Map(addComment, result);

            return result;

        }

        [Authorize(Policy = "HasAnyRole")]
        [HttpGet]
        [Route("comments")]
        public async Task<ActionResult<List<CommentFullInfoDto>>> GetComments()
        {
            var commentsInfo = await (from c in dbContext.Comments
                                      join u in dbContext.UsersInfo on c.UserInfoId equals u.Id
                                      select new CommentFullInfo
                                      {
                                          CommentId = c.Id,
                                          DateAdded = c.DateAdded,
                                          DateModified = c.DateModified,
                                          FirstName = u.FirstName,
                                          LastName = u.LastName,
                                          UserEmail = u.UserEmail,
                                          UserInfoId = c.UserInfoId,
                                          UserName = u.UserName,
                                          Text = c.Text,
                                          Status = c.Status,
                                          PostId = c.PostId
                                      }).ToListAsync();

            var result = new List<CommentFullInfoDto>();
            _mapper.Map(commentsInfo, result);

            return result;

        }

        [Authorize(Policy = "HasAnyRole")]
        [HttpGet]
        [Route("comments/{commentId:int}")]
        public async Task<ActionResult<CommentFullInfoDto>> GetComment(int commentId)
        {
            var commentsInfo = await (from c in dbContext.Comments
                                      join u in dbContext.UsersInfo on c.UserInfoId equals u.Id
                                      where c.Id == commentId
                                      select new CommentFullInfo
                                      {
                                          CommentId = c.Id,
                                          DateAdded = c.DateAdded,
                                          DateModified = c.DateModified,
                                          FirstName = u.FirstName,
                                          LastName = u.LastName,
                                          UserEmail = u.UserEmail,
                                          UserInfoId = c.UserInfoId,
                                          UserName = u.UserName,
                                          Text = c.Text,
                                          Status = c.Status,
                                          PostId = c.PostId
                                      }).FirstOrDefaultAsync();

            var result = new CommentFullInfoDto();
            _mapper.Map(commentsInfo, result);

            return result;
        }
    }
}
