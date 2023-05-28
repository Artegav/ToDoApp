using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using todo_domain_entities;
using todo_domain_entities.Services;

namespace todo_aspnetmvc_ui.Controllers
{
    public class TodoController : Controller
    {
        private readonly IListService _listService;
        private readonly IItemService _itemService;

        public TodoController(IListService listService, IItemService itemService)
        {
            _listService = listService;
            _itemService = itemService;
        }

        // GET:
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var lists = await _listService.GetLists();
            foreach (var list in lists)
            {
                list.Items = await _itemService.GetItemsByListId(list.Id);
            }
            
            return View(lists);
        }

        // GET: Todo/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();
            
            var todoList = await _listService.GetListById(id.Value);

            return View(todoList);
        }

        // GET: Todo/Create
        [HttpGet]
        public IActionResult Create() => View();

        // POST: Todo/Create
        [HttpPost]
        public async Task<IActionResult> Create(TodoList todoList)
        {
            if (!ModelState.IsValid) 
                return View(todoList);

            await _listService.AddList(todoList);
            return RedirectToAction(nameof(Index));
        }

        // GET: Todo/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var todoList = await _listService.GetListById(id.Value);

            return View(todoList);
        }

        // POST: Todo/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(int id, TodoList todoList)
        {
            if (id != todoList.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(todoList);
            
            try
            {
                await _listService.UpdateList(todoList);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_listService.TodoListExists(todoList.Id))
                {
                    return NotFound();
                }
                
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Todo/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var todoList = await _listService.GetListById(id.Value);

            return View(todoList);
        }

        // POST: Todo/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            await _listService.DeleteList(id);
            return RedirectToAction(nameof(Index));
        }
        
        [HttpGet]
        public async Task<IActionResult> Copy(int id)
        {
            await _listService.CopyList(id);
            return RedirectToAction(nameof(Index));
        }
        
        [HttpGet]
        public async Task<IActionResult> Hide(int id)
        {
            var todoList = await _listService.GetListById(id);
            todoList.IsHidden = true;
            await _listService.UpdateList(todoList);

            return RedirectToAction(nameof(Index));
        }
        
        [HttpGet]
        public async Task<IActionResult> Show()
        {
            var lists = await _listService.GetLists();
            var hiddenLists = lists.Where(x => x.IsHidden = false);
            await _listService.UpdateRangeOfLists(hiddenLists);
            
            return RedirectToAction(nameof(Index));
        }
    }
}
