using PostService.DTOs;

namespace PostService.Services
{
    public interface IPublication
    {
        public Task<PostDto> AddPostAsync(AddPostDto addPostItem);
        public Task<List<PostDto>> GetAllPostsAsync();
        public Task<PostDto> GetPost(long id);
    }
}
