using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using todo_domain_entities;
using todo_domain_entities.Services;

namespace todo_aspnetmvc_ui.Controllers
{
    public class ItemController : Controller
    {
        private readonly IItemService _itemService;
        private readonly IListService _listService;

        public ItemController(IItemService itemService, IListService listService)
        {
            _itemService = itemService;
            _listService = listService;
        }

        // GET: Item
        [HttpGet("/Item")]
        public async Task<IActionResult> Index(int? listId)
        {
            if(listId.HasValue)
                return View(await _itemService.GetItemsByListId(listId.Value));
            
            var items = await _itemService.GetItems();
            
            return View(items);
        }
        

        // GET: Item/Details/5
        [HttpGet("/Item/Details/{id:int}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();
            
            var todoItem = await _itemService.GetItemById(id.Value);
            return View(todoItem);
        }

        // GET: Item/Create
        [HttpGet("/Item/Create")]
        public async Task<IActionResult> Create()
        {
            var lists = await _listService.GetLists();
            ViewData["ToDoListId"] = new SelectList(lists, "Id", "Title");
            return View();
        }

        // POST: Item/Create
        [HttpPost("/Item/Create")]
        public async Task<IActionResult> Create(TodoItem todoItem)
        {
            await _itemService.AddItem(todoItem);
            return RedirectToAction(nameof(Index));
        }

        // GET: Item/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var todoItem = await _itemService.GetItemById(id.Value);
            var lists = await _listService.GetLists();
            ViewData["ToDoListId"] = new SelectList(lists, "Id", "Title", todoItem.ToDoListId);
            return View(todoItem);
        }

        // POST: Item/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(int id, TodoItem todoItem)
        {
            if (id != todoItem.Id)
            {
                return NotFound();
            }

            try
            {
                await _itemService.UpdateItem(todoItem);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_itemService.TodoItemExists(todoItem.Id))
                {
                    return NotFound();
                }

                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Item/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var todoItem = await _itemService.GetItemById(id.Value);

            return View(todoItem);
        }

        // POST: Item/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _itemService.DeleteItem(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
