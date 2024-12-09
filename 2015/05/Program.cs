var lines = File.ReadAllLines(args[0]);

HashSet<string> badStrings = [
    "ab", "cd", "pq", "xy"
];
int nice = lines.Where(l => IsNice(l, badStrings)).Count();
Console.WriteLine($"Nice: {nice}");


static bool IsNice(string s, HashSet<string> badStrings)
{
    int cVowels = 0;
    int cDoubled = 0;
    int cBad = 0;
    for (int i = 0; i < s.Length; ++i)
    {
        if ("aeiou".Contains(s[i]))
            ++cVowels;
        if (i > 0)
        {
            if (s[i] == s[i - 1])
                ++cDoubled;
            foreach (var bad in badStrings)
            {
                if (s[i - 1] == bad[0] && s[i] == bad[1])
                    ++cBad;
            }
        }
    }
    return cVowels >= 3 && cDoubled > 0 && cBad == 0;
}
