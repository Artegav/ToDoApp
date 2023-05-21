namespace todo_domain_entities.Services;

public class ItemService : IItemService
{
    public Task<TodoItem> AddItem(TodoItem item)
    {
        throw new NotImplementedException();
    }
    
    public Task<TodoItem> DeleteItem(int? id)
    {
        throw new NotImplementedException();
    }
    
    public Task<TodoItem> GetItemById(int id)
    {
        throw new NotImplementedException();
    }
    
    public Task<IEnumerable<TodoItem>> GetItems()
    {
        throw new NotImplementedException();
    }
    
    public Task<TodoItem> UpdateItem(TodoItem item)
    {
        throw new NotImplementedException();
    }
}