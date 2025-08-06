using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PostService.Data;
using PostService.Data.Models;
using PostService.DTOs;

namespace PostService.Services
{
    public class PublicationService(PostDbContexts dbContext, IMapper mapper) : IPublication
    {
        private readonly PostDbContexts _dbContext = dbContext;
        private readonly IMapper _mapper = mapper;

        public async Task<PostDto> AddPostAsync(AddPostDto addPostItem)
        {
            var post = new Post();
            _mapper.Map(addPostItem, post);

            _dbContext.Add(post);
            await _dbContext.SaveChangesAsync();

            var result = new PostDto();
            _mapper.Map(post, result);

            return result;
        }

        public async Task<List<PostDto>> GetAllPostsAsync()
        {
            var posts = await _dbContext.Posts.ToListAsync();

            var result = new List<PostDto>();
            _mapper.Map(posts, result);

            return result;
        }

        public async Task<PostDto> GetPost(long id)
        {
            var post = await _dbContext.Posts.FirstOrDefaultAsync();

            var result = new PostDto();
            _mapper.Map(post, result);

            return result;
        }


    }
}
