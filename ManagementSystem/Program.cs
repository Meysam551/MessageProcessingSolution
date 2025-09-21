var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();

// Add Swagger/OpenAPI support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Enable XML comments for Swagger
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
});

var app = builder.Build();

// Enable Swagger UI in all environments (optional)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Management API v1");
    c.RoutePrefix = string.Empty; // Swagger available at root: https://localhost:5001/
});

// Middleware
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
