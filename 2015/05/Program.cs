if (args.Length == 0)
{
    interactive();
}
else
{
    batch(args[0]);
}

static void interactive()
{
    var line = Console.ReadLine();
    while (line is not null && line.Length > 0)
    {
        bool isNice = IsNice2(line, null!);
        Console.WriteLine(isNice ? "1 nice" : "0 naughty");
        line = Console.ReadLine();
    }
}

static void batch(string filename)
{
    var lines = File.ReadAllLines(filename);

    HashSet<string> badStrings = [
        "ab", "cd", "pq", "xy"
    ];
    foreach (var line in lines)
    {
        Console.WriteLine("{0} {1}", line, IsNice2(line, badStrings) ? "X" : "_");
    }
    int nice = lines.Where(l => IsNice2(l, badStrings)).Count();
    Console.WriteLine($"Nice: {nice}");
}

static bool IsNice2(string s, HashSet<string> badStrings)
{
    return HasPairOccuringTwice(s) && RepeatingLetter(s);
}

static bool HasPairOccuringTwice(string s)
{
    for (int i = 0; i < s.Length - 3; ++i)
    {
        char ch1 = s[i];
        char ch2 = s[i + 1];
        for (int j = i + 2; j < s.Length - 1; ++j)
        {
            if (ch1 == s[j] && ch2 == s[j + 1])
            {
                return true;
            }
        }
    }
    return false;
}

static bool RepeatingLetter(string s)
{
    for (int i = 0; i < s.Length - 2; ++i)
    {
        if (s[i] == s[i + 2])
            return true;
    }
    return false;
}



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
