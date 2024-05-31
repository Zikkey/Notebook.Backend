using Notebook.Application.DTOs;
using Notebook.Domain.Entities;

namespace Notebook.Application.Services;

public interface INoteService
{
    public Task<NoteEntity> Create(UpsertNoteRequest request, CancellationToken token = default);
    
    public Task<NoteEntity> Update(UpdateNoteRequest request, CancellationToken token = default);
    
    public Task<NoteEntity?> GetById(Guid id, CancellationToken token = default);
    
    public Task<NoteEntity[]> GetAll(CancellationToken token = default);
    
    public Task DeleteById(Guid id, CancellationToken token = default);
}