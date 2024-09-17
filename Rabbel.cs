
using System.ComponentModel;

namespace RabbelSolver
{
    public class Rabbel(Settings settings)
    {
        private readonly string[] _wordList = GetWordList();
        private readonly List<Tile> _board = GetBoard(settings.Letters);
        private readonly int _minLength = settings.MinLength;
        private readonly int _maxLength = settings.MaxLength;

        public List<string> Solve()
        {
            var result = new List<string>();

            foreach (var tile in _board)
            {
                result.AddRange(Process(tile));
            }

            return result.Distinct().ToList();
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

                var newRef = new List<(int x, int y)>(visited)
                {
                    (currentTile.X, currentTile.Y)
                };

                // Down
                GoToTile(currentTile, 0, 1, part, result, newRef);

                // Up
                GoToTile(currentTile, 0, -1, part, result, newRef);

                // Right
                GoToTile(currentTile, 1, 0, part, result, newRef);

                // Left
                GoToTile(currentTile, -1, 0, part, result, newRef);

                // Down right
                GoToTile(currentTile, 1, 1, part, result, newRef);

                // Down left
                GoToTile(currentTile, -1, 1, part, result, newRef);

                // Up right
                GoToTile(currentTile, 1, -1, part, result, newRef);

                // Up left
                GoToTile(currentTile, -1, -1, part, result, newRef);
            }
        }
    }
}
