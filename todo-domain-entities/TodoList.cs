using System.ComponentModel.DataAnnotations;

namespace todo_domain_entities;

public class TodoList
{
    public int Id { get; set; }
    
    [Required]
    public string? Title { get; set; }

    public string? Description { get; set; }
    
    public bool IsHidden { get; set; }

    public List<TodoItem>? Items { get; set; }
}