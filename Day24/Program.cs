// See https://aka.ms/new-console-template for more information
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines(args[0]);

string directions = "<>^v";
HashSet<(int, int)> moves = new HashSet<(int, int)> { (0,0), (1, 0), (-1, 0), (0, 1), (0, -1) };
HashSet<(int, int)> targets = new();
List<(int, int, int)> blizzards = new();

int rows = lines.Length - 2;
int cols = lines[0].Length - 2;
// we'll set origin of (0,0) as inside the square excluding borders.

for (int i = 1; i < lines.Length - 1; i++)
{
    for (int j = 1; j < lines[i].Length - 1; j++)
    {
        if (directions.Contains(lines[i][j]))
        {
            // it's a blizzard
            blizzards.Add((directions.IndexOf(lines[i][j]), i - 1, j - 1));
        }
    }
}

int gcd = GCD(rows, cols);
int LCM = (rows * cols) / gcd;

Dictionary<int, List<(int, int, int)>> clockToBlizzards = CalcClockToBlizzards(blizzards, LCM, rows, cols, directions);

targets.Add((rows, cols - 1));

//Part1(moves, targets, rows, cols, LCM, clockToBlizzards);
Part2(moves, rows, cols, LCM, clockToBlizzards);


// Nicked from Stack Overflow - why is this not in Math?
static int GCD(int a, int b)
{
    while (a != 0 && b != 0)
    {
        if (a > b)
            a %= b;
        else
            b %= a;
    }

    return a | b;
}

static int remainder(int a, int b)
{
    //Console.WriteLine($"For {a} and {b}, {Math.IEEERemainder(a, b)} vs {a % b}"); ;
    if (a >= 0) return a % b;
    else
    {
        int x = a % b;
        if (x != 0)
            return b - (a % b); // (Math.Abs(a - b) % b);
        else
            return 0;
    }
}

Dictionary<int, List<(int, int, int)>> CalcClockToBlizzards(List<(int, int, int)> blizzards, int lCM, int rows, int cols, string directions)
{
    var result = new Dictionary<int, List<(int, int, int)>>();
    result.Add(0, blizzards);
    List<(int, int, int)> lastBlizzards = blizzards;
    //PrintGrid(blizzards, rows, cols, directions);
    int clock = 1;
    for (int i = 1; i < LCM; i++)
    {
        List<(int, int, int)> newBlizzards = new();
        foreach ((int type, int r, int c) in lastBlizzards)
        {
            // <>^v
            switch (type)
            {
                case 0:
                    int nc = c - 1;
                    if (nc < 0) nc = cols - 1;
                    newBlizzards.Add((type, r, nc));
                    break;
                case 1:
                    int nc2 = c + 1;
                    if (nc2 >= cols) nc2 = 0;
                    newBlizzards.Add((type, r, nc2));
                    break;
                case 2:
                    int nr = r - 1;
                    if (nr < 0) nr = rows - 1;
                    newBlizzards.Add((type, nr, c));
                    break;
                case 3:
                    int nr2 = r + 1;
                    if (nr2 >= rows) nr2 = 0;
                    newBlizzards.Add((type, nr2, c));
                    break;
                default:
                    throw new Exception("Unknown blizzard type.");
            }
        }
        result.Add(clock, newBlizzards);
        if (lastBlizzards.Count != newBlizzards.Count) throw new Exception("we've gained or lost a blizzard.");
        lastBlizzards = newBlizzards;
        Console.WriteLine($"Clock = {clock}");
        //Console.ReadLine();
        //PrintGrid(newBlizzards, rows, cols, directions);
        clock++;
    }

    return result;
}

