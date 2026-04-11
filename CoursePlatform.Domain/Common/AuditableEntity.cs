namespace CoursePlatform.Domain.Common;

public abstract class AuditableEntity : BaseEntity
{
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }   // userId (Guid as string)
    public string? UpdatedBy { get; set; }   // userId (Guid as string)
}