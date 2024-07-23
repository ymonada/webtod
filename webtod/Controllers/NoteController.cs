using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webtod.Contracts;
using webtod.DataAccess;
using webtod.DataAccess.Entities;

namespace webtod.Controllers;
[ApiController]
[Route("controller")]
public class NoteController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public NoteController(ApplicationDbContext context)
    {
        _context = context;
    }
    [HttpGet]
    public async Task<IActionResult> GetByFilter([FromQuery] GetNoteRequest request)
    {
        var notesQuery =  _context.Notes
            .Where(x => string.IsNullOrWhiteSpace(request.search) ||
                   x.Title.ToLower().Contains(request.search.ToLower()));
        Expression<Func<NoteEntity, object>> selectorKey = request.sortItem?.ToLower() switch
        {
            "date" => note => note.CreatedAt,
            "title" => note => note.Title,
            "upadate" => note => note.UpdatedAt,
            _ => note => note.Id
        };
        notesQuery = request.sortOrder == "desc" ? 
            notesQuery.OrderByDescending(selectorKey) : 
            notesQuery.OrderBy(selectorKey);
        
        var noteDtos = await notesQuery
            .Select(x => new NoteDto(x.Id, x.Title, x.Description, x.CreatedAt, x.UpdatedAt))
            .ToListAsync();
        return Ok(new GetNoteResponse(noteDtos));

    }
    [HttpPost]
    public async Task<IActionResult> Create([FromBody]CreateNoteRequest request)
    {
        var note = new NoteEntity(request.title, request.description);
        await _context.Notes.AddAsync(note);
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateNoteRequest request)
    {
        await _context.Notes
            .Where(x => x.Id == request.id)
            .ExecuteUpdateAsync(x => x
                .SetProperty(s => s.Title, s => request.title)
                .SetProperty(s => s.Description, s => request.description)
                .SetProperty(s => s.UpdatedAt, s => DateTime.UtcNow)
            );
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _context.Notes
            .Where(x => x.Id == id)
            .ExecuteDeleteAsync();
        await _context.SaveChangesAsync();
        return Ok();
    }
}