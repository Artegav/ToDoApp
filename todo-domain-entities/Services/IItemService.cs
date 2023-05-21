namespace todo_domain_entities.Services;

public interface IItemService
{
    public Task<IEnumerable<TodoItem>> GetItems();
    public Task<TodoItem> GetItemById(int id);
    public Task<TodoItem> AddItem(TodoItem item);
    public Task<TodoItem> UpdateItem(TodoItem item);
    public Task<TodoItem> DeleteItem(int? id);
}