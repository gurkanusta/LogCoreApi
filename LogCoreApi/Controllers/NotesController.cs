using LogCoreApi.Entities;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LogCoreApi.Data;

using LogCoreApi.Exceptions;
using AutoMapper;
using LogCoreApi.DTOs.Notes;
using Microsoft.AspNetCore.Authorization;


namespace LogCoreApi.Controllers;





[ApiController]
[Route("api/[controller]")]
public class NotesController : ControllerBase
{
    private readonly AppDbContext _context;

    public NotesController(AppDbContext context)
    {
        _context = context;
    }


    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var notes = await _context.Notes.ToListAsync();
        var result = _mapper.Map<List<NoteResponseDto>>(notes);
        return Ok(result);
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var note = await _context.Notes.FindAsync(id); 

        if (note is null) 
            throw new NotFoundException("Note not found"); 

        var result = _mapper.Map<NoteResponseDto>(note); 
        return Ok(result); 
    }



    [HttpPost]
    public async Task<IActionResult> Create(NoteCreateDto dto)
    {
        
        

        var entity = _mapper.Map<Note>(dto); 
        _context.Notes.Add(entity); 
        await _context.SaveChangesAsync(); 

        var result = _mapper.Map<NoteResponseDto>(entity); 
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, result); 
    }


    private readonly IMapper _mapper;



    public NotesController(AppDbContext context, IMapper mapper)
    {
        _context = context; 
        _mapper = mapper;   
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, NoteUpdateDto dto)
    {
        var note = await _context.Notes.FindAsync(id); 

        if (note is null) 
            throw new NotFoundException("Note not found"); 

        

        _mapper.Map(dto, note); 
        await _context.SaveChangesAsync(); 

        return NoContent(); 
    }



    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var note = await _context.Notes.FindAsync(id); 

        if (note is null) 
            throw new NotFoundException("Note not found"); 

        _context.Notes.Remove(note); 
        await _context.SaveChangesAsync(); 

        return NoContent(); 
    }





}
