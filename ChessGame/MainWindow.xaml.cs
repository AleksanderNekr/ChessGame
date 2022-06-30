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

        private void ButtonBase_Click(object sender, RoutedEventArgs e)
        {
            _ = MessageBox.Show(Environment.CurrentDirectory);
        }

        private void MainWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            double newSize = Math.Min(e.NewSize.Width, e.NewSize.Height) - 140;
            this.Board.Width  = newSize;
            this.Board.Height = newSize;
        }
    }
}
