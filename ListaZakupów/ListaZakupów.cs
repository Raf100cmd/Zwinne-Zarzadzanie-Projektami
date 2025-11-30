using System.Collections.Generic;

namespace ListaZakupowWPF
{
    public class ListaZakupow
    {
        public string NazwaListy { get; set; }
        public List<Zakup> Zakupy { get; set; } = new List<Zakup>();

        public override string ToString()
        {
            return NazwaListy;
        }
    }
}


