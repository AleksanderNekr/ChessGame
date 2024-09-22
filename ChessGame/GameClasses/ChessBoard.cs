using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;

namespace ChessGame.GameClasses;

public sealed class ChessBoard
{
    public const int Size = 8;

    private readonly List<Piece> _pieces = new();
    internal Action? AfterBoardChanged = null;

    private Piece?[,] Board { get; } = new Piece?[Size, Size];

    public Piece? GetPieceOrNull(int row, int column)
    {
        var coord = new Coordinate(row, column);
        return Board[coord.Row, coord.Column];
    }

    public Piece? GetPieceOrNull(Coordinate coordinate)
        => GetPieceOrNull(coordinate.Row, coordinate.Column);

    private void SetPiece(Piece piece, int row, int column)
    {
        var coord = new Coordinate(row, column);
        Board[coord.Row, coord.Column] = piece;
        piece.Coordinate = coord;
        _pieces.Add(piece);

        OnBoardChanged();
    }

    public void SetPiece(Piece piece, Coordinate coordinate)
    {
        SetPiece(piece, coordinate.Row, coordinate.Column);
    }

    public void RemovePiece(int row, int column)
    {
        var coord = new Coordinate(row, column);
        var piece = GetPieceOrNull(coord);
        if (piece == null)
        {
            return;
        }

        Board[coord.Row, coord.Column] = null;
        _pieces.Remove(piece);

        OnBoardChanged();
    }

    public void RemovePiece(Coordinate coordinate)
    {
        RemovePiece(coordinate.Row, coordinate.Column);
    }

    internal void OnBoardChanged()
    {
        AfterBoardChanged?.Invoke();
        UpdateAllValidMoves();
    }

    /// <summary>
    ///     Removes all pieces from the board.
    /// </summary>
    public void Clear()
    {
        for (var row = 0; row < Size; row++)
        {
            for (var column = 0; column < Size; column++)
            {
                Board[row, column] = null;
            }
        }

        _pieces.Clear();
    }

    public IEnumerable<Piece> GetPlayerPieces(PieceColor color)
        => _pieces.Where(piece => piece.Color == color);

    private bool HasPieceAt(int row, int column)
        => Board[row, column] != null;

    public void ChangePlayer(PieceColor currentPlayerColor)
    {
        var nextPlayerColor = currentPlayerColor == PieceColor.White
            ? PieceColor.Black
            : PieceColor.White;

        foreach (var piece in _pieces)
        {
            // If the piece is the color that we need, unlock it.
            if (piece.Color == nextPlayerColor)
            {
                piece.IsEnabled = true;
                continue;
            }

            // If the piece is not the color that we need, lock it.
            piece.IsEnabled = false;
            piece.BorderBrush = Brushes.Transparent;
        }
    }

    private void UpdateAllValidMoves()
    {
        // Using for loop instead of foreach because we need to change the collection.
        for (var i = _pieces.Count - 1; i >= 0; i--)
        {
            var piece = _pieces[i];
            piece.UpdateValidMoves();
        }
    }

    public void MovePiece(Piece piece, int newCoordinateRow, int newCoordinateColumn)
    {
        if (HasPieceAt(newCoordinateRow, newCoordinateColumn))
        {
            RemovePiece(newCoordinateRow, newCoordinateColumn);
        }

        Board[piece.Coordinate.Row, piece.Coordinate.Column] = null;
        SetPiece(piece, newCoordinateRow, newCoordinateColumn);
    }

    public override int GetHashCode()
    {
        // Start with 2 digits: row and col, then the number of the piece: 1 – pawn, 2 – knight, 3 – bishop, 4 – rook, 5 – queen, 6 – king
        var hash = "";
        for (var row = 0; row < Size; row++)
        {
            for (var col = 0; col < Size; col++)
            {
                var piece = Board[row, col];
                if (piece is null)
                {
                    continue;
                }

                var pieceType = piece switch
                {
                    Pawn => "1",
                    Knight => "2",
                    Bishop => "3",
                    Rook => "4",
                    Queen => "5",
                    King => "6",
                    _ => throw new ArgumentOutOfRangeException(nameof(piece))
                };

                hash += $"{row}{col}{pieceType}";
            }
        }

        return hash.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        if (obj is not ChessBoard other)
        {
            return false;
        }

        if (_pieces.Count != other._pieces.Count)
        {
            return false;
        }

        for (var row = 0; row < Size; row++)
        {
            for (var col = 0; col < Size; col++)
            {
                var piece = Board[row, col];
                var otherPiece = other.Board[row, col];
                if (piece?.GetType() != otherPiece?.GetType() || piece?.Color != otherPiece?.Color)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public ChessBoard Clone()
    {
        var newBoard = new ChessBoard();
        foreach (var piece in _pieces)
        {
            Piece _ = piece switch
            {
                Pawn pawn => new Pawn(newBoard, pawn.Color, pawn.Coordinate.Row, pawn.Coordinate.Column),
                Knight knight => new Knight(newBoard, knight.Color, knight.Coordinate.Row, knight.Coordinate.Column),
                Bishop bishop => new Bishop(newBoard, bishop.Color, bishop.Coordinate.Row, bishop.Coordinate.Column),
                Rook rook => new Rook(newBoard, rook.Color, rook.Coordinate.Row, rook.Coordinate.Column),
                Queen queen => new Queen(newBoard, queen.Color, queen.Coordinate.Row, queen.Coordinate.Column),
                King king => new King(newBoard, king.Color, king.Coordinate.Row, king.Coordinate.Column),
                _ => throw new ArgumentOutOfRangeException(nameof(piece)),
            };
        }

        return newBoard;
    }

    public void ResetPreset(Action setPreset)
    {
        Clear();

        setPreset();
    }
}