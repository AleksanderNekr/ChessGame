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
            ChessBoard.ContentChanged += this.ChessBoard_ContentChanged;
        }

        private void ChessBoard_ContentChanged(object sender, ContentChangedEventArgs contentChangedEventArgs)
        {
            this.UpdateBoard();
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double newSize = Math.Min(e.NewSize.Width, e.NewSize.Height) - 140;
            this.Board.Width  = newSize;
            this.Board.Height = newSize;
        }

        private void UpdateBoard()
        {
            this.Board.Children.Clear();
            for (var i = 0; i < ChessBoard.Board.GetLength(0); i++)
            {
                for (var j = 0; j < ChessBoard.Board.GetLength(1); j++)
                {
                    Piece? piece = ChessBoard.GetPieceOrNull(i, j);
                    if (piece == null)
                    {
                        continue;
                    }

                    Grid.SetRow(piece, i);
                    Grid.SetColumn(piece, j);
                    this.Board.Children.Add(piece);
                }
            }
        }

        private void ButtonBase_Click(object sender, RoutedEventArgs e)
        {
            var whitePawn = new Pawn(PieceColor.White, 4, 5);
            var blackPawn = new Pawn(PieceColor.Black, 4, 4);
        }
    }
}
