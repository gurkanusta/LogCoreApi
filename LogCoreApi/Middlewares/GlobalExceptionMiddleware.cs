using System.Net; 
using Microsoft.AspNetCore.Mvc; 
using Serilog; 
using LogCoreApi.Exceptions; 

namespace LogCoreApi.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next; 

    private readonly IHostEnvironment _env;  

    public GlobalExceptionMiddleware(RequestDelegate next, IHostEnvironment env)
    {
        _next = next; 
        _env = env;   
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context); 
        }
        catch (Exception ex)
        {
            
            Log.Error(ex, "Unhandled exception. Path: {Path}", context.Request.Path);


            
            context.Response.ContentType = "application/json";

            
            var (statusCode, problem) = CreateProblemDetails(context, ex);

            

            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(problem);
        }
    }

    private (int StatusCode, ProblemDetails Problem) CreateProblemDetails(HttpContext context, Exception ex)
    {
        
        if (ex is NotFoundException)
        {
            var p = new ProblemDetails
            {

                Title = "Not Found", 

                Status = (int)HttpStatusCode.NotFound, 
                Detail = ex.Message, 
                Instance = context.Request.Path
                
            };

            return (p.Status.Value, p);
        }

        
        if (ex is ValidationException vex)
        {
            var vp = new ValidationProblemDetails(vex.Errors)
            {
                Title = "Validation Error", 
                Status = (int)HttpStatusCode.BadRequest, 
                Detail = vex.Message, 
                Instance = context.Request.Path 
            };

            return (vp.Status.Value, vp);
        }

        
        var detail = _env.IsDevelopment()
            ? ex.Message 
            : "An unexpected error occurred."; 

        var problem = new ProblemDetails
        {


            Title = "Server Error", 
            Status = (int)HttpStatusCode.InternalServerError, 
            Detail = detail, 

            Instance = context.Request.Path 
        };


        return (problem.Status.Value, problem);


    }
}
