using System;
using System.Windows;
using System.Windows.Controls;
using ChessGame.GameClasses;
using DecisionBuilder;

namespace ChessGame;

/// <inheritdoc cref="System.Windows.Window" />
internal sealed partial class MainWindow
{
    private readonly ChessBoard _board;

    public MainWindow()
    {
        InitializeComponent();
        _board = new ChessBoard();
        _board.AfterBoardChanged += AfterBoardChanged;
        ValidMove.ShowValidMove += ShowValidMoveShowValidMove;
        ValidMove.HideValidMove += HideValidMoveHideValidMove;
    }

    private void AfterBoardChanged()
    {
        UpdateGridBoard();
    }

    private void HideValidMoveHideValidMove(ValidMove sender, EventArgs e)
    {
        UpdateGridBoard();
    }

    private void ShowValidMoveShowValidMove(ValidMove sender, EventArgs e)
    {
        SetPieceToBoard(sender, sender.Coordinate.Row, sender.Coordinate.Column);
    }

    private void UpdateGridBoard()
    {
        BoardPresenter.Children.Clear();
        for (var i = 0; i < ChessBoard.Size; i++)
        {
            for (var j = 0; j < ChessBoard.Size; j++)
            {
                UserControl? control = _board.GetPieceOrNull(i, j);
                if (control == null)
                {
                    continue;
                }

                SetPieceToBoard(control, i, j);
            }
        }
    }

    private void SetPieceToBoard(UserControl control, int i, int j)
    {
        Grid.SetRow(control, i);
        Grid.SetColumn(control, j);
        BoardPresenter.Children.Add(control);
    }

    private void ButtonBase_Click(object sender, RoutedEventArgs e)
    {
        ResetPreset();
    }

    private void ResetPreset()
    {
        _board.Clear();
        _board.AfterBoardChanged -= AfterBoardChanged;

        SetPawns();
        SetKnights();
        SetBishops();
        SetRooks();
        SetQueens();
        SetKings();

        _board.AfterBoardChanged += AfterBoardChanged;
        _board.OnBoardChanged();
    }

    private void SetKings()
    {
        _ = new King(_board, PieceColor.Black, 0, 4);
        _ = new King(_board, PieceColor.White, 7, 4);
    }

    private void SetQueens()
    {
        _ = new Queen(_board, PieceColor.Black, 0, 3);
        _ = new Queen(_board, PieceColor.White, 7, 3);
    }

    private void SetRooks()
    {
        _ = new Rook(_board, PieceColor.Black, 0, 0);
        _ = new Rook(_board, PieceColor.Black, 0, 7);
        _ = new Rook(_board, PieceColor.White, 7, 0);
        _ = new Rook(_board, PieceColor.White, 7, 7);
    }

    private void SetBishops()
    {
        _ = new Bishop(_board, PieceColor.White, 7, 2);
        _ = new Bishop(_board, PieceColor.White, 7, 5);
        _ = new Bishop(_board, PieceColor.Black, 0, 2);
        _ = new Bishop(_board, PieceColor.Black, 0, 5);
    }

    private void SetKnights()
    {
        _ = new Knight(_board, PieceColor.White, 7, 1);
        _ = new Knight(_board, PieceColor.White, 7, 6);
        _ = new Knight(_board, PieceColor.Black, 0, 1);
        _ = new Knight(_board, PieceColor.Black, 0, 6);
    }

    private void SetPawns()
    {
        for (var i = 0; i < 8; i++)
        {
            _ = new Pawn(_board, PieceColor.White, 6, i);
        }

        for (var i = 0; i < 8; i++)
        {
            _ = new Pawn(_board, PieceColor.Black, 1, i);
        }
    }

    private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        var newSize = Math.Min(e.NewSize.Width, e.NewSize.Height) - 140;
        BoardPresenter.Width = newSize;
        BoardPresenter.Height = newSize;
    }

    private void ShowTree_Click(object sender, RoutedEventArgs e)
    {
        var graph = TreeWindow.NewGraph<Grid>();
        var board = new ChessBoard();

        var boardPresenter = BoardPresenter;
        var grid = new Grid
        {
            Width = boardPresenter.Width,
            Height = boardPresenter.Height,
            Background = boardPresenter.Background,
        };

        foreach (UIElement boardChild in boardPresenter.Children)
        {
            if (boardChild is Piece piece)
            {
                switch (piece)
                {
                    case Pawn pawn:
                        grid.Children.Add(new Pawn(board, pawn.Color, pawn.Coordinate.Row, pawn.Coordinate.Column));
                        break;
                    case Knight knight:
                        grid.Children.Add(new Knight(board, knight.Color, knight.Coordinate.Row, knight.Coordinate.Column));
                        break;
                    case Bishop bishop:
                        grid.Children.Add(new Bishop(board, bishop.Color, bishop.Coordinate.Row, bishop.Coordinate.Column));
                        break;
                    case Rook rook:
                        grid.Children.Add(new Rook(board, rook.Color, rook.Coordinate.Row, rook.Coordinate.Column));
                        break;
                    case Queen queen:
                        grid.Children.Add(new Queen(board, queen.Color, queen.Coordinate.Row, queen.Coordinate.Column));
                        break;
                    case King king:
                        grid.Children.Add(new King(board, king.Color, king.Coordinate.Row, king.Coordinate.Column));
                        break;
                }
            }
        }

        TreeWindow.AddTreeRoot(graph, grid);
        var tree = new TreeWindow();
        tree.BuildGraph(graph);
        tree.ShowDialog();
    }
}