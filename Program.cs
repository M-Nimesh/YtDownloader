using YouTubeDownloaderApi.Services;
using YouTubeDownloaderApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<YouTubeService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Environment.WebRootPath = "CustomPathToStaticFiles";

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
}

app.UseHttpsRedirection();

app.Map("/error", (HttpContext context, IHostEnvironment env) =>
{
    var exceptionHandlerFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
    var error = exceptionHandlerFeature?.Error;

    app.Logger.LogError(error, "An unhandled error occurred.");

    return Results.Problem(
        title: "An unexpected error occurred.",
        statusCode: StatusCodes.Status500InternalServerError,
        detail: env.IsDevelopment() ? error?.StackTrace : null
    );
});


app.MapGet("/youtube/info", async (
    [FromServices] YouTubeDLService youtubeDlService,
    [FromServices] IHostEnvironment env,
    [FromQuery] string videoUrl) =>
{
    if (string.IsNullOrWhiteSpace(videoUrl))
    {
        return Results.BadRequest(new { Error = "YouTube video URL is required." });
    }

    var videoInfo = await youtubeDlService.GetYouTubeVideoInfo(videoUrl, env.IsDevelopment());

    if (videoInfo.Error != null)
    {
        return Results.Problem(
            title: "Failed to retrieve video information from YouTube.",
            statusCode: StatusCodes.Status500InternalServerError,
            detail: videoInfo.Error,
            extensions: new Dictionary<string, object?>
            {
                { "stackTrace", videoInfo.StackTrace }
            }
        );
    }

    return Results.Ok(videoInfo);

})
.WithName("GetYouTubeVideoInfo")
.WithOpenApi();

app.Run();

// Ensure YouTubeService is defined in the project
public class YouTubeService
{
    // Implementation of YouTubeService
    public string GetVideoInfo(string videoUrl)
    {
        // Example method implementation
        return $"Info for video: {videoUrl}";
    }
}