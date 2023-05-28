namespace todo_domain_entities.Services;

public interface IItemService
{
    public Task<IEnumerable<TodoItem>> GetItems();
    public Task<TodoItem> GetItemById(int id);
    public Task AddItem(TodoItem item);
    public Task UpdateItem(TodoItem item);
    public Task DeleteItem(int id);
    public Task<List<TodoItem>> GetItemsByListId(int id);
    bool TodoItemExists(int id);
}