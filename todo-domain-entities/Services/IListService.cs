namespace todo_domain_entities.Services;

public interface IListService
{
    public Task<IEnumerable<TodoList>> GetLists();
    public Task<TodoList?> GetListById(int id);
    public Task AddList(TodoList list);
    public  Task<TodoList> UpdateList(TodoList list);
    public Task<TodoList> DeleteList(int? id);
}