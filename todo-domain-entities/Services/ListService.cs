

using Microsoft.EntityFrameworkCore;
using todo_domain_entities.Data;

namespace todo_domain_entities.Services;

public class ListService : IListService
{
    private readonly TodoContext _context;

    public ListService(TodoContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<TodoList>> GetLists()
    {
        return await _context.TodoList.ToListAsync();
    }
    
    public async Task<TodoList?> GetListById(int id)
    {
        return await _context.TodoList.FirstOrDefaultAsync(td => td.Id == id);
    }
    
    public async Task AddList(TodoList list)
    {
        _context.TodoList.Add(list);
        await _context.SaveChangesAsync();
    }
    
    public async Task<TodoList> UpdateList(TodoList list)
    {
        throw new NotImplementedException();
    }
    
    public async Task<TodoList> DeleteList(int? id)
    {
        throw new NotImplementedException();
    }
}