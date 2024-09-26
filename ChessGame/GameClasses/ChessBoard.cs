using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ChessGame.GameClasses;

public sealed class ChessBoard
{
    public const int Size = 8;

    private readonly Piece?[,] _board = new Piece?[Size, Size];
    private readonly List<Piece> _blackPieces = new();
    private readonly List<Piece> _whitePieces = new();

    public Action? AfterBoardChanged;
    public Action<PieceColor>? GameFinished;

    private PieceColor _currentPlayer;
    private PieceColor? _winner;

    private ChessBoard() { }
    public Piece? LastClickedPiece { get; set; }

    public static ChessBoard Init(Action<ChessBoard>? setPresetAction = null, PieceColor firstPlayer = PieceColor.White)
    {
        var board = new ChessBoard
        {
            _currentPlayer = 1 - firstPlayer,
        };

        board.DisableValidMovesUpdate();
        setPresetAction?.Invoke(board);
        board.ChangePlayer();
        board.EnableValidMovesUpdate();

        board.OnBoardChanged();

        return board;
    }

    public Piece? GetPieceOrNull(int row, int column)
    {
        var coord = new Coordinate(row, column);
        return _board[coord.Row, coord.Column];
    }

    public Piece? GetPieceOrNull(Coordinate coordinate)
        => GetPieceOrNull(coordinate.Row, coordinate.Column);

    public IList<Piece> GetPlayerPieces(PieceColor color)
        => color == PieceColor.White
            ? _whitePieces
            : _blackPieces;

    public void MovePiece(Piece piece, int newCoordinateRow, int newCoordinateColumn)
    {
        if (HasPieceAt(newCoordinateRow, newCoordinateColumn))
        {
            RemovePiece(newCoordinateRow, newCoordinateColumn);
        }

        _board[piece.Coordinate.Row, piece.Coordinate.Column] = null;
        SetPiece(piece, newCoordinateRow, newCoordinateColumn);

        ChangePlayer();
        OnBoardChanged();
    }

