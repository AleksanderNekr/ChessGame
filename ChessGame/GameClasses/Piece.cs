using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ChessGame.GameClasses
{
    /// <summary>
    /// Base class for all chess pieces.
    /// </summary>
    internal abstract class Piece : UserControl
    {
        private bool _isEnemy;

        /// <summary>
        /// Constructor for the Piece class.
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
        /// Constructor for the Piece class.
        /// </summary>
        /// <param name="color">The color of the piece.</param>
        /// <param name="coordinate">The coordinate of the piece.</param>
        protected Piece(PieceColor color, Coordinate coordinate) : this(color, coordinate.Row, coordinate.Column)
        {
        }

        /// <summary>
        /// Color of the piece.
        /// </summary>
        public PieceColor Color { get; }

        /// <summary>
        /// Valid moves of the piece.
        /// </summary>
        internal protected List<Coordinate> ValidMoves { get; } = new();

        /// <summary>
        /// White image of the piece.
        /// </summary>
        protected abstract ImageBrush WhiteImage { get; }

        /// <summary>
        /// Black image of the piece.
        /// </summary>
        protected abstract ImageBrush BlackImage { get; }

        /// <summary>
        /// Coordinate of the piece.
        /// </summary>
        internal protected Coordinate Coordinate { get; internal set; }

        /// <summary>
        /// Event occurs when the piece is clicked.
        /// </summary>
        internal static event LastClickedHandler? LastClicked;

        /// <summary>
        /// Moves the piece to the specified coordinate.
        /// </summary>
        /// <param name="newCoordinate">The new coordinate of the piece.</param>
        public void MoveTo(Coordinate newCoordinate)
        {
            this.HideValidMoves();
            Coordinate oldCoordinate = this.Coordinate;
            ChessBoard.Board[newCoordinate.Row, newCoordinate.Column]     = this;
            ChessBoard.Board[this.Coordinate.Row, this.Coordinate.Column] = null;
            this.Coordinate                                               = newCoordinate;
            ChessBoard.OnBoardChanged(this, new BoardChangedEventArgs(oldCoordinate, newCoordinate));
            this.ChangePlayer();
        }

        private void ChangePlayer()
        {
            if (this.Color == PieceColor.White)
            {
                LockPieces(PieceColor.White);
                return;
            }

            LockPieces(PieceColor.Black);
        }

        private static void LockPieces(PieceColor color)
        {
            foreach (Piece? piece in ChessBoard.Board)
            {
                if (piece == null)
                {
                    continue;
                }

                if (piece.Color == color)
                {
                    piece.IsEnabled = false;
                    UnsetEnemy(piece);
                    continue;
                }

                piece.IsEnabled = true;
            }
        }

        /// <summary>
        /// Moves the piece to the specified coordinate.
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
                Piece? place = ChessBoard.GetPieceOrNull(row, column);
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

        /// <summary>
        /// Updates the valid moves of the piece.
        /// </summary>
        protected abstract void UpdateValidMoves();

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
            piece.ShowValidMoves();
            LastClicked?.Invoke(piece, e);
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
            if (piece._isEnemy)
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
                Piece? place = ChessBoard.GetPieceOrNull(coordinate);
                if (place == null)
                {
                    _ = new ValidMove(this.Color, coordinate);
                    continue;
                }

                if (place.Color == this.Color)
                {
                    continue;
                }

                SetEnemy(place);
            }
        }

        private static void SetEnemy(Piece place)
        {
            place._isEnemy    = true;
            place.BorderBrush = Brushes.Red;
            place.IsEnabled   = true;
        }

        private void HideValidMoves()
        {
            foreach (Coordinate coordinate in this.ValidMoves)
            {
                Piece? place = ChessBoard.GetPieceOrNull(coordinate);
                if (place == null)
                {
                    continue;
                }

                if (place.Color == this.Color)
                {
                    ChessBoard.RemovePiece(coordinate);
                    continue;
                }

                UnsetEnemy(place);
            }
        }

        private static void UnsetEnemy(Piece place)
        {
            place._isEnemy    = false;
            place.BorderBrush = Brushes.Transparent;
            place.IsEnabled   = false;
        }

        private static void ChessBoard_BoardChanged(Piece sender, BoardChangedEventArgs e)
        {
            sender.UpdateValidMoves();
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
        /// Handler for the BoardChangedEvent.
        /// </summary>
        internal delegate void LastClickedHandler(Piece sender, RoutedEventArgs e);
    }

    /// <summary>
    /// Enum for the piece colors.
    /// </summary>
    internal enum PieceColor
    {
        White,
        Black
    }
}
