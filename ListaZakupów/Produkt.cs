namespace ListaZakupowWPF
{
    public class Produkt
    {
        public string Nazwa { get; set; }
        public string Kategoria { get; set; }

        public override string ToString()
        {
            return $"{Nazwa} ({Kategoria})";
        }
    }
}






