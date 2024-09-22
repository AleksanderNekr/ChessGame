using System;
using System.Collections.Generic;
using System.Linq;
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
        var newBoardPresenter = GetNewBoardPresenter(BoardPresenter, _board);
        DrawGraph();
    }

    private void DrawGraph()
    {
        var treeWindow = new TreeWindow();

        var root = new TreeNode<string>(
            "Root",
            new[]
            {
                new TreeNode<string>(
                    "Child 1",
                    new[]
                    {
                        new TreeNode<string>("Child 1.1", Array.Empty<TreeNode<string>>()),
                        new TreeNode<string>("Child 1.2", Array.Empty<TreeNode<string>>()),
                    }),
                new TreeNode<string>(
                    "Child 2",
                    new[]
                    {
                        new TreeNode<string>("Child 2.1", Array.Empty<TreeNode<string>>()),
                        new TreeNode<string>("Child 2.2", Array.Empty<TreeNode<string>>()),
                    }),
            }
        );

        treeWindow.DrawTree(root);

        ValidMove.ShowValidMove -= ShowValidMoveShowValidMove;
        ValidMove.HideValidMove -= HideValidMoveHideValidMove;

        treeWindow.ShowDialog();

        treeWindow.Closing += (_, _) =>
        {
            ValidMove.ShowValidMove += ShowValidMoveShowValidMove;
            ValidMove.HideValidMove += HideValidMoveHideValidMove;
        };
    }

    private static Grid GetNewBoardPresenter(Grid sourceBoardPresenter, ChessBoard sourceChessBoard)
    {
        var newBoardPresenter = new Grid
        {
            Height = 250,
            Width = 250,
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
        foreach (var piece in sourceChessBoard.GetPlayerPieces(PieceColor.Black).Union(sourceChessBoard.GetPlayerPieces(PieceColor.White)))
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


    private void Mate2MovesEasy_Click(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }
}