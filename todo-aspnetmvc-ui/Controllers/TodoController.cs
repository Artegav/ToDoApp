using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using todo_domain_entities;
using todo_domain_entities.Data;
using todo_domain_entities.Services;

namespace todo_aspnetmvc_ui.Controllers
{
    public class TodoController : Controller
    {
        private readonly TodoContext _context;
        private IListService _listService;

        public TodoController(TodoContext context, IListService listService)
        {
            _context = context;
            _listService = listService;
        }

        // GET: Todo
        [Route("/")]
        public async Task<IActionResult> Index()
        {
              return _context.TodoList != null ? 
                          View(await _listService.GetLists()) :
                          Problem("Entity set 'TodoContext.TodoList'  is null.");

        }

        // GET: Todo/Details/5
        [Route("/Todo/Details/{id:int}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TodoList == null)
            {
                return NotFound();
            }

            var todoList = await _context.TodoList
                .FirstOrDefaultAsync(m => m.Id == id);
            if (todoList == null)
            {
                return NotFound();
            }

            return View(todoList);
        }

        [Route("/Todo/Create")]
        // GET: Todo/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Todo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("/Todo/Create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<TodoList>> Create(TodoList todoList)
        {
            if (!ModelState.IsValid) 
                return View(todoList);

            await _listService.AddList(todoList);
            return RedirectToAction(nameof(Index));
        }

        // GET: Todo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TodoList == null)
            {
                return NotFound();
            }

            var todoList = await _context.TodoList.FindAsync(id);
            if (todoList == null)
            {
                return NotFound();
            }
            return View(todoList);
        }

        // POST: Todo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,IsHidden")] TodoList todoList)
        {
            if (id != todoList.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(todoList);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TodoListExists(todoList.Id))
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
            return View(todoList);
        }

        // GET: Todo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TodoList == null)
            {
                return NotFound();
            }

            var todoList = await _context.TodoList
                .FirstOrDefaultAsync(m => m.Id == id);
            if (todoList == null)
            {
                return NotFound();
            }

            return View(todoList);
        }

        // POST: Todo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TodoList == null)
            {
                return Problem("Entity set 'TodoContext.TodoList'  is null.");
            }
            var todoList = await _context.TodoList.FindAsync(id);
            if (todoList != null)
            {
                _context.TodoList.Remove(todoList);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TodoListExists(int id)
        {
          return (_context.TodoList?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
