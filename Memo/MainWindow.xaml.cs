﻿using System;
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
using System.Windows.Threading;

namespace Memo
{

    public partial class MainWindow : Window
    {
        // Tarjetas seleccionadas
        private Border T1 { get; set; }
        private Border T2 { get; set; }

        private DispatcherTimer Timer { get; set; }

        private const string SIMBOLO_INICIAL_TARJETA = "s";
        private const int DIFICULTAD_BAJA = 3;
        private const int DIFICULTAD_MEDIA = 4;
        private const int DIFICULTAD_ALTA = 5;
        private const int COLUMNAS = 4;
        private const int TIEMPO_VISUALIZACION = 1;

        public MainWindow()
        {
            InitializeComponent();
            Timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromSeconds(TIEMPO_VISUALIZACION)
            };
            Timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Vuelven las cartas al estado anterior y se habilitan
            RestablerEstado();
            Timer.Stop();
            HabilitarTarjetas();
        }

        private void DeshabilitarTarjetas()
        {
            // Se deshabilitan todas las tarjetas
            foreach (var item in ContenedorCartasGrid.Children)
            {
                ((Border)item).IsEnabled = false;
            }
        }

        private void HabilitarTarjetas()
        {
            // Se habilitan todas menos las que tienen algo en el Tag, es decir, las que ya han sido averiguadas
            foreach (var item in ContenedorCartasGrid.Children)
            {
                Border b = (Border)item;
                b.IsEnabled = b.Tag == null;
            }
        }

        private void MostrarButton_Click(object sender, RoutedEventArgs e)
        {
            DeshabilitarTarjetas();
            BarraProgressBar.Value = BarraProgressBar.Maximum;
            RevelarTarjetas();
            ComprobarFinJuego();
        }

        private void RevelarTarjetas()
        {
            foreach (var item in ContenedorCartasGrid.Children)
            {
                RevelarTarjeta((Border)item);
            }
        }

        private void InicarButton_Click(object sender, RoutedEventArgs e)
        {
            // Por si es una partida nueva en la misma ejecución
            LimpiarReferencias();

            // Símbolos disponibles
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

            // Se reinicia la barra y se establece el objetivo de parejas
            BarraProgressBar.Maximum = parejas;
            BarraProgressBar.Value = 0;

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
                        Text = SIMBOLO_INICIAL_TARJETA,
                        Style = (Style)Resources["Tarjetas"]
                    };

                    // Se le asigna un Tag aleatorio
                    int aleatorio = r.Next(simbolos.Count);
                    textBlock.Tag = simbolos[aleatorio];
                    simbolos.RemoveAt(aleatorio);

                    // Creación del resto de elementos
                    Viewbox viewbox = new Viewbox() { Child = textBlock };

                    Border border = new Border()
                    {
                        Child = viewbox,
                        Style = (Style)Resources["BordeTarjetas"]
                    };
                    border.MouseDown += Border_MouseDown;

                    // Posicionamiento dentro del grid
                    grid.Children.Add(border);
                    Grid.SetRow(border, i);
                    Grid.SetColumn(border, j);
                }
            }

            // Habilitamos la opción de mostrar
            MostrarButton.IsEnabled = true;
        }

        private void RevelarTarjeta(Border b)
        {
            b.Style = (Style)Resources["TarjetasVolteadas"];
            TextBlock t = (TextBlock)((Viewbox)b.Child).Child;
            t.Text = ((char)t.Tag).ToString();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Border b = sender as Border;
            RevelarTarjeta(b);

            if (T1 == null) { T1 = b; }
            else if (T1 != b) { T2 = b; }

            ComprobarCartas();
        }

        private bool CartasSeleccionadas()
        {
            return T1 != null && T2 != null;
        }

        private bool CartasIguales()
        {
            TextBlock t1 = (TextBlock)((Viewbox)T1.Child).Child;
            TextBlock t2 = (TextBlock)((Viewbox)T2.Child).Child;
            return ((char)t1.Tag).ToString() == ((char)t2.Tag).ToString();
        }

        private void LimpiarReferencias()
        {
            T1 = null;
            T2 = null;
        }

        private void RestablerEstado()
        {
            TextBlock t1 = (TextBlock)((Viewbox)T1.Child).Child;
            TextBlock t2 = (TextBlock)((Viewbox)T2.Child).Child;
            t1.Text = SIMBOLO_INICIAL_TARJETA;
            t2.Text = SIMBOLO_INICIAL_TARJETA;
            T1.Style = (Style)Resources["BordeTarjetas"];
            T2.Style = (Style)Resources["BordeTarjetas"];
            LimpiarReferencias();
        }

        private void ComprobarCartas()
        {
            // Si se seleccionan dos cartas distintas
            if (CartasSeleccionadas() && !CartasIguales())
            {
                // Se deshabiltan hasta que pasen unos segundos
                DeshabilitarTarjetas();
                Timer.Start();
            }
            // Si son iguales
            else if (CartasSeleccionadas() && CartasIguales())
            {
                // Se deshabilitan y coloco flag a true, servirá para no volverlo a habilitar
                // con el metodo Habilitar
                T1.IsEnabled = false;
                T1.Tag = true;
                T2.IsEnabled = false;
                T2.Tag = true;
                LimpiarReferencias();
                ActualizarBarraProgreso();
            }
        }

        private void ActualizarBarraProgreso()
        {
            BarraProgressBar.Value++;
            ComprobarFinJuego();
        }

        private void ComprobarFinJuego()
        {
            if (BarraProgressBar.Value == BarraProgressBar.Maximum)
            {
                MessageBox.Show("Partida finalizada.", "Fin", MessageBoxButton.OK, MessageBoxImage.Information);
                MostrarButton.IsEnabled = false;
            }
        }
    }
}
