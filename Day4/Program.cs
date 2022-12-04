// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines(args[0]);

//long result = Part1(lines);
long result = Part2(lines);
Console.WriteLine(result);

long Part1(string[] lines)
{
    long count = 0;

    foreach (string line in lines)
    {
        var elfbits = line.Split(',');
        var elf1 = elfbits[0].Split("-");
        var elf2 = elfbits[1].Split("-");

        long elf1A = long.Parse(elf1[0]);
        long elf1B = long.Parse(elf1[1]);
        long elf2A = long.Parse(elf2[0]);
        long elf2B = long.Parse(elf2[1]);

        if (elf1A <= elf2A && elf2B <= elf1B)
        {
            count++;
            Console.WriteLine($"First Clause - ({elf1A},{elf1B}) is contained in ({elf2A},{elf2B})");
        }
        else if (elf2A <= elf1A && elf1B <= elf2B)
        {
            count++;
            Console.WriteLine($"Second Clause - ({elf1A},{elf1B}) is contained in ({elf2A},{elf2B})");
        }
    }

    return count;
}

long Part2(string[] lines)
{
    long count = 0;

    foreach (string line in lines)
    {
        var elfbits = line.Split(',');
        var elf1 = elfbits[0].Split("-");
        var elf2 = elfbits[1].Split("-");

        long elf1A = long.Parse(elf1[0]);
        long elf1B = long.Parse(elf1[1]);
        long elf2A = long.Parse(elf2[0]);
        long elf2B = long.Parse(elf2[1]);

        if (elf1A <= elf2A && elf2B <= elf1B)
        {
            count++;
            Console.WriteLine($"First Clause - ({elf1A},{elf1B}) is contained in ({elf2A},{elf2B})");
        }
        else if (elf2A <= elf1A && elf1B <= elf2B)
        {
            count++;
            Console.WriteLine($"Second Clause - ({elf1A},{elf1B}) is contained in ({elf2A},{elf2B})");
        }
        else if (elf1A <= elf2A && elf1B <= elf2B && elf2A <= elf1B)
        {
            count++;
            Console.WriteLine($"Third Clause - ({elf1A},{elf1B}) is overlapping in ({elf2A},{elf2B})");
        }
        else if (elf2A <= elf1A && elf2B <= elf1B && elf1A <= elf2B)
        {
            count++;
            Console.WriteLine($"Fourth Clause - ({elf1A},{elf1B}) is overlapping in ({elf2A},{elf2B})");
        }
    }

    return count;
}