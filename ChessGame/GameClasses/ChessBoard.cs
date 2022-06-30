namespace ChessGame.GameClasses
{
    internal static class ChessBoard
    {
        public static Piece? GetPieceOrNull(int row, int column)
        {
            var coord = new Coordinate(row, column);
            return Board[coord.Row, coord.Column];
        }
        public static Piece? GetPieceOrNull(Coordinate coordinate)
        {
            var coord = new Coordinate(coordinate.Row, coordinate.Column);
            return Board[coord.Row, coord.Column];
        }

        public static void SetPiece(Piece piece, int row, int column)
        {
            var coord = new Coordinate(row, column);
            Board[coord.Row, coord.Column] = piece;
        }

        public static void SetPiece(Piece piece, Coordinate coordinate)
        {
            var coord = new Coordinate(coordinate.Row, coordinate.Column);
            Board[coord.Row, coord.Column] = piece;
        }

        private static readonly Piece?[,] Board = new Piece?[8, 8];
    }
}
