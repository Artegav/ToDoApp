using Microsoft.EntityFrameworkCore;
using todo_domain_entities.Data;

namespace todo_domain_entities.Services;

public class ItemService : IItemService
{
    private readonly TodoContext _context;

    public ItemService(TodoContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    public async Task<TodoItem> GetItemById(int id)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(nameof(id), "Id must be greater than zero.");
        
        return await _context.TodoItems
            .Include(t => t.TodoList)
            .FirstOrDefaultAsync(m => m.Id == id) ?? throw new InvalidOperationException("No items found.");
    }
    
    public async Task<IEnumerable<TodoItem>> GetItems()
    {
        return await _context.TodoItems.ToListAsync() ?? throw new InvalidOperationException("No items found.");
    }
    
    public async Task AddItem(TodoItem item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item),"The Todo item cannot be null.");
        }

        var state = item.Status;
        if (state == State.Completed)
            item.IsCompleted = true;
        else
            item.IsCompleted = false;
        
        _context.TodoItems.Add(item);
        await _context.SaveChangesAsync();
    }
    
    public async Task UpdateItem(TodoItem item)
    {
        if (item.Id <= 0)
            throw new InvalidOperationException("Id must be greater than zero.");
        
        var state = item.Status;
        if (state == State.Completed)
            item.IsCompleted = true;
        else
            item.IsCompleted = false;
        
        _context.Update(item);
        await _context.SaveChangesAsync().ConfigureAwait(false);
        
    }
    
    public async Task DeleteItem(int id)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(nameof(id), "Id must be greater than zero.");
        
        var todoItem = await GetItemById(id);
        _context.TodoItems.Remove(todoItem);
        await _context.SaveChangesAsync();
    }
    
    public async Task<List<TodoItem>> GetItemsByListId(int id)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(nameof(id), "Id must be greater than zero.");
        
        var items = _context.TodoItems.AsQueryable();
        return await items.Where(i => i.ToDoListId == id).ToListAsync();
    }
    
    public bool TodoItemExists(int id)
    {
        return (_context.TodoItems?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}