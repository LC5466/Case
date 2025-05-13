using Case.Models;
using Newtonsoft.Json;

namespace Case.Services
{
    public class FirmaService
    {
        private readonly HttpClient _httpClient;

        public FirmaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<FirmaInfo> HentFirmaInfoAsync(string orgNo, string firmaNavn)
        {
            var url = $"https://data.brreg.no/enhetsregisteret/api/enheter/{orgNo}"; //url-en til Brønnøysund Registeret sitt API, med orgNo som input

            try
            {
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                    return LagFeil(orgNo, firmaNavn);

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<FirmaEnhet>(json); //gjør om til json-objekt

                string status = "Aktiv"; //default status er aktiv, inntill sjekken mot Brreg-API sier noe annet
                //statuser
                if (data.slettedato != null)
                    status = "Slettet";
                else if (data.konkurs == true)
                    status = "Konkurs";
                else if ((data.underAvvikling ?? false) || (data.underTvangsavviklingEllerTvangsopplosning ?? false))
                    status = "UnderAvvikling";

                return new FirmaInfo
                {
                    OrgNo = orgNo,
                    FirmaNavn = firmaNavn,
                    Status = status,
                    AntallAnsatte = data.antallAnsatte,
                    OrganisasjonsformKode = data.organisasjonsform?.kode ?? "",
                    Naeringskode = data.naeringskode1?.kode ?? ""
                };
            }
            catch
            {
                return LagFeil(orgNo, firmaNavn);
            }
        }

        //hvis organsissjonsnummeret ikke finnes i registeret, lag status "feil"
        private FirmaInfo LagFeil(string orgNo, string firmaNavn) => new()
        {
            OrgNo = orgNo,
            FirmaNavn = firmaNavn,
            Status = "Feil"
        };
    }
}