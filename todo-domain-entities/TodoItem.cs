using System.ComponentModel.DataAnnotations;

namespace todo_domain_entities;

public class TodoItem
{
    public int Id { get; set; }
    
    [Required]
    public string? Title { get; set; }
    
    public string? Description { get; set; }
    
    public State? Status { get; set; }
    
    public bool? IsHidden { get; set; }
    
    public DateTime? CreationDate { get; set; }
    
    public DateTime? DueDate { get; set; }

    public int ToDoListId { get; set; }

    public TodoList? TodoList { get; set; }
}

public enum State
{
    NotStarted,
    InProgress,
    Completed
}