using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ChessGame.GameClasses;

/// <summary>
///     Base class for all chess pieces.
/// </summary>
public abstract class Piece : UserControl
{
    private protected readonly ChessBoard Board;
    private readonly List<ValidMove> _validMoves = new();

    /// <summary>
    ///     Constructor for the Piece class.
    /// </summary>
    /// <param name="board">The chess board</param>
    /// <param name="color">The color of the piece.</param>
    /// <param name="row">The row of the piece.</param>
    /// <param name="column">The column of the piece.</param>
    protected Piece(ChessBoard board, PieceColor color, int row, int column)
    {
        Board = board;
        Coordinate = new Coordinate(row, column);
        Color = color;
        Cursor = Cursors.Hand;
        BorderThickness = new Thickness(1);
        Focusable = true;
        FocusVisualStyle = null;
        MouseEnter += Piece_MouseEnter;
        MouseLeave += Piece_MouseLeave;
        GotFocus += Piece_GotFocus;
        LostFocus += Piece_LostFocus;
        MouseLeftButtonUp += Piece_MouseLeftButtonUp;
        SetBackgroundImage();
        Board.AddNewPiece(this, Coordinate);
    }

    /// <summary>
    ///     Constructor for the Piece class.
    /// </summary>
    /// <param name="board">The chess board</param>
    /// <param name="color">The color of the piece.</param>
    /// <param name="coordinate">The coordinate of the piece.</param>
    protected Piece(ChessBoard board, PieceColor color, Coordinate coordinate) : this(board, color, coordinate.Row, coordinate.Column)
    {
    }

    /// <summary>
    ///     Color of the piece.
    /// </summary>
    public PieceColor Color { get; }

    /// <summary>
    ///     Valid moves of the piece.
    /// </summary>
    protected List<Coordinate> ValidMoves { get; } = new();

    /// <summary>
    ///     White image of the piece.
    /// </summary>
    protected abstract ImageBrush WhiteImage { get; }

    /// <summary>
    ///     Black image of the piece.
    /// </summary>
    protected abstract ImageBrush BlackImage { get; }

    /// <summary>
    ///     Coordinate of the piece.
    /// </summary>
    public Coordinate Coordinate { get; set; }

    /// <summary>
    ///     Event occurs when the piece is clicked.
    /// </summary>
    public static event LastClickedHandler? LastClicked;

    /// <summary>
    ///     Moves the piece to the specified coordinate.
    /// </summary>
    /// <param name="newCoordinate">The new coordinate of the piece.</param>
    public void MoveTo(Coordinate newCoordinate)
    {
        MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        HideValidMoves();

        Board.MovePiece(this, newCoordinate.Row, newCoordinate.Column);
        Coordinate = newCoordinate;
    }

    /// <summary>
    ///     Updates the valid moves of the piece.
    /// </summary>
    public abstract void UpdateValidMoves();

    protected void AddRangeMoves(Piece piece, int rowDif, int colDif)
    {
        var row = piece.Coordinate.Row;
        var column = piece.Coordinate.Column;
        while (Coordinate.IsCorrectCoordinate(row += rowDif, column += colDif))
        {
            var place = Board.GetPieceOrNull(row, column);
            if (place == null)
            {
                piece.ValidMoves.Add(new Coordinate(row, column));
                continue;
            }

            // If ally piece is found, then stop.
            if (place.Color == piece.Color)
            {
                break;
            }

            // If enemy piece is found, then add it to the valid moves and stop.
            piece.ValidMoves.Add(new Coordinate(row, column));
            break;
        }
    }

    private void Piece_GotFocus(object sender, RoutedEventArgs e)
    {
        var piece = (Piece)sender;
        piece.BorderThickness = new Thickness(2);
        piece.BorderBrush = Brushes.Chartreuse;
        piece.MouseEnter -= Piece_MouseEnter;
        piece.MouseLeave -= Piece_MouseLeave;

        piece.ShowValidMoves();
        LastClicked?.Invoke(piece, e);
    }

    private static void Piece_LostFocus(object sender, RoutedEventArgs e)
    {
        var piece = (Piece)sender;
        piece.BorderThickness = new Thickness(1);
        piece.BorderBrush = Brushes.Transparent;
        piece.MouseEnter += Piece_MouseEnter;
        piece.MouseLeave += Piece_MouseLeave;
        piece.HideValidMoves();
    }

    private void Piece_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        var piece = (Piece)sender;
        if (piece.IsEnemy())
        {
            Board.LastClickedPiece?.MoveTo(piece.Coordinate);
            return;
        }

        if (piece.IsFocused)
        {
            // Remove focus from the piece.
            piece.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            return;
        }

        piece.Focus();
        Board.LastClickedPiece = piece;
    }

    private bool IsEnemy()
        => Color != Board.GetCurrentPlayer();

    private void ShowValidMoves()
    {
        foreach (var coordinate in ValidMoves)
        {
            var place = Board.GetPieceOrNull(coordinate);
            switch (place)
            {
                case null:
                    _validMoves.Add(new ValidMove(Board, coordinate));
                    continue;
                case var piece when piece.Color != Color:
                    SetEnemyHighlight(piece);
                    break;
            }
        }
    }

    private void HideValidMoves()
    {
        foreach (var validMoveCoord in ValidMoves)
        {
            var piece = Board.GetPieceOrNull(validMoveCoord);
            if (piece is not null)
            {
                UnsetEnemyHighlight(piece);
            }
        }

        _validMoves.ForEach(validMove => validMove.Dispose());
        _validMoves.Clear();
    }

    private static void SetEnemyHighlight(Piece place)
    {
        place.BorderBrush = Brushes.Red;
        place.IsEnabled = true;
    }

    private static void UnsetEnemyHighlight(Piece place)
    {
        place.BorderBrush = Brushes.Transparent;
        place.IsEnabled = false;
    }

    private void SetBackgroundImage()
    {
        if (Color == PieceColor.White)
        {
            Background = WhiteImage;
            return;
        }

        Background = BlackImage;
    }

    private static void Piece_MouseLeave(object sender, MouseEventArgs e)
    {
        var piece = (Piece)sender;
        if (piece.IsEnemy())
        {
            piece.BorderThickness = new Thickness(1);
            piece.BorderBrush = Brushes.Red;
            return;
        }

        piece.BorderBrush = Brushes.Transparent;
    }

    private static void Piece_MouseEnter(object sender, MouseEventArgs e)
    {
        var piece = (Piece)sender;
        if (piece.IsEnemy())
        {
            piece.BorderThickness = new Thickness(2);
            piece.BorderBrush = Brushes.Red;
            return;
        }

        piece.BorderBrush = Brushes.Chartreuse;
    }

    public IEnumerable<Coordinate> GetValidMoves()
        => ValidMoves;


    /// <summary>
    ///     Handler for the BoardChangedEvent.
    /// </summary>
    public delegate void LastClickedHandler(Piece sender, RoutedEventArgs e);
}

/// <summary>
///     Enum for the piece colors.
/// </summary>
public enum PieceColor
{
    White,
    Black,
}