using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ChessGame.GameClasses;

/// <summary>
///     Knight class.
/// </summary>
internal sealed class Knight : Piece
{
    /// <inheritdoc />
    public Knight(ChessBoard board, PieceColor color, int row, int column) : base(board, color, row, column)
    {
    }

    /// <inheritdoc />
    public Knight(ChessBoard board, PieceColor color, Coordinate coordinate) : base(board, color, coordinate)
    {
    }

    /// <inheritdoc />
    protected override ImageBrush WhiteImage { get; } = (ImageBrush)Application.Current.Resources["WhiteKnight"];

    /// <inheritdoc />
    protected override ImageBrush BlackImage { get; } = (ImageBrush)Application.Current.Resources["BlackKnight"];

    /// <inheritdoc />
    protected internal override void UpdateValidMoves()
    {
        ValidMoves.Clear();
        TryToAddMove(Coordinate.Row - 2, Coordinate.Column + 1);
        TryToAddMove(Coordinate.Row - 2, Coordinate.Column - 1);
        TryToAddMove(Coordinate.Row - 1, Coordinate.Column + 2);
        TryToAddMove(Coordinate.Row - 1, Coordinate.Column - 2);
        TryToAddMove(Coordinate.Row + 2, Coordinate.Column + 1);
        TryToAddMove(Coordinate.Row + 2, Coordinate.Column - 1);
        TryToAddMove(Coordinate.Row + 1, Coordinate.Column + 2);
        TryToAddMove(Coordinate.Row + 1, Coordinate.Column - 2);
    }

    private void TryToAddMove(int coordinateRow, int coordinateColumn)
    {
        Coordinate coordinate;
        try
        {
            coordinate = new Coordinate(coordinateRow, coordinateColumn);
        }
        catch (ArgumentOutOfRangeException)
        {
            return;
        }

        UserControl? place = Board.GetPieceOrNull(coordinate);
        if (place == null || IsEnemy(place))
        {
            ValidMoves.Add(coordinate);
        }
    }

    private bool IsEnemy(UserControl place)
        => place is Piece piece && piece.Color != Color;
}