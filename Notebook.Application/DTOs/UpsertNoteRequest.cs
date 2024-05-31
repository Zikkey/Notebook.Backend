namespace Notebook.Application.DTOs;

public class UpsertNoteRequest
{
    /// <summary>
    /// Название
    /// </summary>
    public string Title { get; set; }
    
    /// <summary>
    /// Описание
    /// </summary>
    public string Description { get; set; }
    
    /// <summary>
    /// Теги
    /// </summary>
    public string[] Tags { get; set; }
    
    /// <summary>
    /// Выполнено ли
    /// </summary>
    public bool IsDone { get; set; }
}