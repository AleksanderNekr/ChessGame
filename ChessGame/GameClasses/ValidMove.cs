using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ChessGame.GameClasses;

internal sealed class ValidMove : UserControl
{
    private ValidMove(int row, int column)
    {
        MouseEnter += ValidMove_MouseEnter;
        MouseLeave += ValidMove_MouseLeave;
        MouseLeftButtonUp += ValidMove_MouseLeftButtonUp;
        Piece.LastClicked += Piece_LastClicked;
        Coordinate = new Coordinate(row, column);
        Cursor = Cursors.Hand;
        BorderThickness = new Thickness(1);
        Background = Image;
        Focusable = true;
        FocusVisualStyle = null;
        ShowValidMove?.Invoke(this, EventArgs.Empty);
    }

    public ValidMove(Coordinate coordinate) : this(coordinate.Row, coordinate.Column)
    {
    }

    internal Coordinate Coordinate { get; }

    private static ImageBrush Image { get; } = CircleBrush;

    private static ImageBrush CircleBrush
    {
        get
        {
            var circle = new GeometryDrawing(new SolidColorBrush(),
                new Pen(Brushes.Chartreuse, 1),
                new EllipseGeometry());
            var image = new ImageBrush(new DrawingImage(circle))
            {
                Viewport = new Rect(0.35, 0.35, 0.3, 0.3),
                Opacity = 0.4,
            };
            return image;
        }
    }

    internal static Piece? LastClickedPiece { get; private set; }

    private static ImageBrush RectangleBrush
    {
        get
        {
            var rect = new GeometryDrawing(new SolidColorBrush(),
                new Pen(Brushes.Chartreuse, 1),
                new RectangleGeometry(new Rect()));

            var brush = new ImageBrush(new DrawingImage(rect)) { Opacity = 0.2 };
            return brush;
        }
    }

    internal static event ValidMoveEventHandler? ShowValidMove;
    internal static event ValidMoveEventHandler? HideValidMove;

    private static void Piece_LastClicked(Piece sender, RoutedEventArgs e)
    {
        LastClickedPiece = sender;
    }

    private static void ValidMove_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (LastClickedPiece == null)
        {
            throw new NullReferenceException("LastClickedPiece is null.");
        }

        var validMove = (ValidMove)sender;
        var coordinate = validMove.Coordinate;
        LastClickedPiece.MoveTo(coordinate);
    }

    private void ValidMove_MouseEnter(object sender, MouseEventArgs e)
    {
        Background = RectangleBrush;
    }

    private void ValidMove_MouseLeave(object sender, MouseEventArgs e)
    {
        Background = CircleBrush;
    }

    internal void Hide()
    {
        HideValidMove?.Invoke(this, EventArgs.Empty);
    }

    internal delegate void ValidMoveEventHandler(ValidMove sender, EventArgs e);
}