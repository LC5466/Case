using Case.Models;
using Case.Services;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Case.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FirmaController : ControllerBase
{
    private readonly FirmaService _firmaService;
    private readonly IWebHostEnvironment _env;

    public FirmaController(FirmaService firmaService, IWebHostEnvironment env)
    {
        _firmaService = firmaService;
        _env = env;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadCsv(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Ingen fil lastet opp.");

        var firmaer = new List<FirmaInfo>();

        using var stream = file.OpenReadStream();
        using var reader = new StreamReader(stream);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";",
            HasHeaderRecord = false
        });

        bool isFirstLine = true;
        while (await csv.ReadAsync())
        {
            if (isFirstLine)
            {
                isFirstLine = false;
                continue; // hopp over header
            }

            //må bytte om på rekkefølgen, da dataene er feil rekkefølge
            var firmaNavn = csv.GetField(0)?.Trim();
            var orgNo = csv.GetField(1)?.Trim();

            var info = await _firmaService.HentFirmaInfoAsync(orgNo, firmaNavn);
            firmaer.Add(info);
        }

        // Lagre til CSV
        var outputPath = Path.Combine(_env.WebRootPath ?? _env.ContentRootPath, "firmaer_output.csv");
        using var writer = new StreamWriter(outputPath);
        await writer.WriteLineAsync("OrgNo;FirmaNavn;Status;AntallAnsatte;OrganisasjonsformKode;Naeringskode");

        foreach (var firma in firmaer)
        {
            await writer.WriteLineAsync(
                $"{firma.OrgNo};{firma.FirmaNavn};{firma.Status};{firma.AntallAnsatte};{firma.OrganisasjonsformKode};{firma.Naeringskode}");
        }

        return Ok(new { message = "Filen ble behandlet", output = outputPath });
    }
}
