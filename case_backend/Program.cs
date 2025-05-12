using Case.Services;

var builder = WebApplication.CreateBuilder(args);

// Legg til kontroller-støtte (nødvendig for API-controllerne dine)
builder.Services.AddControllers();

// Registrer FirmaService som HttpClient-tjeneste
builder.Services.AddHttpClient<FirmaService>();

// Legg til Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Bruk Swagger i utviklingsmodus
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Tving HTTPS
app.UseHttpsRedirection();

// Autorisasjon (kan fjernes hvis du ikke bruker det)
app.UseAuthorization();

// Viktig: Map controller-endepunkter
app.MapControllers();

app.Run();