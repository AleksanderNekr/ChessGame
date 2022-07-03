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
            ChessBoard.BoardChanged += this.ChessBoardBoardChanged;
        }

        private void ChessBoardBoardChanged(Piece sender, BoardChangedEventArgs boardChangedEventArgs)
        {
            this.UpdateGridBoard();
        }

        private void UpdateGridBoard()
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
            ChessBoard.Clear();
            SetPawns();
            SetKnights();
            SetBishops();
        }

        private static void SetBishops()
        {
            _ = new Bishop(PieceColor.White, 7, 2);
            _ = new Bishop(PieceColor.White, 7, 5);
            _ = new Bishop(PieceColor.Black, 0, 2);
            _ = new Bishop(PieceColor.Black, 0, 5);
        }

        private static void SetKnights()
        {
            _ = new Knight(PieceColor.White, 7, 1);
            _ = new Knight(PieceColor.White, 7, 6);
            _ = new Knight(PieceColor.Black, 0, 1);
            _ = new Knight(PieceColor.Black, 0, 6);
        }

        private static void SetPawns()
        {
            for (var i = 0; i < 8; i++)
            {
                _ = new Pawn(PieceColor.White, 6, i);
            }

            for (var i = 0; i < 8; i++)
            {
                _ = new Pawn(PieceColor.Black, 1, i);
            }
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double newSize = Math.Min(e.NewSize.Width, e.NewSize.Height) - 140;
            this.Board.Width  = newSize;
            this.Board.Height = newSize;
        }
    }
}
