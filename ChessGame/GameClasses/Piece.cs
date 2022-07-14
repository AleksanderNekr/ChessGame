using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ChessGame.GameClasses
{
    /// <summary>
    ///     Base class for all chess pieces.
    /// </summary>
    internal abstract class Piece : UserControl
    {
        private bool _isEnemy;

        private static Piece? _lastMovedPiece;

        /// <summary>
        ///     Constructor for the Piece class.
        /// </summary>
        /// <param name="color">The color of the piece.</param>
        /// <param name="row">The row of the piece.</param>
        /// <param name="column">The column of the piece.</param>
        protected Piece(PieceColor color, int row, int column)
        {
            this.Coordinate         =  new Coordinate(row, column);
            this.Color              =  color;
            this.Cursor             =  Cursors.Hand;
            this.BorderThickness    =  new Thickness(1);
            this.Focusable          =  true;
            this.FocusVisualStyle   =  null;
            ChessBoard.BoardChanged += ChessBoard_BoardChanged;
            this.MouseEnter         += Piece_MouseEnter;
            this.MouseLeave         += Piece_MouseLeave;
            this.GotFocus           += Piece_GotFocus;
            this.LostFocus          += Piece_LostFocus;
            this.MouseLeftButtonUp  += Piece_MouseLeftButtonUp;
            this.SetBackgroundImage();
            ChessBoard.SetPiece(this, this.Coordinate);
        }

        /// <summary>
        ///     Constructor for the Piece class.
        /// </summary>
        /// <param name="color">The color of the piece.</param>
        /// <param name="coordinate">The coordinate of the piece.</param>
        protected Piece(PieceColor color, Coordinate coordinate) : this(color, coordinate.Row, coordinate.Column)
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
        internal protected Coordinate Coordinate { get; internal set; }

        /// <summary>
        ///     Event occurs when the piece is clicked.
        /// </summary>
        internal static event LastClickedHandler? LastClicked;

        /// <summary>
        ///     Moves the piece to the specified coordinate.
        /// </summary>
        /// <param name="newCoordinate">The new coordinate of the piece.</param>
        public void MoveTo(Coordinate newCoordinate)
        {
            this.HideValidMoves();
            Coordinate oldCoordinate = this.Coordinate;
            this.IfPawnMove(newCoordinate, oldCoordinate);

            if (ChessBoard.Board[newCoordinate.Row, newCoordinate.Column] != null)
            {
                ChessBoard.Pieces.Remove((Piece)ChessBoard.Board[newCoordinate.Row, newCoordinate.Column]
                                      ?? throw new InvalidOperationException());
            }

            ChessBoard.Board[newCoordinate.Row, newCoordinate.Column]     = this;
            ChessBoard.Board[this.Coordinate.Row, this.Coordinate.Column] = null;
            this.Coordinate                                               = newCoordinate;
            this.ChangePlayer();
            ChessBoard.OnBoardChanged(this, new BoardChangedEventArgs(oldCoordinate, newCoordinate));

            _lastMovedPiece = this;
        }

        protected static List<Coordinate> GetAllAttackCoordinates(PieceColor color)
        {
            Dictionary<Coordinate, int> attackCoordinates = new();
            foreach (Piece piece in ChessBoard.Pieces)
            {
                if (piece.Color == color)
                {
                    piece.UpdateValidMoves();
                    foreach (Coordinate validMove in piece.ValidMoves)
                    {
                        attackCoordinates.TryAdd(validMove, 0);
                    }
                }
            }

            return new List<Coordinate>(attackCoordinates.Keys);
        }

        private void IfPawnMove(Coordinate newCoordinate, Coordinate oldCoordinate)
        {
            if (this is not Pawn pawn)
            {
                return;
            }

            pawn.PrevCoord = oldCoordinate;
            pawn.LastMove  = newCoordinate;
            CutIfTakeOnPass(newCoordinate, oldCoordinate);
        }

        private static void CutIfTakeOnPass(Coordinate newCoordinate, Coordinate oldCoordinate)
        {
            var enemy = (Piece?)ChessBoard.Board[oldCoordinate.Row, newCoordinate.Column];
            if ((ChessBoard.GetControlOrNull(newCoordinate) != null) || enemy is not Pawn)
            {
                return;
            }

            if ((ValidMove.LastClickedPiece != null) && (enemy.Color != ValidMove.LastClickedPiece.Color))
            {
                ChessBoard.Pieces.Remove(enemy);
            }

            ChessBoard.Board[oldCoordinate.Row, newCoordinate.Column] = null;
        }

        private void ChangePlayer()
        {
            if (this.Color == PieceColor.White)
            {
                ChangeUnlockPieces(PieceColor.Black);
                return;
            }

            ChangeUnlockPieces(PieceColor.White);
        }

        private static void ChangeUnlockPieces(PieceColor color)
        {
            foreach (Piece piece in ChessBoard.Pieces)
            {
                // If the piece is the color that we need, unlock it.
                if (piece.Color == color)
                {
                    piece.IsEnabled = true;
                    continue;
                }

                // If the piece is not the color that we need, lock it.
                piece.IsEnabled   = false;
                piece.BorderBrush = Brushes.Transparent;
            }
        }

        /// <summary>
        ///     Moves the piece to the specified coordinate.
        /// </summary>
        /// <param name="row">The row of the coordinate.</param>
        /// <param name="column">The column of the coordinate.</param>
        public void MoveTo(int row, int column)
        {
            this.MoveTo(new Coordinate(row, column));
        }

        internal static void AddRangeMoves(Piece piece, int rowDif, int colDif)
        {
            int row    = piece.Coordinate.Row;
            int column = piece.Coordinate.Column;
            while (Coordinate.IsCorrectCoordinate(row += rowDif, column += colDif))
            {
                UserControl? place = ChessBoard.GetControlOrNull(row, column);
                if (place == null)
                {
                    piece.ValidMoves.Add(new Coordinate(row, column));
                    continue;
                }

                // If ally piece is found, then stop.
                if (place is Piece ally && (ally.Color == piece.Color))
                {
                    break;
                }

                // If enemy piece is found, then add it to the valid moves and stop.
                piece.ValidMoves.Add(new Coordinate(row, column));
                break;
            }
        }

        /// <summary>
        ///     Updates the valid moves of the piece.
        /// </summary>
        protected abstract void UpdateValidMoves();

        public static void UpdateAllValidMoves()
        {
            foreach (Piece piece in ChessBoard.Pieces)
            {
                piece.UpdateValidMoves();
            }
        }

        private static void Piece_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is ValidMove)
            {
                return;
            }

            var piece = (Piece)sender;
            piece.BorderThickness =  new Thickness(2);
            piece.BorderBrush     =  Brushes.Chartreuse;
            piece.MouseEnter      -= Piece_MouseEnter;
            piece.MouseLeave      -= Piece_MouseLeave;
            piece.UpdateValidMoves();
            AddTakeOnPass(piece);
            piece.ShowValidMoves();
            LastClicked?.Invoke(piece, e);

            // OMG! It fixes ShadowBug!
            UnlockPieces(piece.Color);
        }

        private static void AddTakeOnPass(Piece piece)
        {
            if (_lastMovedPiece is not Pawn pawnEnemy || piece is not Pawn pawn)
            {
                return;
            }

            if (pawnEnemy.LastMove.Row != (pawnEnemy.PrevCoord.Row + (pawnEnemy.Move * 2)))
            {
                return;
            }

            if ((pawnEnemy.LastMove.Row != pawn.Coordinate.Row) || !IsNear(pawnEnemy, pawn))
            {
                return;
            }

            pawn.ValidMoves.Add(new Coordinate(pawnEnemy.LastMove.Row + pawn.Move,
                                               pawnEnemy.LastMove.Column));
        }

        private static bool IsNear(Pawn pawnEnemy, Pawn pawn)
        {
            return Math.Abs(pawnEnemy.LastMove.Column - pawn.Coordinate.Column) == 1;
        }

        private static void UnlockPieces(PieceColor pieceColor)
        {
            foreach (Piece piece in ChessBoard.Pieces.Where(piece => piece.Color == pieceColor))
            {
                piece.IsEnabled = true;
            }
        }

        private static void Piece_LostFocus(object sender, RoutedEventArgs e)
        {
            var piece = (Piece)sender;
            piece.BorderThickness =  new Thickness(1);
            piece.BorderBrush     =  Brushes.Transparent;
            piece.MouseEnter      += Piece_MouseEnter;
            piece.MouseLeave      += Piece_MouseLeave;
            piece.HideValidMoves();
        }

        private static void Piece_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var piece = (Piece)sender;
            if (piece._isEnemy && (piece.Color != ValidMove.LastClickedPiece?.Color))
            {
                ValidMove.LastClickedPiece?.MoveTo(piece.Coordinate);
                return;
            }

            if (piece.IsFocused)
            {
                // Remove focus from the piece.
                piece.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                return;
            }

            piece.Focus();
        }

        private void ShowValidMoves()
        {
            foreach (Coordinate coordinate in this.ValidMoves)
            {
                UserControl? place = ChessBoard.GetControlOrNull(coordinate);
                switch (place)
                {
                    case null:
                        _ = new ValidMove(coordinate);
                        continue;
                    case Piece piece:
                        SetEnemyHighlight(piece);
                        break;
                }
            }
        }

        private static void SetEnemyHighlight(Piece place)
        {
            place._isEnemy    = true;
            place.BorderBrush = Brushes.Red;
            place.IsEnabled   = true;
        }

        private void HideValidMoves()
        {
            foreach (Coordinate coordinate in this.ValidMoves)
            {
                UserControl? place = ChessBoard.GetControlOrNull(coordinate);
                switch (place)
                {
                    case null:
                        continue;
                    case ValidMove:
                        // If valid move is found, then remove it.
                        ChessBoard.RemoveControl(coordinate);
                        continue;
                    default:
                        // If enemy piece is found, then hide its highlight.
                        UnsetEnemyHighlight((Piece?)place);
                        break;
                }
            }
        }

        private static void UnsetEnemyHighlight(Piece? place)
        {
            if (place == null)
            {
                return;
            }

            place._isEnemy    = false;
            place.BorderBrush = Brushes.Transparent;
            place.IsEnabled   = false;
        }

        internal static void ChessBoard_BoardChanged(UserControl sender, BoardChangedEventArgs e)
        {
            if (sender is not Piece piece)
            {
                return;
            }

            piece.UpdateValidMoves();
        }

        private void SetBackgroundImage()
        {
            if (this.Color == PieceColor.White)
            {
                this.Background = this.WhiteImage;
                return;
            }

            this.Background = this.BlackImage;
        }

        private static void Piece_MouseLeave(object sender, MouseEventArgs e)
        {
            var piece = (Piece)sender;
            if (piece._isEnemy)
            {
                piece.BorderThickness = new Thickness(1);
                piece.BorderBrush     = Brushes.Red;
                return;
            }

            piece.BorderBrush = Brushes.Transparent;
        }

        private static void Piece_MouseEnter(object sender, MouseEventArgs e)
        {
            var piece = (Piece)sender;
            if (piece._isEnemy)
            {
                piece.BorderThickness = new Thickness(2);
                piece.BorderBrush     = Brushes.Red;
                return;
            }

            piece.BorderBrush = Brushes.Chartreuse;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{this.Color} {this.GetType().Name} on {this.Coordinate}";
        }

        /// <summary>
        ///     Handler for the BoardChangedEvent.
        /// </summary>
        internal delegate void LastClickedHandler(Piece sender, RoutedEventArgs e);
    }

    /// <summary>
    ///     Enum for the piece colors.
    /// </summary>
    internal enum PieceColor
    {
        White,
        Black
    }
}
