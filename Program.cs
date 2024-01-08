var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddCors(options =>
{
    options.AddPolicy("localhost", builder =>
    {
        builder.WithOrigins("http://localhost:4200")
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});

builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseRouting();
app.UseAuthorization();


//app.UseEndpoints(endpoints =>
//{
//    // Specify your custom endpoint here
//    endpoints.MapControllerRoute(
//        name: "default",
//        pattern: "{controller=HackerController}/{action=GetStories}/{id?}");
//});
app.UseCors("localhost");
app.MapControllers();

app.Run();
