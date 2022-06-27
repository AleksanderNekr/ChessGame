using System;
using System.Windows;

namespace ChessGame
{
    /// <inheritdoc cref="System.Windows.Window" />
    internal sealed partial class MainWindow
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }

        private void BoardSizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double newSize = e.NewValue;
            this.Board.Width  = newSize;
            this.Board.Height = newSize;
        }

        private void ButtonBase_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(Environment.CurrentDirectory);
        }
    }
}
