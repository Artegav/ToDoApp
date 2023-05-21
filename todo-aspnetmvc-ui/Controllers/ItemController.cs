using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using todo_domain_entities;
using todo_domain_entities.Data;

namespace todo_aspnetmvc_ui.Controllers
{
    public class ItemController : Controller
    {
        private readonly TodoContext _context;

        public ItemController(TodoContext context)
        {
            _context = context;
        }

        // GET: Item
        [Route("/Item")]
        public async Task<ActionResult<TodoItem>> Index()
        {
            var todoContext = _context.TodoItems.Include(t => t.TodoList);
            return View(await todoContext.ToListAsync());
        }

        // GET: Item/Details/5
        [HttpGet("/Item/Details/{id:int}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TodoItems == null)
            {
                return NotFound();
            }

            var todoItem = await _context.TodoItems
                .Include(t => t.TodoList)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (todoItem == null)
            {
                return NotFound();
            }

            return View(todoItem);
        }

        // GET: Item/Create
        [Route("/Item/Create")]
        public ActionResult<TodoItem> Create()
        {
            ViewData["ToDoListId"] = new SelectList(_context.TodoList, "Id", "Title");
            return View();
        }

        // POST: Item/Create
        [HttpPost("/Item/Create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<TodoItem>> Create(TodoItem todoItem)
        {
            if (!ModelState.IsValid)
            {
                ViewData["ToDoListId"] = new SelectList(_context.TodoList, "Id", "Title", todoItem.ToDoListId);
                return View(todoItem);
            }

            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Item/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TodoItems == null)
            {
                return NotFound();
            }

            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }
            ViewData["ToDoListId"] = new SelectList(_context.TodoList, "Id", "Title", todoItem.ToDoListId);
            return View(todoItem);
        }

        // POST: Item/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Status,IsHidden,CreationDate,DueDate,ToDoListId")] TodoItem todoItem)
        {
            if (id != todoItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(todoItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TodoItemExists(todoItem.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ToDoListId"] = new SelectList(_context.TodoList, "Id", "Title", todoItem.ToDoListId);
            return View(todoItem);
        }

        // GET: Item/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TodoItems == null)
            {
                return NotFound();
            }

            var todoItem = await _context.TodoItems
                .Include(t => t.TodoList)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (todoItem == null)
            {
                return NotFound();
            }

            return View(todoItem);
        }

        // POST: Item/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TodoItems == null)
            {
                return Problem("Entity set 'TodoContext.TodoItems'  is null.");
            }
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem != null)
            {
                _context.TodoItems.Remove(todoItem);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TodoItemExists(int id)
        {
          return (_context.TodoItems?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
