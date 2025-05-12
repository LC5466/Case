using Case_backend.Services;

var builder = WebApplication.CreateBuilder(args);

// Legg til kontroller-støtte (nødvendig for API-controllerne dine)
builder.Services.AddControllers();

// Registrer FirmaService som HttpClient-tjeneste
builder.Services.AddHttpClient<FirmaService>();

// Legg til Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5285") // PORT = frontend-port
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Bruk Swagger i utviklingsmodus
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Tving HTTPS
app.UseHttpsRedirection();

app.UseCors();

// Autorisasjon (kan fjernes hvis du ikke bruker det)
app.UseAuthorization();

// Viktig: Map controller-endepunkter
app.MapControllers();

app.Run();