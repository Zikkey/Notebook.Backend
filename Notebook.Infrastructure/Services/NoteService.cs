using Mapster;
using Microsoft.EntityFrameworkCore;
using Notebook.Application.DTOs;
using Notebook.Application.Persistence;
using Notebook.Application.Services;
using Notebook.Domain.Entities;

namespace Notebook.Infrastructure.Services;

public class NoteService : INoteService
{
    private readonly IApplicationDbContext context;

    public NoteService(IApplicationDbContext context)
    {
        this.context = context;
    }

    public async Task<NoteEntity> Create(UpsertNoteRequest request, CancellationToken token = default)
    {
        var note = request.Adapt<NoteEntity>();

        await context.Notes.AddAsync(note, token);
        
        await context.SaveChangesAsync(token);

        return note;
    }

    public async Task<NoteEntity> Update(UpdateNoteRequest request, CancellationToken token = default)
    {
        var note = await GetById(request.Id, token);
        
        if (note is null)
            throw new Exception("Записка не найдена");

        request.Adapt(note);
        
        await context.SaveChangesAsync(token);

        return note;
    }

    public async Task<NoteEntity?> GetById(Guid id, CancellationToken token = default)
    {
        return await context.Notes.Where(n => !n.IsDeleted && n.Id == id).FirstOrDefaultAsync(cancellationToken: token);
    }

    public async Task<NoteEntity[]> GetAll(CancellationToken token = default)
    {
        return await context.Notes.Where(n => !n.IsDeleted).OrderByDescending(n => n.CreatedOn).ToArrayAsync(cancellationToken: token);
    }

    public async Task DeleteById(Guid id, CancellationToken token = default)
    {
        var note = await GetById(id, token);

        if (note is null)
            throw new Exception("Записка не найдена");

        context.Notes.Remove(note);

        await context.SaveChangesAsync(token);
    }
}