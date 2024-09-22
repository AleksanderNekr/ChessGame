using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ChessGame.GameClasses;

internal static class Extensions
{
    private const double BoardsSize = 250;

    public static Grid Clone(this Grid sourceBoardPresenter)
    {
        var newBoardPresenter = new Grid
        {
            Height = BoardsSize,
            Width = BoardsSize,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Background = sourceBoardPresenter.Background.Clone(),
            RenderTransform = sourceBoardPresenter.RenderTransform.Clone(),
        };
        foreach (var row in sourceBoardPresenter.RowDefinitions)
        {
            newBoardPresenter.RowDefinitions.Add(new RowDefinition { Height = row.Height });
        }
        foreach (var col in sourceBoardPresenter.ColumnDefinitions)
        {
            newBoardPresenter.ColumnDefinitions.Add(new ColumnDefinition { Width = col.Width });
        }

        var newWindowBoard = new ChessBoard();
        foreach (var piece in sourceBoardPresenter.Children.OfType<Piece>())
        {
            Piece _ = piece switch
            {
                Pawn pawn => new Pawn(newWindowBoard, pawn.Color, pawn.Coordinate.Row, pawn.Coordinate.Column),
                Knight knight => new Knight(newWindowBoard, knight.Color, knight.Coordinate.Row, knight.Coordinate.Column),
                Bishop bishop => new Bishop(newWindowBoard, bishop.Color, bishop.Coordinate.Row, bishop.Coordinate.Column),
                Rook rook => new Rook(newWindowBoard, rook.Color, rook.Coordinate.Row, rook.Coordinate.Column),
                Queen queen => new Queen(newWindowBoard, queen.Color, queen.Coordinate.Row, queen.Coordinate.Column),
                King king => new King(newWindowBoard, king.Color, king.Coordinate.Row, king.Coordinate.Column),
                _ => throw new ArgumentOutOfRangeException(nameof(piece)),
            };
        }
        FillBoardPresenter(newBoardPresenter, newWindowBoard);
        return newBoardPresenter;

        static void FillBoardPresenter(Grid boardPresenter, ChessBoard source)
        {
            boardPresenter.Children.Clear();
            for (var i = 0; i < ChessBoard.Size; i++)
            {
                for (var j = 0; j < ChessBoard.Size; j++)
                {
                    UserControl? control = source.GetPieceOrNull(i, j);
                    if (control == null)
                    {
                        continue;
                    }

                    Grid.SetRow(control, i);
                    Grid.SetColumn(control, j);
                    boardPresenter.Children.Add(control);
                }
            }
        }
    }
}