using System;

namespace ChessGame.GameClasses;

public sealed class Coordinate
{
    private readonly int _columnCoordinate;

    private readonly int _rowCoordinate;

    internal Coordinate(int coordinateRow, int coordinateColumn)
    {
        Row = coordinateRow;
        Column = coordinateColumn;
    }

    internal int Row
    {
        get => _rowCoordinate;

        private init
        {
            if (value is < 0 or > 7)
            {
                throw new ArgumentOutOfRangeException(nameof(value),
                    value,
                    "Row coordinate must be between 0 and 7");
            }

            _rowCoordinate = value;
        }
    }

    internal int Column
    {
        get => _columnCoordinate;

        private init
        {
            if (value is < 0 or > 7)
            {
                throw new ArgumentOutOfRangeException(nameof(value),
                    value,
                    "Column coordinate must be between 0 and 7");
            }

            _columnCoordinate = value;
        }
    }

    public override string ToString()
        => $"{Row} : {Column}";

    public static bool IsCorrectCoordinate(int row, int column)
        => row >= 0 && column >= 0 && row < 8 && column < 8;
}