using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Memo
{
    
    public partial class MainWindow : Window
    {
        private TextBlock TextBlock1 { get; set; }
        private TextBlock TextBlock2 { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MostrarButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void InicarButton_Click(object sender, RoutedEventArgs e)
        {
            const int DIFICULTAD_BAJA = 3;
            const int DIFICULTAD_MEDIA = 4;
            const int DIFICULTAD_ALTA = 5;
            const int COLUMNAS = 4;

            List<char> simbolos = "aabbccddeeffgghhiijj".ToCharArray().ToList();

            // Establecer dificultad
            int dificultad = DIFICULTAD_MEDIA;
            if (BajaRadioButton.IsChecked == true)
            {
                dificultad = DIFICULTAD_BAJA;
            }
            else if (AltaRadioButton.IsChecked == true)
            {
                dificultad = DIFICULTAD_ALTA;
            }

            int parejas = dificultad * 2;
            int simbolosTotales = parejas * 2;

            // Reiniciar las columnas y filas del grid
            Grid grid = ContenedorCartasGrid;
            grid.RowDefinitions.Clear();
            grid.ColumnDefinitions.Clear();

            // Añadir columnas
            for (int i = 0; i < COLUMNAS; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            // Semilla
            Random r = new Random();

            // Eliminar sómbolos que no necesito
            simbolos.RemoveRange(simbolosTotales, simbolos.Count - simbolosTotales);

            // Crear lar cartas y crear filas
            for (int i = 0; i < dificultad; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition());
                for (int j = 0; j < COLUMNAS; j++)
                {
                    // TextBlock
                    TextBlock textBlock = new TextBlock()
                    {
                        Text = "s",
                        Style = (Style)Resources["Tarjetas"]
                    };

                    // Se le asigna un Tag aleatorio
                    int aleatorio = r.Next(simbolos.Count);
                    textBlock.Tag = simbolos[aleatorio];
                    simbolos.RemoveAt(aleatorio);

                    Viewbox viewbox = new Viewbox() { Child = textBlock };
                    Border border = new Border() { Child = viewbox};
                    border.MouseDown += Border_MouseDown;

                    grid.Children.Add(border);
                    Grid.SetRow(border, i);
                    Grid.SetColumn(border, j);
                }
            }
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Border b = sender as Border;
            b.Background = Brushes.White;
            Viewbox v = (Viewbox) b.Child;
            TextBlock t = (TextBlock)v.Child;
            t.Text = ((char)t.Tag).ToString();
            
        }

        private bool CartasSeleccionadas()
        {
            return TextBlock1 != null & TextBlock2 != null;
        }

        private bool CartasIguales()
        {
            return (string)TextBlock1.Tag == (string)TextBlock2.Tag;
        }
    }
}
