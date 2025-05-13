namespace Case.Models
{
    public class FirmaEnhet //dette er generell informasjon av en enhet i json-fila
    {
        public string organisasjonsnummer { get; set; }
        public int? antallAnsatte { get; set; }
        public bool? konkurs { get; set; }
        public bool? underAvvikling { get; set; }
        public bool? underTvangsavviklingEllerTvangsopplosning { get; set; }
        public DateTime? slettedato { get; set; }
        public Organisasjonsform organisasjonsform { get; set; }
        public Naeringskode naeringskode1 { get; set; }
    }

    public class Organisasjonsform
    {
        public string kode { get; set; }
    }

    public class Naeringskode
    {
        public string kode { get; set; }
    }
}