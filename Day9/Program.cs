// See https://aka.ms/new-console-template for more information
using System.ComponentModel;

Console.WriteLine("Hello, World!");
var lines = File.ReadAllLines(args[0]);
//Part1(lines);
Part2(lines);

void Part1(string[] lines) 
{
    (int, int) hPos = (0, 0);
    (int, int) tPos = (0, 0);
    HashSet<(int,int)> tVisited = new();
    tVisited.Add(tPos);

    foreach (var s in lines)
    {
        var splits = s.Split(' ');
        string direction = splits[0];
        int count = int.Parse(splits[1]);

        for (int i = 0; i < count; i++)
        {
            var newH = moveH(hPos, direction);
            if (!adjacent(newH, tPos))
            { // move tail
                tPos = moveT(tPos, newH);
                tVisited.Add(tPos);
            }
            hPos = newH;
        }
    }
    foreach ((int tx, int ty) in tVisited) 
    {
        Console.WriteLine($"TPos is ({tx},{ty})");
    }
    Console.WriteLine($"Visited {tVisited.Count} positions.");
}

void Part2(string[] lines)
{
    (int, int)[] position = new (int, int)[10];
    for (int i = 0; i < 10; i++) position[i] = (0, 0);
    HashSet<(int,int)> tVisited = new();
    tVisited.Add((0, 0));

    foreach (var s in lines)
    {
        var splits = s.Split(' ');
        string direction = splits[0];
        int count = int.Parse(splits[1]);

        for (int i = 0; i < count; i++)
        {
            position[0] = moveH(position[0], direction);
            for (int j = 1; j < 10; j++) {
                if (!adjacent(position[j-1], position[j]))
                { // move tail
                    position[j] = moveT(position[j], position[j-1]);
                }
            }
            tVisited.Add(position[9]);
        }
    }
    Console.WriteLine($"Tail visited {tVisited.Count} positions.");
}

bool adjacent((int,int) a, (int,int) b)
{
    (int ax, int ay) = a;
    (int bx, int by) = b;
    int xdiff = Math.Abs(ax - bx);
    int ydiff = Math.Abs(ay - by);

    return (xdiff <= 1) && (ydiff <= 1);
}

bool squareOn((int, int) a, (int, int) b)
{
    (int ax, int ay) = a;
    (int bx, int by) = b;

    return (ax == bx) || (ay == by); // share an axis
}

(int,int) moveH((int,int) h, string c)
{
    (int hx, int hy) = h;
    switch (c)
    {
        case "U":
            return (hx, hy + 1);
        case "D":
            return (hx, hy - 1);
        case "L":
            return (hx - 1, hy);
        case "R":
            return (hx + 1, hy);
        default:
            throw new Exception($"Unknown move {c}");
    }
}

(int, int) moveT((int,int) t, (int,int) h)
{
    (int hx, int hy) = h;
    (int tx, int ty) = t;

    //
    // There's an assumption here that the difference between the two points
    // will not be more than two away.  That is, I only ever move the head by
    // one square in one of the directions up,down,left or right, and so the
    // tail will be at most two away.
    // This way, just using the sign of the difference gives us the move for
    // the tail, assuming sign returns 1 for +ive, -1 for -ve and 0 for 0.
    //
    return (tx + Math.Sign(hx - tx), ty + Math.Sign(hy - ty));
}


