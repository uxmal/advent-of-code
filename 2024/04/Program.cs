var board = ReadBoard(args[0]);
// int count = WordSearch(board, "XMAS", CountWordsAt);
int count = WordSearch(board, "MAS", CountCrossAt);


Console.WriteLine($"Count: {count}");

static string[] ReadBoard(string filename)
{
    return File.ReadAllLines(filename);
}

static int WordSearch(
    string[] board,
    string word,
    Func<string[], int, int, string, int> eval)
{
    int count = 0;
    for (int y = 0; y < board.Length; ++y)
    {
        var row = board[y];
        int rowCount = 0;
        for (int x = 0; x < row.Length; ++x)
        {
            rowCount += eval(board, y, x, word);
        }
        count += rowCount;
    }
    return count;
}

static int CountWordsAt(string[] board, int y, int x, string word)
{
    int count = 0;
    if (IsWordAt(board, y, x, word, 0, 1))
        ++count;
    if (IsWordAt(board, y, x, word, 1, 1))
        ++count;
    if (IsWordAt(board, y, x, word, 1, 0))
        ++count;
    if (IsWordAt(board, y, x, word, 1, -1))
        ++count;
    if (IsWordAt(board, y, x, word, 0, -1))
        ++count;
    if (IsWordAt(board, y, x, word, -1, -1))
        ++count;
    if (IsWordAt(board, y, x, word, -1, 0))
        ++count;
    if (IsWordAt(board, y, x, word, -1, 1))
        ++count;
    return count;
}

/*
M M M S S M S S 
 A   A   A   A
S S M S S M M M
*/
int CountCrossAt(string[] board, int y, int x, string word)
{
    int count = 0;
    if (IsWordAt(board, y, x, word, 1, 1) &&
        IsWordAt(board, y, x+2, word, 1, -1))
        ++count;
    if (IsWordAt(board, y, x, word, 1, 1) &&
        IsWordAt(board, y+2, x, word, -1, 1))
        ++count;
    if (IsWordAt(board, y, x+2, word, 1, -1) &&
        IsWordAt(board, y+2, x+2, word, -1, -1))
        ++count;
    if (IsWordAt(board, y+2, x, word, -1, 1) &&
        IsWordAt(board, y+2, x+2, word, -1, -1))
        ++count;
    return count;
}

static bool IsWordAt(string[] board, int y, int x, string word, int dy, int dx)
{
    foreach (var ch in word)
    {
        if (0 <= y && y < board.Length)
        {
            var row = board[y];
            if (0 <= x && x < row.Length)
            {
                if (ch == row[x])
                {
                    y += dy;
                    x += dx;
                    continue;
                }
            }
        }
        return false;
    }
    return true;
}