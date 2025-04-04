using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class TasksEndpoints
{
    public static void MapEndpoints(WebApplication app)
    {
        app.MapPost("/tasks/add", async (TaskDb db, Task task) =>
        {
            try
            {

                if (task is null || string.IsNullOrWhiteSpace(task.Name))
                {
                    return Results.BadRequest("The task must have a name.");
                }

                task.Id = Guid.NewGuid(); 
                await db.Tasks.AddAsync(task);
                await db.SaveChangesAsync();
                return Results.Created($"/tasks/{task.Id}", task);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex.Message}");
                return Results.Problem("An internal server error occurred. Please try again later.");
            }
        });

        app.MapGet("/tasks", async (TaskDb db) =>
        {
            try
            {
                var tasks = await db.Tasks.ToListAsync();
                return Results.Ok(tasks);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex.Message}");
                return Results.Problem("An internal server error occurred. Please try again later.");
            }
        });

        app.MapGet("/task/{id:guid}", async (TaskDb db, Guid id) =>
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return Results.BadRequest("The task ID is invalid.");
                }

                var task = await db.Tasks.FindAsync(id);
                if (task is null)
                {
                    return Results.NotFound("Task not found.");
                }
                return Results.Ok(task);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex.Message}");
                return Results.Problem("An internal server error occurred. Please try again later.");
            }
        });

        app.MapPut("/task/{id:guid}", async (TaskDb db, Guid id, [FromBody] Task task) => {
            try {
                if (task is null || string.IsNullOrWhiteSpace(task.Name))
                {
                    return Results.BadRequest("The task must have a name.");
                }

                var ExistingTask = await db.Tasks.FindAsync(id);
                if( ExistingTask is null)
                {
                    return Results.NotFound("Task not found.");
                }

                ExistingTask.Id = id;
                ExistingTask.Name = task.Name;
                ExistingTask.IsCompleted = task.IsCompleted;

                await db.SaveChangesAsync();
                return Results.Ok(ExistingTask);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex.Message}");
                return Results.Problem("An internal server error occurred. Please try again later.");
            }
        });

        app.MapDelete("/task/{id:guid}", async (TaskDb db, Guid id) => {
            try {
            
                var task = await db.Tasks.FindAsync(id);
                if(task is null)
                {
                    return Results.NotFound("Task not found.");
                }

                db.Tasks.Remove(task);
                await db.SaveChangesAsync();
                return Results.NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex.Message}");
                return Results.Problem("An internal server error occurred. Please try again later.");
            }
        });
    }
}