using Microsoft.AspNetCore.Mvc;
using Notebook.Application.DTOs;
using Notebook.Application.Services;
using Notebook.Domain.Entities;

namespace Notebook.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class NoteController : ControllerBase
{
    private readonly INoteService noteService;

    public NoteController(INoteService noteService)
    {
        this.noteService = noteService;
    }

    [HttpGet]
    [Route("get")]
    public async Task<NoteEntity?> Get(Guid id, CancellationToken token = default)
    {
        return await noteService.GetById(id, token);
    }
    
    [HttpGet]
    [Route("get_all")]
    public async Task<NoteEntity[]> GetAll(CancellationToken token = default)
    {
        return await noteService.GetAll(token);
    }
    
    [HttpPost]
    [Route("create")]
    public async Task<NoteEntity> Create(UpsertNoteRequest request, CancellationToken token = default)
    {
        return await noteService.Create(request, token);
    }
    
    [HttpPost]
    [Route("update")]
    public async Task<NoteEntity> Update(UpdateNoteRequest request, CancellationToken token = default)
    {
        return await noteService.Update(request, token);
    }
    
    [HttpDelete]
    [Route("delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken token = default)
    {
        await noteService.DeleteById(id, token);

        return Ok();
    }
}