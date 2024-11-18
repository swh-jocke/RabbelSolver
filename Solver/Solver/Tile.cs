namespace Solver
{
    public readonly struct Tile(int x, int y, string letter)
    {
        public int X { get; } = x;
        public int Y { get; } = y;
        public string Letter { get; } = letter;
    }
}
