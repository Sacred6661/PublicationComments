using AutoMapper;
using MassTransit;
using Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProfileService.Data;
using ProfileService.DTOs;
using System.Diagnostics;
using System.Security.Claims;

namespace ProfileService.Controllers
{
    [Route("api/v1")]
    public class ProfileController(ProfileDbContext dbContext, IMapper mapper, IPublishEndpoint publish) : ControllerBase
    {
        private readonly ProfileDbContext _dbContext = dbContext;
        private readonly IMapper _mapper = mapper;
        private readonly IPublishEndpoint _publish = publish;

        [Authorize(Policy = "HasAnyRole")]
        [HttpPut("profile")]
        public async Task<ActionResult<UserProfileDto>> EditProfile([FromBody] EditUserProfile userInfo)
        {
            var userId = User.FindFirst("UserId")?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var isAdmin = (userRole?.ToLower()?.Contains("admin") ?? false);

            if (userId == null || userRole == null)
                return Unauthorized();

            if (!isAdmin)
                userInfo.UserId = userId;

            var user = _dbContext.UserProfiles.Where(u => u.UserId.Contains(userInfo.UserId))?.FirstOrDefault();

            if (user != null)
            {
                if(!string.IsNullOrEmpty(userInfo.FirstName))
                    user.FirstName = userInfo.FirstName;
                if (!string.IsNullOrEmpty(userInfo.LastName))
                    user.LastName = userInfo.LastName;
                if (!string.IsNullOrEmpty(userInfo.PhoneNumber))
                    user.PhoneNumber = userInfo.PhoneNumber;
                if (!string.IsNullOrEmpty(userInfo.AvatarUrl))
                    user.AvatarUrl = userInfo.AvatarUrl;

                user.DateUpdated = DateTime.UtcNow;

                await _dbContext.SaveChangesAsync();

                var profileEdited = new ProfileEdited();
                if (!string.IsNullOrEmpty(userInfo.FirstName))
                    profileEdited.FirstName = userInfo.FirstName;
                if (!string.IsNullOrEmpty(userInfo.LastName))
                    profileEdited.LastName = userInfo.LastName;
                if (!string.IsNullOrEmpty(userInfo.PhoneNumber))
                    profileEdited.PhoneNumber = userInfo.PhoneNumber;
                if (!string.IsNullOrEmpty(userInfo.AvatarUrl))
                    profileEdited.AvatarUrl = userInfo.AvatarUrl;

                profileEdited.UserId = userInfo.UserId;

                await _publish.Publish(profileEdited);

                var result = new UserProfileDto();
                _mapper.Map(user, result);

                return Ok(result);
            }

            return NotFound("User not found");
        }

        [Authorize(Policy = "HasAnyRole")]
        [HttpGet("profile")]
        public async Task<ActionResult<UserProfileDto>> GetMyProfile()
        {
            var userId = User.FindFirst("UserId")?.Value;

            if (userId == null)
                return Unauthorized();

            var user = await  _dbContext.UserProfiles.Where(u => u.UserId.Contains(userId))?.FirstOrDefaultAsync();

            if (user == null)
                return NotFound("User not found");

            var result = new UserProfileDto();
            _mapper.Map(user, result);

            return Ok(result);
        }

        [Authorize(Policy = "HasAnyRole")]
        [HttpGet("profile/{userId}")]
        public async Task<ActionResult<UserProfileDto>> GetProfile(string userId)
        {
            if (userId == null)
                return Unauthorized();

            var user = await _dbContext.UserProfiles.Where(u => u.UserId.Contains(userId))?.FirstOrDefaultAsync();

            if (user == null)
                return NotFound("User not found");

            var result = new UserProfileDto();
            _mapper.Map(user, result);

            return Ok(result);
        }
    }
}
