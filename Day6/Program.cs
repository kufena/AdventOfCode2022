// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");
var lines = File.ReadAllLines(args[0]);

//long pos = Part1(lines);
long pos = Part2(lines);

long Part1(string[] lines)
{

    foreach (var line in lines)
    {
        int l = line.Length;
        Console.WriteLine(line);
        var chararr = line.ToCharArray();
        for (int i = 3; i < l; i++)
        {
            HashSet<char> s = new HashSet<char>();
            s.Add(chararr[i]);
            s.Add(chararr[i - 1]);
            s.Add(chararr[i - 2]);
            s.Add(chararr[i - 3]);
            Console.WriteLine($"Hashset of {chararr[i]} {chararr[i - 1]} {chararr[i - 2]} {chararr[i - 3]}");
            if (s.Count == 4) // 4 unique chars
            {
                Console.WriteLine($"{i+1}");
                break;
            }
        }
    }
    return 1;
}

long Part2(string[] lines)
{

    foreach (var line in lines)
    {
        int l = line.Length;
        Console.WriteLine(line);
        var chararr = line.ToCharArray();
        for (int i = 13; i < l; i++)
        {
            HashSet<char> s = new HashSet<char>();
            for (int j = 0; j < 14; j++)
            {
                s.Add(chararr[i - j]);
            }
            Console.WriteLine($"Hashset of {s.Count} {i}");
            if (s.Count == 14) // 14 unique chars
            {
                Console.WriteLine($"{i+1}");
                break;
            }
        }
    }
    return 1;
}
