using System;

namespace ChessGame.GameClasses
{
    public sealed class Coordinate
    {
        private readonly int _columnCoordinate;

        private readonly int _rowCoordinate;

        internal Coordinate(int coordinateRow, int coordinateColumn)
        {
            this.Row    = coordinateRow;
            this.Column = coordinateColumn;
        }

        internal int Row
        {
            get { return this._rowCoordinate; }
            private init
            {
                if (value is < 0 or > 7)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value,
                                                          "Row coordinate must be between 0 and 7");
                }

                this._rowCoordinate = value;
            }
        }

        internal int Column
        {
            get { return this._columnCoordinate; }
            private init
            {
                if (value is < 0 or > 7)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value,
                                                          "Column coordinate must be between 0 and 7");
                }

                this._columnCoordinate = value;
            }
        }
    }
}