    public override int GetHashCode()
    {
        // Start with 2 digits: row and col, then the number of the piece: 1 – pawn, 2 – knight, 3 – bishop, 4 – rook, 5 – queen, 6 – king
        var hash = "";
        for (var row = 0; row < Size; row++)
        {
            for (var col = 0; col < Size; col++)
            {
                var piece = _board[row, col];
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
                    _ => throw new ArgumentOutOfRangeException(nameof(piece)),
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

        if (GetPiecesCount() != other.GetPiecesCount())
        {
            return false;
        }

        for (var row = 0; row < Size; row++)
        {
            for (var col = 0; col < Size; col++)
            {
                var piece = _board[row, col];
                var otherPiece = other._board[row, col];
                if (piece is null != otherPiece is null || piece?.GetType() != otherPiece?.GetType() || piece?.Color != otherPiece?.Color)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public ChessBoard Clone(PieceColor? startColor = null)
    {
        var newBoard = Init(board =>
            {
                foreach (var piece in _whitePieces.Concat(_blackPieces))
                {
                    Piece _ = piece switch
                    {
                        Pawn pawn => new Pawn(board, pawn.Color, pawn.Coordinate.Row, pawn.Coordinate.Column),
                        Knight knight => new Knight(board, knight.Color, knight.Coordinate.Row, knight.Coordinate.Column),
                        Bishop bishop => new Bishop(board, bishop.Color, bishop.Coordinate.Row, bishop.Coordinate.Column),
                        Rook rook => new Rook(board, rook.Color, rook.Coordinate.Row, rook.Coordinate.Column),
                        Queen queen => new Queen(board, queen.Color, queen.Coordinate.Row, queen.Coordinate.Column),
                        King king => new King(board, king.Color, king.Coordinate.Row, king.Coordinate.Column),
                        _ => throw new ArgumentOutOfRangeException(nameof(piece)),
                    };
                }
            },
            startColor ?? _currentPlayer);

        return newBoard;
    }

    public async Task<int> CalculateDifferentCellsAsync(ChessBoard finalBoard, CancellationToken cancellationToken)
        => await Task.Run(() =>
            {
                var count = 0;
                for (var row = 0; row < Size; row++)
                {
                    for (var col = 0; col < Size; col++)
                    {
                        var piece = _board[row, col];
                        var otherPiece = finalBoard._board[row, col];
                        if (piece is null != otherPiece is null || piece?.GetType() != otherPiece?.GetType() || piece?.Color != otherPiece?.Color)
                        {
                            count++;
                        }

                        if (cancellationToken.IsCancellationRequested)
                        {
                            return -1;
                        }
                    }
                }

                return count;
            },
            cancellationToken);

    public void AddNewPiece(Piece piece, Coordinate coordinate)
    {
        SetPiece(piece, coordinate);
        switch (piece.Color)
        {
            case PieceColor.White:
                _whitePieces.Add(piece);
                break;
            case PieceColor.Black:
                _blackPieces.Add(piece);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void ChangePlayer()
    {
        var newPlayerColor = 1 - _currentPlayer;
        switch (newPlayerColor)
        {
            case PieceColor.White:
                _whitePieces.ForEach(piece => piece.IsEnabled = true);
                _blackPieces.ForEach(piece =>
                {
                    piece.IsEnabled = false;
                    piece.BorderBrush = Brushes.Transparent;
                });
                break;
            case PieceColor.Black:
                _blackPieces.ForEach(piece => piece.IsEnabled = true);
                _whitePieces.ForEach(piece =>
                {
                    piece.IsEnabled = false;
                    piece.BorderBrush = Brushes.Transparent;
                });
                break;
        }

        _currentPlayer = newPlayerColor;
    }

    private void SetPiece(Piece piece, int row, int column)
    {
        var coord = new Coordinate(row, column);
        _board[coord.Row, coord.Column] = piece;
        piece.Coordinate = coord;
    }

    private void SetPiece(Piece piece, Coordinate coordinate)
    {
        SetPiece(piece, coordinate.Row, coordinate.Column);
    }

    private void EnableValidMovesUpdate()
    {
        AfterBoardChanged -= UpdateAllValidMoves;
        AfterBoardChanged += UpdateAllValidMoves;
    }

    private void DisableValidMovesUpdate()
    {
        AfterBoardChanged -= UpdateAllValidMoves;
    }

    private void OnBoardChanged()
    {
        AfterBoardChanged?.Invoke();
    }

    private void RemovePiece(int row, int column)
    {
        var coord = new Coordinate(row, column);
        var piece = GetPieceOrNull(coord);
        if (piece == null)
        {
            return;
        }

        _board[coord.Row, coord.Column] = null;

        switch (piece.Color)
        {
            case PieceColor.White:
                _whitePieces.Remove(piece);
                break;
            case PieceColor.Black:
                _blackPieces.Remove(piece);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private bool HasPieceAt(int row, int column)
        => _board[row, column] != null;

    private void UpdateAllValidMoves()
    {
        _whitePieces.ForEach(x => x.UpdateValidMoves());
        _blackPieces.ForEach(x => x.UpdateValidMoves());
        var hasMoves = _currentPlayer == PieceColor.White
            ? _whitePieces.Any(x => x.HasValidMoves())
            : _blackPieces.Any(x => x.HasValidMoves());

        if (!hasMoves)
        {
            _winner = 1 - _currentPlayer;
            GameFinished?.Invoke(_winner.Value);
        }
    }

    private int GetPiecesCount()
        => _whitePieces.Count + _blackPieces.Count;

    public PieceColor GetCurrentPlayer()
        => _currentPlayer;

    public ChessBoard CloneAsOnlyPieces()
    {
        var newBoard = new ChessBoard
        {
            _currentPlayer = _currentPlayer,
        };

        foreach (var piece in _whitePieces.Concat(_blackPieces))
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

    public PieceColor? GetWinner()
        => _winner;
}