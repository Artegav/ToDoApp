namespace todo_domain_entities.Services;

public interface IListService
{
    public Task<IEnumerable<TodoList>> GetLists();
    public Task<TodoList> GetListById(int id);
    public Task AddList(TodoList list);
    public Task UpdateList(TodoList list);
    public Task UpdateRangeOfLists(IEnumerable<TodoList> lists);
    public Task DeleteList(int id);
    public Task CopyList(int id);
    public bool TodoListExists(int id);
}