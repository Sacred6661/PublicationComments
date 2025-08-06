using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PostService.DTOs;
using PostService.Services;

namespace PostService.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class PostController(IPublication publication) : ControllerBase
    {
        private readonly IPublication _publication = publication;

        [Authorize(Policy = "HasAnyRole")]
        [HttpPost]
        [Route("posts")]
        public async Task<ActionResult<PostDto>> AddPost(AddPostDto addPost)
        {
            var result = await _publication.AddPostAsync(addPost);

            return result;
        }

        [Authorize(Policy = "HasAnyRole")]
        [HttpGet]
        [Route("posts")]
        public async Task<ActionResult<List<PostDto>>> GetAllPosts()
        {
            var result = await _publication.GetAllPostsAsync();

            return result;
        }

        [Authorize(Policy = "HasAnyRole")]
        [HttpGet]
        [Route("posts/{postId:int}")]
        public async Task<ActionResult<PostDto>> GetPost(long postId)
        {
            var result = await _publication.GetPost(postId);

            return result;
        }
    }
}