static void Part1(HashSet<(int, int)> moves, HashSet<(int, int)> targets, int rows, int cols, int LCM, Dictionary<int, List<(int, int, int)>> clockToBlizzards)
{
    HashSet<(int, int, int)> seen = new();
    Queue<(int, int, int)> states = new();
    states.Enqueue((-1, 0, 0));

    while (states.Count > 0)
    {
        (int cr, int cc, int time) = states.Dequeue();
        time++;
        int blizzardtime = time % LCM;
        var ourblizzards = clockToBlizzards[blizzardtime];
        foreach ((int dr, int dc) in moves)
        {
            int nr = cr + dr;
            int nc = cc + dc;
            if (targets.Contains((nr, nc)))
            {
                Console.WriteLine($"Compleed in {time}");
                return; // just quit.
            }
            if ((nr < 0 || nr >= rows || nc < 0 || nc >= cols) && !((nr, nc) == (-1, 0)))
                continue;
            //Console.WriteLine($"Checking {nr} {nc2} time {time}");
            bool Fail = false;
            if ((nr, nc) != (-1, 0))
            {
                foreach (int type in (new int[] { 0, 1, 2, 3 }))
                {
                    if (ourblizzards.Contains((type, nr, nc)))
                    {
                        Fail = true;
                        break;
                    }
                }
            }

            if (!Fail)
            {
                if (seen.Contains((nr, nc, remainder(time, LCM))))
                    continue;
                seen.Add((nr, nc, remainder(time, LCM)));
                Console.WriteLine($"We are not failing at {nr} {nc} {time}");
                states.Enqueue((nr, nc, time));
            }

        }
    }
}

static void Part2(HashSet<(int, int)> moves, int rows, int cols, int LCM, Dictionary<int, List<(int, int, int)>> clockToBlizzards)
{
    (int, int)[] targets = new (int, int)[] { (rows,cols-1),(-1,0),(rows,cols-1) };
    int nextTarget = 0;

    HashSet<(int, int, int)> seen = new();
    Queue<(int, int, int)> states = new();
    states.Enqueue((-1, 0, 0));

    while (states.Count > 0)
    {
        (int cr, int cc, int time) = states.Dequeue();
        time++;
        int blizzardtime = time % LCM;
        var ourblizzards = clockToBlizzards[blizzardtime];
        foreach ((int dr, int dc) in moves)
        {
            int nr = cr + dr;
            int nc = cc + dc;
            if ((nr,nc) == targets[nextTarget])
            {
                Console.WriteLine($"Completed target {nextTarget} in {time}");
                nextTarget++;
                if (nextTarget >= targets.Length)
                    return; // just quit.
                while (states.Count > 0)
                    states.Dequeue();
                states.Enqueue((nr, nc, time));
                break;
            }
            if ((nr < 0 || nr >= rows || nc < 0 || nc >= cols) && !(targets.Contains((nr, nc))))
                continue;
            //Console.WriteLine($"Checking {nr} {nc2} time {time}");
            
            bool Fail = false;
            if (!targets.Contains((nr, nc)))
            {
                foreach (int type in (new int[] { 0, 1, 2, 3 }))
                {
                    if (ourblizzards.Contains((type, nr, nc)))
                    {
                        Fail = true;
                        break;
                    }
                }
            }

            if (!Fail)
            {
                if (seen.Contains((nr, nc, remainder(time, LCM))))
                    continue;
                seen.Add((nr, nc, remainder(time, LCM)));
                states.Enqueue((nr, nc, time));
                Console.WriteLine($"We are not failing at {nr} {nc} {time} {states.Count}");
            }

        }
    }
}

static void PrintGrid(List<(int, int, int)> blizzards, int rows, int cols, string directions)
{
    for (int i = 0; i < rows; i++)
    {
        for (int j = 0; j < cols; j++)
        {
            int c = 0;
            char s = '.';
            foreach (var type in new int[] { 0, 1, 2, 3 })
            {
                if (blizzards.Contains((type, i, j)))
                {
                    var sub = blizzards.Where(x => (x.Item1 == type && x.Item2 == i && x.Item3 == j));
                    int k = sub.Count();
                    c += k;
                    s = directions[type];
                }
            }
            if (c == 0) Console.Write(".");
            else if (c == 1) Console.Write(s);
            else Console.Write(c);
        }
        Console.WriteLine();
    }
    Console.WriteLine();
}