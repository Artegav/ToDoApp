namespace todo_domain_entities.ViewModels;

public class TodoItemViewModel
{
    public int ListId { get; set; }
    public List<TodoItem>? Items { get; set; }
}