using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostWebApi.Models;

namespace PostWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Reactioncontroller : ControllerBase
    {
        private readonly PostDbContext _dbContext;
        
        public Reactioncontroller(PostDbContext postDbContext)
        {
            _dbContext = postDbContext;
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
                return BadRequest(ModelState);  // Trả về 400 Bad Request với thông tin lỗi
            }
            await _dbContext.Reactions.AddAsync(reaction);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
        
        // DELETE: api/reaction/{postid}/{userid}
        [HttpDelete("{postid}/{userid}")]
        public async Task<IActionResult> Delete(int postid, int userid)
        {
            // 1. Try to find the reaction in the database by postid and userid
            var existingReaction = await _dbContext.Reactions
                .SingleOrDefaultAsync(r => r.PostId == postid && r.UserId == userid);

            // 2. If the reaction doesn't exist, return NotFound (404)
            if (existingReaction == null)
            {
                return NotFound();  // Reaction not found
            }

            // 3. Remove the reaction
            _dbContext.Reactions.Remove(existingReaction);

            // 4. Save changes asynchronously
            await _dbContext.SaveChangesAsync();

            // 5. Return NoContent (204) if the deletion was successful
            return NoContent();
        }

    }
}