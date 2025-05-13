using Case_backend.Services;

var builder = WebApplication.CreateBuilder(args);

//legg til kontroller-støtte (nødvendig for API-controllerne dine)
builder.Services.AddControllers();

//registrer FirmaService som HttpClient-tjeneste
builder.Services.AddHttpClient<FirmaService>();

//legg til Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5285") //frontend-port NB: ha "http" og ikke "https" pga Blazor og Cors
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

//sruk Swagger i utviklingsmodus
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//tving HTTPS
app.UseHttpsRedirection();

//IKKE FLYTT
app.UseCors();

//autorisasjon (kan fjernes hvis du ikke bruker det)
app.UseAuthorization();

//kontrollér endepunkter
app.MapControllers();

app.Run();