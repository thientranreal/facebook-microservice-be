using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostWebApi.Models;
using PostWebApi.Repositories;

namespace PostWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Reactioncontroller : ControllerBase
    {
        private readonly PostDbContext _dbContext;
        private readonly PostRepository _postRepository;
        public Reactioncontroller(PostDbContext postDbContext, PostRepository postRepository)
        {
            _dbContext = postDbContext;
            _postRepository = postRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Story>>> GetStories()
        {
            var reactions = await _dbContext.Reactions.ToListAsync();
            if (reactions == null || !reactions.Any())
            {
                return NoContent(); // 204 No Content
            }
            return Ok(reactions); // 200 OK
        }
        
        [HttpPost]
        public async Task<ActionResult> Create(Reaction reaction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return 400 Bad Request with validation errors
            }
            try
            {
                // Check if the post exists
                var post = await _postRepository.Exited(reaction.PostId);
                if (post == null)
                {
                    return NotFound(); // Return 404 if post does not exist
                }

                // Add reaction to the database
                await _dbContext.Reactions.AddAsync(reaction);
                await _dbContext.SaveChangesAsync();

                return Ok(); // Return 200 OK
            }
            catch (Exception e)
            {
                // Log the error (replace with your logging mechanism)
                Console.WriteLine($"Error: {e.Message}");
                // Return 500 Internal Server Error
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        
        // DELETE: api/reaction/{postid}/{userid}
        [HttpDelete("{postid}/{userid}")]
        public async Task<IActionResult> Delete(int postid, int userid)
        {
            try
            {
                // 1. Try to find the reaction in the database by postid and userid
                var existingReaction = await _dbContext.Reactions
                    .SingleOrDefaultAsync(r => r.PostId == postid && r.UserId == userid);

                var post = await _postRepository.Exited(postid);

                // 2. If the reaction doesn't exist, return NotFound (404)
                if (existingReaction == null || post == null)
                {
                    return NotFound();  // Reaction or Post not found
                }

                // 3. Remove the reaction
                _dbContext.Reactions.Remove(existingReaction);

                // 4. Save changes asynchronously
                await _dbContext.SaveChangesAsync();

                // 5. Return NoContent (204) if the deletion was successful
                return NoContent();
            }
            catch (Exception e)
            {
                // Log the error (replace with your logging mechanism)
                Console.WriteLine($"Error: {e.Message}");

                // Return 500 Internal Server Error
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


    }
}