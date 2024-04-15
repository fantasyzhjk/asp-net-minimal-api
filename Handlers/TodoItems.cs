using LinqToDB;

public static class TodoItems
{
    public static RouteGroupBuilder Reg(IEndpointRouteBuilder app)
    {
        var todoItems = app.MapGroup("/todoitems");

        todoItems.MapGet("/", GetAllTodos);
        todoItems.MapGet("/complete", GetCompleteTodos);
        todoItems.MapGet("/{id}", GetTodo);
        todoItems.MapPost("/", CreateTodo);
        todoItems.MapPut("/{id}", UpdateTodo);
        todoItems.MapDelete("/{id}", DeleteTodo);

        return todoItems;
    }

    static async Task<IResult> GetAllTodos(Db db)
    {
        return JsonResult.Ok(await db.Todos.ToArrayAsync());
    }

    static async Task<IResult> GetCompleteTodos(Db db)
    {
        return JsonResult.Ok(await db.Todos.Where(t => t.IsComplete).ToListAsync());
    }

    static async Task<IResult> GetTodo(int id, Db db)
    {
        return await db.Todos.SingleOrDefaultAsync(v => v.Id == id) is Models.Todo todo
            ? JsonResult.Ok(todo)
            : TypedResults.NotFound();
    }

    static async Task<IResult> CreateTodo(Models.Todo todo, Db db)
    {
        await db.InsertAsync(todo);

        return TypedResults.Created($"/todoitems/{todo.Id}", todo);
    }

    static async Task<IResult> UpdateTodo(int id, Models.Todo inputTodo, Db db)
    {
        var todo = await db.Todos.SingleOrDefaultAsync(v => v.Id == id);

        if (todo is null)
            return TypedResults.NotFound();

        todo.Name = inputTodo.Name;
        todo.IsComplete = inputTodo.IsComplete;

        await db.UpdateAsync(todo);

        return TypedResults.NoContent();
    }

    static async Task<IResult> DeleteTodo(int id, Db db)
    {
        if (await db.Todos.Where(v => v.Id == id).DeleteAsync() < 1)
        {
            return TypedResults.NotFound();
        }
        return TypedResults.NoContent();
    }
}
