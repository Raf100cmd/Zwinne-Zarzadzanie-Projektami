using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace ListaZakupowWPF
{
    public partial class MainWindow : Window
    {
        private string fileProdukty;
        private string fileListeZakupow;
        private List<Produkt> listaProduktow = new List<Produkt>();
        private List<ListaZakupow> wszystkieListyZakupow = new List<ListaZakupow>();
        private ListaZakupow aktualnaLista;

        public MainWindow()
        {
            InitializeComponent();
            fileProdukty = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "produkty.txt");
            fileListeZakupow = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "listy_zakupow.txt");
            WczytajProdukty();
            WczytajListeZakupow();
        }

        #region Produkty
        private void WczytajProdukty()
        {
            if (File.Exists(fileProdukty))
            {
                listaProduktow.Clear();
                foreach (var line in File.ReadAllLines(fileProdukty))
                {
                    var parts = line.Split(';');
                    if (parts.Length == 2)
                        listaProduktow.Add(new Produkt { Nazwa = parts[0], Kategoria = parts[1] });
                }
            }

            ListaProdukty.ItemsSource = null;
            ListaProdukty.ItemsSource = listaProduktow;

            CmbProduktyDoZakupow.ItemsSource = null;
            CmbProduktyDoZakupow.ItemsSource = listaProduktow;
            CmbProduktyDoZakupow.DisplayMemberPath = "Nazwa";
        }

        private void BtnDodajProdukt_Click(object sender, RoutedEventArgs e)
        {
            string nazwa = TxtProdukt.Text.Trim();
            string kategoria = (CmbKategoria.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content.ToString() ?? "Inne";

            if (string.IsNullOrWhiteSpace(nazwa))
            {
                MessageBox.Show("Wpisz nazwę produktu!");
                return;
            }

            listaProduktow.Add(new Produkt { Nazwa = nazwa, Kategoria = kategoria });
            TxtProdukt.Text = "";
            CmbKategoria.SelectedIndex = 0;

            WczytajProdukty();
            ZapiszProdukty();
        }

        private void ZapiszProdukty()
        {
            try
            {
                var lines = listaProduktow.Select(p => $"{p.Nazwa};{p.Kategoria}");
                File.WriteAllLines(fileProdukty, lines);
            }
            catch (Exception ex) { MessageBox.Show("Błąd przy zapisie produktów: " + ex.Message); }
        }
        #endregion

        #region Placeholder TextBox
        private void TxtNazwaNowejListy_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TxtNazwaNowejListy.Text == "Nazwa nowej listy")
            {
                TxtNazwaNowejListy.Text = "";
                TxtNazwaNowejListy.Foreground = Brushes.Black;
            }
        }

        private void TxtNazwaNowejListy_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtNazwaNowejListy.Text))
            {
                TxtNazwaNowejListy.Text = "Nazwa nowej listy";
                TxtNazwaNowejListy.Foreground = Brushes.Gray;
            }
        }
        #endregion

        #region Listy zakupów
        private void WczytajListeZakupow()
        {
            wszystkieListyZakupow.Clear();

            if (File.Exists(fileListeZakupow))
            {
                string[] content = File.ReadAllText(fileListeZakupow).Split(new string[] { "\n---\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var listaStr in content)
                {
                    var lines = listaStr.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    if (lines.Length > 0)
                    {
                        ListaZakupow listaZakupow = new ListaZakupow { NazwaListy = lines[0] };
                        for (int i = 1; i < lines.Length; i++)
                        {
                            var parts = lines[i].Split(';');
                            if (parts.Length == 3)
                                listaZakupow.Zakupy.Add(new Zakup { Nazwa = parts[0], Ilosc = double.Parse(parts[1]), Jednostka = parts[2] });
                        }
                        wszystkieListyZakupow.Add(listaZakupow);
                    }
                }
            }

            CmbListeZakupow.ItemsSource = null;
            CmbListeZakupow.ItemsSource = wszystkieListyZakupow;
            if (wszystkieListyZakupow.Count > 0)
                CmbListeZakupow.SelectedIndex = 0;
        }

        private void BtnNowaListaZakupow_Click(object sender, RoutedEventArgs e)
        {
            string nazwaListy = TxtNazwaNowejListy.Text.Trim();
            if (string.IsNullOrWhiteSpace(nazwaListy) || nazwaListy == "Nazwa nowej listy")
                nazwaListy = "Nowa lista " + (wszystkieListyZakupow.Count + 1);

            ListaZakupow lista = new ListaZakupow { NazwaListy = nazwaListy };
            wszystkieListyZakupow.Add(lista);

            aktualnaLista = lista;
            CmbListeZakupow.ItemsSource = null;
            CmbListeZakupow.ItemsSource = wszystkieListyZakupow;
            CmbListeZakupow.SelectedItem = lista;

            ListaZakupy.ItemsSource = null;
            ListaZakupy.ItemsSource = aktualnaLista.Zakupy;

            TxtNazwaNowejListy.Text = "Nazwa nowej listy";
            TxtNazwaNowejListy.Foreground = Brushes.Gray;

            ZapiszListeZakupow();
        }

        private void BtnUsunListeZakupow_Click(object sender, RoutedEventArgs e)
        {
            if (aktualnaLista == null)
            {
                MessageBox.Show("Wybierz listę do usunięcia.");
                return;
            }

            if (MessageBox.Show($"Czy na pewno chcesz usunąć listę '{aktualnaLista.NazwaListy}'?", "Usuń listę", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                wszystkieListyZakupow.Remove(aktualnaLista);
                aktualnaLista = null;
                ListaZakupy.ItemsSource = null;
                CmbListeZakupow.ItemsSource = null;
                CmbListeZakupow.ItemsSource = wszystkieListyZakupow;
                if (wszystkieListyZakupow.Count > 0)
                    CmbListeZakupow.SelectedIndex = 0;
                ZapiszListeZakupow();
            }
        }

        private void CmbListeZakupow_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            aktualnaLista = CmbListeZakupow.SelectedItem as ListaZakupow;
            if (aktualnaLista != null)
            {
                ListaZakupy.ItemsSource = null;
                ListaZakupy.ItemsSource = aktualnaLista.Zakupy;
            }
        }

        private void BtnDodajDoZakupow_Click(object sender, RoutedEventArgs e)
        {
            if (aktualnaLista == null)
            {
                MessageBox.Show("Wybierz listę zakupów lub utwórz nową.");
                return;
            }

            string nazwa = (CmbProduktyDoZakupow.SelectedItem as Produkt)?.Nazwa ?? "";
            if (string.IsNullOrWhiteSpace(nazwa))
            {
                MessageBox.Show("Wybierz produkt z listy.");
                return;
            }

            if (!double.TryParse(TxtIloscZakup.Text.Trim(), out double ilosc) || ilosc <= 0)
            {
                MessageBox.Show("Wpisz poprawną ilość!");
                return;
            }

            string jednostka = (CmbJednostkaZakup.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content.ToString() ?? "szt.";

            aktualnaLista.Zakupy.Add(new Zakup { Nazwa = nazwa, Ilosc = ilosc, Jednostka = jednostka });

            ListaZakupy.ItemsSource = null;
            ListaZakupy.ItemsSource = aktualnaLista.Zakupy;

            TxtIloscZakup.Text = "";
            CmbJednostkaZakup.SelectedIndex = 0;

            ZapiszListeZakupow();
        }

        private void ZapiszListeZakupow()
        {
            try
            {
                List<string> content = new List<string>();
                foreach (var lista in wszystkieListyZakupow)
                {
                    content.Add(lista.NazwaListy);
                    foreach (var z in lista.Zakupy)
                        content.Add($"{z.Nazwa};{z.Ilosc};{z.Jednostka}");
                    content.Add("---");
                }
                File.WriteAllText(fileListeZakupow, string.Join("\n", content));
            }
            catch (Exception ex) { MessageBox.Show("Błąd przy zapisie listy zakupów: " + ex.Message); }
        }
        #endregion
    }
}






