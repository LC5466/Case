using Case_backend.Models;
using Case_backend.Services;
using Case.Shared;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Case_backend.Controllers;

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
        var fileName = "firmaer_output.csv";
        using var writer = new StreamWriter(fileName);
        await writer.WriteLineAsync("OrgNo;FirmaNavn;Status;AntallAnsatte;OrganisasjonsformKode;Naeringskode");

        foreach (var firma in firmaer)
        {
            await writer.WriteLineAsync(
                $"{firma.OrgNo};{firma.FirmaNavn};{firma.Status};{firma.AntallAnsatte};{firma.OrganisasjonsformKode};{firma.Naeringskode}");
        }

        return Ok(new { message = "Filen ble behandlet", output = fileName });
    }

    [HttpGet("statistikk")]
    public IActionResult HentStatistikk()
    {
        var path = Path.Combine(_env.WebRootPath ?? _env.ContentRootPath, "firmaer_output.csv");

        if (!System.IO.File.Exists(path))
            return NotFound("Filen firmaer_output.csv finnes ikke.");

        var linjer = System.IO.File.ReadAllLines(path).Skip(1); // hopp over header
        var firmaStatus = new Dictionary<string, int>();
        var orgformFordeling = new Dictionary<string, int>();
        var ansattFordeling = new Dictionary<string, int>
    {
        { "0 ansatte", 0 },
        { "1-9 ansatte", 0 },
        { "10-49 ansatte", 0 },
        { "50+ ansatte", 0 }
    };

        int totalOrgform = 0;

        foreach (var linje in linjer)
        {
            var deler = linje.Split(';');
            if (deler.Length < 6) continue;

            string status = deler[2];
            string orgform = deler[4];
            int.TryParse(deler[3], out int antallAnsatte);

            // Status
            if (!firmaStatus.ContainsKey(status))
                firmaStatus[status] = 0;
            firmaStatus[status]++;

            // Organisasjonsform
            if (!string.IsNullOrWhiteSpace(orgform))
            {
                if (!orgformFordeling.ContainsKey(orgform))
                    orgformFordeling[orgform] = 0;
                orgformFordeling[orgform]++;
                totalOrgform++;
            }

            // Ansattfordeling
            if (antallAnsatte == 0)
                ansattFordeling["0 ansatte"]++;
            else if (antallAnsatte >= 1 && antallAnsatte <= 9)
                ansattFordeling["1-9 ansatte"]++;
            else if (antallAnsatte >= 10 && antallAnsatte <= 49)
                ansattFordeling["10-49 ansatte"]++;
            else
                ansattFordeling["50+ ansatte"]++;
        }

        // Konverter organisasjonsform til prosent
        var orgformProsent = orgformFordeling.ToDictionary(
            x => x.Key,
            x => Math.Round(x.Value * 100.0 / totalOrgform, 2)
        );

        var resultat = new FirmaStatistikk
        {
            FirmaStatus = firmaStatus,
            OrganisasjonsformProsent = orgformProsent,
            AnsattFordeling = ansattFordeling
        };

        return Ok(resultat);
    }

}