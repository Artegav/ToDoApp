using System.ComponentModel.DataAnnotations;

namespace todo_domain_entities;

public class TodoItem
{
    public int Id { get; set; }
    
    [Required]
    public string? Title { get; set; }
    
    public string? Description { get; set; }
    
    public State? Status { get; set; }
    
    public bool IsCompleted { get; set; }
    
    [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}")]
    public DateTime? CreationDate { get; set; }
    
    [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}")]
    public DateTime? DueDate { get; set; }

    public int ToDoListId { get; set; }

    public TodoList? TodoList { get; set; }
}