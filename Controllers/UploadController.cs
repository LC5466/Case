using Case.Models;
using Case.Services;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Case.Controllers;

[ApiController]
[Route("api/upload")]
public class UploadController : ControllerBase
{
    private readonly FirmaService _firmaService;

    public UploadController(FirmaService firmaService)
    {
        _firmaService = firmaService;
    }

    [HttpPost] //POST responsen
    public async Task<IActionResult> UploadCsv(IFormFile file)
    {
        //variabler
        var firmaer = new List<FirmaInfo>();
        var output_FileName = "firmaer_output.csv";
        bool isFirstLine = true;

        using var stream = file.OpenReadStream();
        using var reader = new StreamReader(stream);
        using var writer = new StreamWriter(output_FileName);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";" //delimiter til csv-filene (innebygd variabelnavn til CsvHelper)
        });

        if (file == null || file.Length == 0)
            return BadRequest("Ingen fil lastet opp.");

        while (await csv.ReadAsync())
        {
            //hvis data inneholder tittel, hopp over
            if (isFirstLine)
            {
                isFirstLine = false;
                continue;
            }

            //må bytte om på rekkefølgen, da dataene er feil rekkefølge i den originale fila
            var firmaNavn = csv.GetField(0)?.Trim();
            var orgNo = csv.GetField(1)?.Trim();

            var info = await _firmaService.HentFirmaInfoAsync(orgNo, firmaNavn);
            firmaer.Add(info);
        }

        //lagre til CSV med de nødvendige kriteriene i henhold til oppgaven
        await writer.WriteLineAsync("OrgNo;FirmaNavn;Status;AntallAnsatte;OrganisasjonsformKode;Naeringskode"); //nye kolonne-headere

        foreach (var firma in firmaer)
        {
            await writer.WriteLineAsync(
                $"{firma.OrgNo};{firma.FirmaNavn};{firma.Status};{firma.AntallAnsatte};{firma.OrganisasjonsformKode};{firma.Naeringskode}");
        }

        return Ok(new { message = "Filen er ferdig behandlet", output = output_FileName });
    }
}