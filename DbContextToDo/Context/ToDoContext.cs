using Microsoft.EntityFrameworkCore;
using DbContextToDo.Entities;

namespace DbContextToDo.DataBase
{
    public class ToDoContext : DbContext
    {
        public ToDoContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<Todo> Todos => Set<Todo>();
        public DbSet<Category> Categories { get; set; } //=> Set<Category>();
            
        
        
    }
}
