using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ChessGame.GameClasses;

public sealed class ValidMove : UserControl, IDisposable
{
    private readonly ChessBoard _board;

    private ValidMove(ChessBoard board, int row, int column)
    {
        _board = board;
        MouseEnter += ValidMove_MouseEnter;
        MouseLeave += ValidMove_MouseLeave;
        MouseLeftButtonUp += ValidMove_MouseLeftButtonUp;
        Coordinate = new Coordinate(row, column);
        Cursor = Cursors.Hand;
        BorderThickness = new Thickness(1);
        Background = Image;
        Focusable = true;
        FocusVisualStyle = null;
        ShowValidMove?.Invoke(this, EventArgs.Empty);
    }

    public ValidMove(ChessBoard board, Coordinate coordinate) : this(board, coordinate.Row, coordinate.Column)
    {
    }

    public Coordinate Coordinate { get; }

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

    public static event ValidMoveEventHandler? ShowValidMove;

    private void ValidMove_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        var validMove = (ValidMove)sender;
        var coordinate = validMove.Coordinate;
        _board.LastClickedPiece?.MoveTo(coordinate);

        Dispose();
    }

    private void ValidMove_MouseEnter(object sender, MouseEventArgs e)
    {
        Background = RectangleBrush;
    }

    private void ValidMove_MouseLeave(object sender, MouseEventArgs e)
    {
        Background = CircleBrush;
    }

    public delegate void ValidMoveEventHandler(ValidMove sender, EventArgs e);

    public void Dispose()
    {
        MouseEnter -= ValidMove_MouseEnter;
        MouseLeave -= ValidMove_MouseLeave;
        MouseLeftButtonUp -= ValidMove_MouseLeftButtonUp;

        Cursor = Cursors.Arrow;
        BorderThickness = new Thickness(0);
        Background = Brushes.Transparent;
        Focusable = false;
    }
}