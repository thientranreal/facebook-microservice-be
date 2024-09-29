using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContactWebApi.Models;

namespace ContactWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JoiningController : ControllerBase
    {
        private readonly ContactDbContext _context;

        public JoiningController(ContactDbContext context)
        {
            _context = context;
        }

        // GET: api/Joining
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Joining>>> GetJoinings()
        {
            return await _context.Joinings.ToListAsync();
        }

        // GET: api/Joining/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Joining>> GetJoining(int id)
        {
            var joining = await _context.Joinings.FindAsync(id);

            if (joining == null)
            {
                return NotFound();
            }

            return joining;
        }

        // PUT: api/Joining/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutJoining(int id, Joining joining)
        {
            if (id != joining.Id)
            {
                return BadRequest();
            }

            _context.Entry(joining).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JoiningExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Joining
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Joining>> PostJoining(Joining joining)
        {
            _context.Joinings.Add(joining);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetJoining", new { id = joining.Id }, joining);
        }

        // DELETE: api/Joining/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJoining(int id)
        {
            var joining = await _context.Joinings.FindAsync(id);
            if (joining == null)
            {
                return NotFound();
            }

            _context.Joinings.Remove(joining);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool JoiningExists(int id)
        {
            return _context.Joinings.Any(e => e.Id == id);
        }
    }
}
