using Microsoft.EntityFrameworkCore;
using todo_domain_entities.Data;

namespace todo_domain_entities.Services;

public class ListService : IListService
{
    private readonly TodoContext _context;

    public ListService(TodoContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    public async Task<IEnumerable<TodoList>> GetLists()
    {
        return await _context.TodoList.ToListAsync() ?? throw new InvalidOperationException("No lists found.");
    }
    
    public async Task<TodoList> GetListById(int id)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(nameof(id), "Id must be greater than zero.");
        
        return await _context.TodoList.Include(t => t.Items)
                   .FirstOrDefaultAsync(td => td.Id == id)
               ?? throw new InvalidOperationException("Entity not found.");
    }
    
    public async Task AddList(TodoList list)
    {
        if (list == null)
            throw new ArgumentNullException(nameof(list));

        _context.TodoList.Add(list);
        await _context.SaveChangesAsync();
    }
    
    public async Task UpdateList(TodoList list)
    {
        if (list.Id <= 0)
            throw new InvalidOperationException("Id must be greater than zero.");

        _context.TodoList.Update(list);
        await _context.SaveChangesAsync();
    }
    
    public async Task UpdateRangeOfLists(IEnumerable<TodoList> lists)
    {
        if (lists == null)
            throw new ArgumentNullException(nameof(lists));
        
        _context.TodoList.UpdateRange(lists);
        await _context.SaveChangesAsync();
    }
    
    public async Task DeleteList(int id)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(nameof(id), "Id must be greater than zero.");

        var todoList = await GetListById(id);
        _context.TodoList.Remove(todoList ?? throw new InvalidOperationException( $"Entity with id {id} not found. Cannot delete."));
        await _context.SaveChangesAsync();
    }

    public async Task CopyList(int id)
    {
        var existingList = _context.TodoList
            .Include(tl => tl.Items)
            .SingleOrDefault(tl => tl.Id == id);
        
        if (existingList == null)
        {
            throw new InvalidOperationException("The specified to-do list does not exist.");
        }
        
        var newList = new TodoList
        {
            Title = existingList.Title + " (Copied)",
            Description = existingList.Description,
            IsHidden = existingList.IsHidden,
            Items = new List<TodoItem>()
        };
        
        foreach (var item in existingList.Items)
        {
            var newItem = new TodoItem
            {
                Title = item.Title + " (Copied)",
                Description = item.Description,
                Status = item.Status,
                IsCompleted = item.IsCompleted,
                CreationDate = item.CreationDate,
                DueDate = item.DueDate
            };
    
            newList.Items.Add(newItem);
        }
        
        _context.TodoList.Add(newList);
        await _context.SaveChangesAsync();
    }
    
    public bool TodoListExists(int id)
    {
        return (_context.TodoList?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}