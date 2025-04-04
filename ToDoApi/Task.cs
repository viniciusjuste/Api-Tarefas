using System.ComponentModel.DataAnnotations;

public class Task
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
}