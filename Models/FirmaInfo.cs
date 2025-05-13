namespace Case.Models
{
    public class FirmaInfo //dette tar ut de nødvendige kriteriene
    {
        public string OrgNo { get; set; }
        public string FirmaNavn { get; set; }
        public string Status { get; set; }
        public int? AntallAnsatte { get; set; }
        public string OrganisasjonsformKode { get; set; }
        public string Naeringskode { get; set; }
    }
}

