using System;
using System.Windows;
using System.Windows.Controls;
using ChessGame.GameClasses;

namespace ChessGame
{
    /// <inheritdoc cref="System.Windows.Window" />
    internal sealed partial class MainWindow
    {
        public MainWindow()
        {
            this.InitializeComponent();
            var pawn = new Pawn(PieceColor.White, 5, 5);
            Grid.SetRow(pawn, 0);
            Grid.SetColumn(pawn, 0);
            this.Board.Children.Add(pawn);
            var button = new Button
                         {
                             Content = "Click me"
                         };
            Grid.SetRow(button, 1);
            Grid.SetColumn(button, 1);
            this.Board.Children.Add(button);
        }

        private void MainWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            double newSize = Math.Min(e.NewSize.Width, e.NewSize.Height) - 140;
            this.Board.Width  = newSize;
            this.Board.Height = newSize;
        }
    }
}
