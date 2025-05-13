namespace Case.Models
{
    public class FirmaStatistikk
    {
        public Dictionary<string, int> FirmaStatus { get; set; }
        public Dictionary<string, double> OrganisasjonsformProsent { get; set; }
        public Dictionary<string, int> AnsattFordeling { get; set; }
    }
}
