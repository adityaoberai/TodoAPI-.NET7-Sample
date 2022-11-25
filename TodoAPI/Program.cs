using Microsoft.EntityFrameworkCore;
using TodoAPI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<TodoDb>(options => options.UseInMemoryDatabase("Todos"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/todo", async(TodoDb db) =>
{
    var todoList = await db.Todos.ToListAsync();
    return Results.Ok(todoList);
})
.WithName("Get All Todos")
.WithOpenApi();

app.MapGet("/todo/{id}", async (int id, TodoDb db) =>
{
    var todo = await db.Todos.FindAsync(id);
    if(todo is null)
    {
        return Results.NotFound();
    }
    else 
    {
        return Results.Ok(todo);
    }
})
.WithName("Get Todo By Id")
.WithOpenApi();

app.MapPost("/todo", async (TodoDb db, Todo todo) =>
{
    db.Todos.Add(todo);
    await db.SaveChangesAsync();
    return Results.Created($"/todo/{todo.Id}", todo);
})
.WithName("Add Todo")
.WithOpenApi();

app.MapPut("/todoitems/{id}", async (int id, Todo inputTodo, TodoDb db) =>
{
    var todo = await db.Todos.FindAsync(id);

    if (todo is null)
    {
        return Results.NotFound();
    }

    todo.Name = inputTodo.Name;
    todo.IsComplete = inputTodo.IsComplete;

    db.Todos.Update(todo);
    await db.SaveChangesAsync();

    return Results.NoContent();
})
.WithName("Update Todo")
.WithOpenApi();

app.MapDelete("/todoitems/{id}", async (int id, TodoDb db) =>
{
    var todo = await db.Todos.FindAsync(id);

    if (todo is not null)
    {
        db.Todos.Remove(todo);
        await db.SaveChangesAsync();
        return Results.Ok(todo);
    }

    return Results.NotFound();
})
.WithName("Delete Todo")
.WithOpenApi();

app.Run();