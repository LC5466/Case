using Case_backend.Services;

var builder = WebApplication.CreateBuilder(args);

//legg til kontroller-støtte (nødvendig for API-controllerne dine)
builder.Services.AddControllers();

//registrer FirmaService som HttpClient-tjeneste
builder.Services.AddHttpClient<FirmaService>();

//legg til Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//sruk Swagger i utviklingsmodus
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//tving HTTPS
app.UseHttpsRedirection();

//kontrollér endepunkter
app.MapControllers();

app.Run();