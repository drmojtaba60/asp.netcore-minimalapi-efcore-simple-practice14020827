using DbContextToDo.DataBase;
using DbContextToDo.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<ToDoContext>(options=>
{
    options.UseInMemoryDatabase(databaseName:"ToDos");

    
});

builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGet("health-api", async () => await Task.FromResult("ok"));
app.MapGet("todos", async (ToDoContext context) =>
{
    SeedDataForTest(context);
    return  await context.Todos.AsNoTracking().ToListAsync();
});
app.MapGet("todos/{id}", async ([FromRoute] int id, ToDoContext context) => Results.Ok( await context.Todos.AsNoTracking().FirstOrDefaultAsync(q => q.Id == id)));
app.MapPost("todos", async (ToDoContext context, [FromBody] Todo toDo)=>
{
    await context.Todos.AddAsync(toDo);
    await context.SaveChangesAsync();
    return Results.Created($"todos/{toDo.Id}",toDo);
});

app.MapPut("todos/{id}", async (ToDoContext context,[FromRoute]int id, [FromBody] Todo toDo)  
    =>
{
    var entity = await context.Todos.FindAsync(toDo.Id);
    if (entity is null) return Results.NotFound();
    entity.Name = toDo.Name;
    entity.IsComplete = false;
    await context.SaveChangesAsync();
    return Results.NoContent();
});


app.MapDelete("todos/{id}", async (ToDoContext context, [FromRoute] int id) =>
{
   if(!await context.Todos.AnyAsync(q=>q.Id==id))
        return Results.NotFound();
    var entity = await context.Todos.FindAsync(id);
    context.Todos.Remove(entity);
    await context.SaveChangesAsync();
    return Results.NoContent() ;

});

app.MapPut("todos/{id}/set-complete", async (int id, ToDoContext context) =>
{
    var entity = await context.Todos.FindAsync(id);
    if (entity is null) return Results.NotFound();
    entity.IsComplete = true;
    await context.SaveChangesAsync();
    return Results.NoContent();
});
app.MapPut("todos/{id}/set-uncomplete", async (int id, ToDoContext context) =>
{
    var entity = await context.Todos.FindAsync(id);
    if (entity is null) return Results.NotFound();
    entity.IsComplete = false;
    await context.SaveChangesAsync();
    return Results.NoContent();
});

 void SeedDataForTest(ToDoContext context)
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
app.Run();
