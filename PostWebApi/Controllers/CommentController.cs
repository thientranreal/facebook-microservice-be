using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostWebApi.Models;
using PostWebApi.Repositories;

namespace PostWebApi.Controllers;
[Route("api/[controller]")]
[ApiController]
public class CommentController:ControllerBase
{
    private readonly PostDbContext _dbContext;
    private readonly PostRepository _postRepository;
    public CommentController(PostDbContext postDbContext, PostRepository postRepository)
    {
        _dbContext = postDbContext;
        _postRepository = postRepository;
    }

    // GET: api/comment
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Comment>>> GetComments()
    {
        var comments = await _dbContext.Comments.ToListAsync();
        if (comments == null || !comments.Any())
        {
            return NoContent(); // 204 No Content
        }
        return Ok(comments); // 200 OK
    }

    //POST : api/comment
    [HttpPost]
    public async Task<ActionResult<Comment>> Create(Comment comment)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);  // Trả về 400 Bad Request với thông tin lỗi
        }
        try
        {
            // Kiểm tra model có hợp lệ không dựa trên Data Annotations
            
            var post = await _postRepository.Exited(comment.PostId);
            if (post == null)
            {
                return NotFound(); // Return 404 if post does not exist
            }

            comment.Timeline = DateTime.Now;
            await _dbContext.Comments.AddAsync(comment);
            await _dbContext.SaveChangesAsync();

            // Trả về 201 Created cùng với comment đã tạo
            return CreatedAtAction(nameof(Create), new { id = comment.Id }, comment);
        }
        catch (Exception e)
        {
            // Log the error (replace with your logging mechanism)
            Console.WriteLine($"Error: {e.Message}");

            // Return 500 Internal Server Error
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    //PUT: api/comment/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult<Comment>> Update(int id, [FromBody] Comment comment)
    {
        //Kiểm tra model có hợp lệ không
        if (string.IsNullOrWhiteSpace(comment.Content))
        {
            return BadRequest("New content cannot be empty");  // Trả về 400 nếu nội dung mới rỗng
        }

        try
        {
            // 2. Tìm comment hiện tại theo id
            var existingComment = await _dbContext.Comments.SingleOrDefaultAsync(c => c.Id == id);
            // 3. Nếu không tồn tại comment, trả về NotFound (404)
            if (existingComment == null)
            {
                return NotFound();  // Trả về 404 nếu không tìm thấy comment
            }
            // 4. Cập nhật các thuộc tính của comment hiện tại với giá trị mới
            existingComment.Content = comment.Content;  // Cập nhật nội dung của comment

            // 5. Lưu thay đổi vào database
            await _dbContext.SaveChangesAsync();

            // 6. Trả về NoContent (204) để chỉ ra rằng cập nhật thành công
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

    
    //DELETE: api/post/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            // 1. Try to find the blog post in the database
            var existingComment = await _dbContext.Comments.SingleOrDefaultAsync(c => c.Id == id);
            // 2. If the post doesn't exist, return NotFound (404)
            if (existingComment == null)
            {
                return NotFound();  // Post not found
            }
            // 3. Remove the post
            _dbContext.Comments.Remove(existingComment);
            // 4. Save changes asynchronously
            await _dbContext.SaveChangesAsync();
            // 5. Return NoContent (200) if the deletion was successful
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