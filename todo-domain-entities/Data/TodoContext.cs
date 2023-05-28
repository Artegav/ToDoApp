using Microsoft.EntityFrameworkCore;


namespace todo_domain_entities.Data
{
    public class TodoContext : DbContext
    {
        public TodoContext()
        {
                
        }
        public TodoContext (DbContextOptions<TodoContext> options)
            : base(options)
        {
            // Database.EnsureCreated();
        }

        public DbSet<TodoList> TodoList { get; set; }

        public DbSet<TodoItem> TodoItems { get; set; }
        
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
