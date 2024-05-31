namespace Notebook.Domain.Base;

public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public DateTimeOffset ModifiedOn { get; set; }
    public bool IsDeleted { get; set; }
}