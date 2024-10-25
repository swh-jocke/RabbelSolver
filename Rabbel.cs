namespace RabbelSolver
{
    public class Rabbel(Settings settings)
    {
        private string[] _wordList = GetWordList();
        private readonly List<Tile> _board = GetBoard(settings.Letters);
        private readonly int _minLength = settings.MinLength;
        private readonly int _maxLength = settings.MaxLength;

        private List<string> Letters
        {
            get
            {
                return _board.Select(x => x.Letter).ToList();
            }            
        }

        public List<string> Solve()
        {
            _wordList = ScrubWordList();
            var result = new List<string>();

            Parallel.ForEach(_board, tile =>
            {
                var words = Process(tile);
                lock (result)
                {
                    foreach (var word in words)
                    {
                        result.Add(word);
                    }                    
                }
            });

            return result.Distinct().ToList();
        }

        private string[] ScrubWordList()
        {
            return _wordList
                .Where(x => x.Length >= _minLength && x.Length <= _maxLength && x.Any(x => Letters.Contains(x.ToString())))
                .ToArray();
        }

        private static List<Tile> GetBoard(string lettersString)
        {
            var rows = 4;
            var cols = 4;
            var letters = lettersString.Split(',');
            var board = new List<Tile>();

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    board.Add(new Tile(j, i, letters[i * cols + j]));
                }
            }

            return board;
        }

        private static string[] GetWordList()
        {
            var text = File.ReadAllText("rabbel_ord.txt");
            return text.Split(',')
                .Select(x => x.Trim().ToLower())
                .Select(x => x.Replace("\"", ""))
                .ToArray();
        }

        private List<string> Process(Tile tile)
        {
            var result = new List<string>();
            GoToTile(tile, 0, 0, "", result, []);
            return result;
        }

        private void GoToTile(Tile currentTile, int dirX, int dirY, string part, List<string> result, List<(int x, int y)> visited)
        {
            if (currentTile.X + dirX < 0 || 
                currentTile.X + dirX > 3 || 
                currentTile.Y + dirY < 0 || 
                currentTile.Y + dirY > 3)
            {
                return;
            }

            currentTile = _board
                .Where(x => x.X == currentTile.X + dirX && x.Y == currentTile.Y + dirY)
                .First();

            if (visited.Any(x => x.x == currentTile.X && x.y == currentTile.Y))
            {
                return;
            }

            part += currentTile.Letter;

            if (part.Length > _maxLength)
            {
                return;
            }

            if (string.IsNullOrEmpty(part) || _wordList.Any(x => x.StartsWith(part)))
            {
                if(_wordList.Any(x => x == part && x.Length >= _minLength))
                {
                    result.Add(part);
                }

                // Add current tile to the visited list
                visited.Add((currentTile.X, currentTile.Y));

                // Down
                GoToTile(currentTile, 0, 1, part, result, visited);

                // Up
                GoToTile(currentTile, 0, -1, part, result, visited);

                // Right
                GoToTile(currentTile, 1, 0, part, result, visited);

                // Left
                GoToTile(currentTile, -1, 0, part, result, visited);

                // Down right
                GoToTile(currentTile, 1, 1, part, result, visited);

                // Down left
                GoToTile(currentTile, -1, 1, part, result, visited);

                // Up right
                GoToTile(currentTile, 1, -1, part, result, visited);

                // Up left
                GoToTile(currentTile, -1, -1, part, result, visited);

                visited.RemoveAt(visited.Count - 1);
            }
        }
    }
}
