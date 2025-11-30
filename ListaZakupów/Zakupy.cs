namespace ListaZakupowWPF
{
    public class Zakup
    {
        public string Nazwa { get; set; }
        public double Ilosc { get; set; }
        public string Jednostka { get; set; }

        public override string ToString()
        {
            return $"{Nazwa} - {Ilosc} {Jednostka}";
        }
    }
}



