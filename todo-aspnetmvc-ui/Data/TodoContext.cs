using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using todo_domain_entities;

namespace todo_aspnetmvc_ui.Data
{
    public class TodoContext : DbContext
    {
        public TodoContext (DbContextOptions<TodoContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<TodoList> TodoList { get; set; } = null!;
        public DbSet<TodoItem> TodoItems { get; set; } = null!;
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TodoItem>()
                .HasOne(e => e.TodoList)
                .WithMany(e => e.Items)
                .HasForeignKey(e => e.ToDoListId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
