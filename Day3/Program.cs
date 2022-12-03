// See https://aka.ms/new-console-template for more information
using System.Reflection.Metadata;

Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines(args[0]);

long part1total = Part1(lines);
Console.WriteLine($"Total is {part1total}");

long part2total = Part2(lines);
Console.WriteLine($"Total is {part2total}");

long Part1(string[] lines)
{
    long total = 0;
    foreach (var line in lines)
    {
        if (line.Length % 2 != 0)
            throw new Exception("non-2'ed string");
        var left = line.Substring(0, line.Length / 2);
        var right = line.Substring(line.Length / 2, line.Length / 2);
        var destring = Destring(left);

        foreach (var c in right)
        {
            if (destring.ContainsKey(c))
            {
                int x = LetterToChar(c);

                Console.Out.WriteLine($"Found {c} so adding {x}");
                total += (long)x;
                break;
            }
        }
    }
    return total;
}

long Part2(string[] lines)
{
    long result = 0;

    for (int i = 0; i < lines.Length; i += 3)
    {
        var firstline = Destring(lines[i]);
        HashSet<char> second = new();
        foreach (var c in lines[i + 1])
        {
            if (firstline.ContainsKey(c))
                second.Add(c); // we're not really bothered about the counts are we?
        }
        HashSet<char> third = new();
        foreach (var c in lines[i + 2])
        {
            if (second.Contains(c))
                third.Add(c);
        }

        if (third.Count != 1) {
            throw new Exception("Eek!");
        }

        result += LetterToChar(third.First());
    }
    return result;
}

Dictionary<char,int> Destring(string s)
{
    Dictionary<char, int> result = new();
    foreach (char c in s)
    {
        if (result.ContainsKey(c))
            result[c] += 1;
        else
            result.Add(c, 1);
    }
    return result;
}

static int LetterToChar(char c)
{
    int x = 0;
    if (Char.IsUpper(c))
        x = ((int)c) - ((int)'A') + 27;
    else
        x = ((int)c) - ((int)'a') + 1;
    return x;
}