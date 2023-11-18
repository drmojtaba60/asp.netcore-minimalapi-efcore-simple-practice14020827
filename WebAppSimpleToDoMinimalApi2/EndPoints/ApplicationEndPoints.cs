using DbContextToDo.DataBase;
using DbContextToDo.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebAppSimpleToDoMinimalApi2.ApplicationEndPoints
{
    public static class ApplicationEndPoints
    {
        private static void SeedDataForTest(ToDoContext context)
        {
            if(! context.Categories.Any())
            {
           
                context.Categories.AddRange(
                    new Category { Id = 1, Name = "personal" },
                    new Category { Id = 2, Name = "buy" },
                    new Category { Id = 3, Name = "sport" },
                    new Category { Id = 4, Name = "study" }

                );
                context.SaveChanges();
            }
            if(!context.Todos.Any()) {
                context.Todos.AddRange(
                    new Todo { CategoryId = 1, Name = "clean my table",IsComplete=true },
                    new Todo { Id = 2, CategoryId = 2, Name = "buy bread for breakfast" },
                    new Todo { Id = 3, CategoryId = 2, Name = "buy some coffee" },
                    new Todo { Id = 4, CategoryId = 3, Name = "sport for 30 minutes in evening" },
                    new Todo { Id=100, CategoryId = 2, Name = "خرید نان سهمیه شنبه" },
                    new Todo { Id = 5, CategoryId = 4, Name = "study programming c#",IsComplete = true });
                context.SaveChanges();
            }
     
        }
        public static void SetCustomMapping(this WebApplication app)
        {
            app.MapGet("health-api", async () => await Task.FromResult("ok"));
            app.MapGet("todos", GetToDosAsync);
            app.MapGet("todos/{id}", GetByIdAsync);
            app.MapPost("todos", AddAsync);
            app.MapPut("todos/{id}", UpdateAsync);

            app.MapDelete("todos/{id}", DeleteAsync);
            app.MapPut("todos/{id}/set-complete", SetComplete);
            app.MapPut("todos/{id}/set-uncomplete", SetUnComplete);
        }

        public static void AddAppEndpointDependencyServices(this IServiceCollection services)
        {
            services.AddDbContext<ToDoContext>(options =>
            {
                options.UseInMemoryDatabase(databaseName: "ToDos");
            });
        }

        static async Task<IResult> GetToDosAsync(ToDoContext context)
        {
            SeedDataForTest(context);
            return Results.Ok(await context.Todos.AsNoTracking().ToListAsync());
        }
        static async Task<IResult> GetByIdAsync([FromRoute] int id, ToDoContext context) => Results.Ok(await context.Todos.AsNoTracking().FirstOrDefaultAsync(q => q.Id == id));

      static async Task<IResult> AddAsync([FromBody] Todo toDo, ToDoContext context)
        {
            await context.Todos.AddAsync(toDo);
            await context.SaveChangesAsync();
            return Results.Created($"todos/{toDo.Id}", toDo);
        }
        static async Task<IResult> UpdateAsync([FromRoute] int id, [FromBody] Todo toDo, ToDoContext context)
        {
            var entity = await context.Todos.FindAsync(toDo.Id);
            if (entity is null) return Results.NotFound();
            entity.Name = toDo.Name;
            entity.IsComplete = false;
            await context.SaveChangesAsync();
            return Results.NoContent();
        }
        static async Task<IResult> DeleteAsync(ToDoContext context, [FromRoute] int id)
        {
            if (!await context.Todos.AnyAsync(q => q.Id == id))
                return Results.NotFound();
            var entity = await context.Todos.FindAsync(id);
            context.Todos.Remove(entity);
            await context.SaveChangesAsync();
            return Results.NoContent();

        }
        static async Task<IResult> SetComplete(int id, ToDoContext context)
        {
            var entity = await context.Todos.FindAsync(id);
            if (entity is null) return Results.NotFound();
            entity.IsComplete = true;
            await context.SaveChangesAsync();
            return Results.NoContent();
        }
        static async Task<IResult> SetUnComplete(int id, ToDoContext context)
        {
            var entity = await context.Todos.FindAsync(id);
            if (entity is null) return Results.NotFound();
            entity.IsComplete = false;
            await context.SaveChangesAsync();
            return Results.NoContent();
        }
    }
    
    
}
