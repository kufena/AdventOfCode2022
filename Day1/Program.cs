// See https://aka.ms/new-console-template for more information


Console.WriteLine("Hello, Elves!");

var file = File.ReadAllLines(args[0]);
long largest = Part1(file);

Console.WriteLine($"Largest is {largest}");

long topthree = Part2(file);
Console.WriteLine($"tOP tHREE tOTAL iS {topthree}");

static long Part1(string[] file)
{
    long largest = 0;
    long total = 0;

    foreach (var line in file)
    {
        if (line.Trim().Equals(""))
        {
            if (total > largest)
                largest = total;

            total = 0;
        }
        else
        {
            long l = long.Parse(line.Trim());
            total += l;
        }
    }

    if (total > largest)
        largest = total;
    return largest;
}

static long Part2(string[] file)
{
    List<long> totals = new();
    long total = 0;

    foreach (var line in file)
    {
        if (line.Trim().Equals(""))
        {
            totals.Add(total);
            total = 0;
        }
        else
        {
            long l = long.Parse(line.Trim());
            total += l;
        }
    }

    totals.Add(total);

    totals.Sort();
    totals.Reverse();
    long topthree = totals[0] + totals[1] + totals[2];

    return topthree;
}