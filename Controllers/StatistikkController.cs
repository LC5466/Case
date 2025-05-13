using Case.Models;
using Microsoft.AspNetCore.Mvc;

namespace Case.Controllers;

[ApiController]
[Route("api/statistikk")]
public class StatistikkController : ControllerBase
{
    private readonly IWebHostEnvironment _env;

    public StatistikkController(IWebHostEnvironment env)
    {
        _env = env;
    }

    [HttpGet] //GET responsen
    public IActionResult HentStatistikk()
    {
        //variabler
        int totalOrgform = 0;
        var output_FileName = "firmaer_output.csv";
        var linjer = System.IO.File.ReadAllLines(output_FileName).Skip(1); //hopp over header
        var firmaStatus = new Dictionary<string, int>();
        var orgformFordeling = new Dictionary<string, int>();
        var ansattFordeling = new Dictionary<string, int>
    {
        { "0 ansatte", 0 },
        { "1-9 ansatte", 0 },
        { "10-49 ansatte", 0 },
        { "50+ ansatte", 0 }
    };

        if (!System.IO.File.Exists(output_FileName))
            return NotFound("Filen firmaer_output.csv finnes ikke.");

        foreach (var linje in linjer)
        {
            var deLimiter = linje.Split(';');
            if (deLimiter.Length < 6) continue;

            string status = deLimiter[2];
            string orgform = deLimiter[4];
            int.TryParse(deLimiter[3], out int antallAnsatte); //gjør om til integer

            //status
            if (!firmaStatus.ContainsKey(status))
                firmaStatus[status] = 0;
            firmaStatus[status]++;

            //organisasjonsform
            if (!string.IsNullOrWhiteSpace(orgform))
            {
                if (!orgformFordeling.ContainsKey(orgform))
                    orgformFordeling[orgform] = 0;
                orgformFordeling[orgform]++;
                totalOrgform++;
            }

            //ansattfordeling
            if (antallAnsatte == 0)
                ansattFordeling["0 ansatte"]++;
            else if (antallAnsatte >= 1 && antallAnsatte <= 9)
                ansattFordeling["1-9 ansatte"]++;
            else if (antallAnsatte >= 10 && antallAnsatte <= 49)
                ansattFordeling["10-49 ansatte"]++;
            else
                ansattFordeling["50+ ansatte"]++;
        }

        //konverter organisasjonsform til prosent
        var orgformProsent = orgformFordeling.ToDictionary(
            x => x.Key,
            x => Math.Round(x.Value * 100.0 / totalOrgform, 2) //runder til to desimaler
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