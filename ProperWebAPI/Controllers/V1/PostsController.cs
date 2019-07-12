using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProperWebAPI.Contract.V1;
using ProperWebAPI.Contract.V1.Requests;
using ProperWebAPI.Contract.V1.Response;
using ProperWebAPI.Domain;
using ProperWebAPI.Services;

namespace ProperWebAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme) ]
    public class PostsController : Controller
    {
        private readonly IPostService postService;

        public PostsController(IPostService PostService)
        {
            postService = PostService;
        }

        [HttpGet(ApiRoutes.Posts.GetAll)]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await postService.GetPostsAsync());
        }

        [HttpGet(ApiRoutes.Posts.Get)]
        public async Task<IActionResult> Get([FromRoute]Guid PostId)
        {
            var post = await postService.GetPostByIdAsync(PostId);

            if (post == null)
            {
                return NotFound();
            }
            return Ok(post);
        }

        [HttpPut(ApiRoutes.Posts.Update)]
        public async Task<IActionResult> Update([FromRoute]Guid PostId, [FromBody] UpdatePostRequest request)
        {
            var post = new Post
            {
                Id = PostId,
                Name = request.Name
            };

            var updated = await postService.UpdatePostAsync(post);

            if (updated)
            {
                return Ok(post);
            }

            return NotFound(post);
        }

        [HttpDelete(ApiRoutes.Posts.Delete)]
        public async Task<IActionResult> Delete([FromRoute]Guid PostId)
        {
            
            var deleted = await postService.DeletePostAsync(PostId);

            if (deleted)
            {
                return NoContent();
            }

            return NotFound();
        }

        [HttpPost(ApiRoutes.Posts.Create)]
        public async Task<IActionResult> Create([FromBody] CreatePostRequest postRequest)
        {
            var post = new Post { Name = postRequest.Name};
             
            await postService.CreatePostAsync(post);

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var locationUri = baseUrl + "/" + ApiRoutes.Posts.Get.Replace("{PostId}", post.Id.ToString());

            var response = new PostResponse { Id = post.Id };
            return Created(locationUri, response);
        }
    }
}