// See https://aka.ms/new-console-template for more information
using Microsoft.VisualBasic;

Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines(args[0]);

// No physical grid, as an array, so set the extents as big as we need.
int xl = 0; // 500;
int xr = 1000; // 500;
int deep = 0;

foreach (var line in lines)
{
    var arr = ParseLine(line);
    for (int i = 0; i < arr.Length; i++)
    {
        (int x, int y) = arr[i];
        if (y > deep) deep = y;
    }
}

HashSet<(int,int)> grid = new HashSet<(int,int)> ();

Console.WriteLine($"Grid is {xl} to the left, {xr} to the right, and {deep} deep.");

Part1(lines);
//Part2(lines);

void Part1(string[] lines)
{
    // map rock.
    for (int i = 0; i < lines.Length; i++)
    {
        var points = ParseLine(lines[i]);
        for (int j = 1; j < points.Length; j++)
        {
            MapPoints(points[j - 1], points[j]);
        }
    }

    // flow sand.
    int count = 0;
    while (MapNewSand())
    {
        count++;
    }

    Console.WriteLine($"We managed {count} sand grains.");
}


void Part2(string[] lines)
{
    // map rock.
    for (int i = 0; i < lines.Length; i++)
    {
        var points = ParseLine(lines[i]);
        for (int j = 1; j < points.Length; j++)
        {
            MapPoints(points[j - 1], points[j]);
        }
    }

    // For part 2, we add two extra layers, and make it rock hard.
    // I think the constraint here is if deep > 500 - then the triangle formed by
    // falling sand will be more than 500 wide, and we'll get negative x values.
    deep += 2;
    for (int i = 0; i < 1001; i++)
    {
        grid.Add((deep, i));
    }

    // flow sand.
    int count = 0;
    while (MapNewSand())
    {
        count++;

        if (grid.Contains((0, 500))) // [0][500] == 2) // snow at the point.
            break;

    }

    Console.WriteLine($"We managed {count} sand grains.");
}

// A part1 sand mapping function which returns a bool.
//   - True if it lands a bit of sand on solid ground
//   - False if sand falls below the deepest part of the grid.
bool MapNewSand()
{
    (int x, int y) = (500 - xl, 0);
    while (y < deep + 1)
    {
        // next move is x,y+1
        int ny = y + 1;

        if (ny >= deep + 1)
            break;

        // if we can go straight down, then fine.
        if (!grid.Contains((ny, x)))  // [ny][x] == 0)
        {
            y = ny;
            continue;
        }
        // can't go straight down - check to the left.
        x = x - 1;
        if (x < 0)
            break;
        if (!grid.Contains((ny,x))) //[ny][x] == 0)
        {
            y = ny;
            continue;
        }
        // can't go left - go right?
        x = x + 2;
        //if (x >= grid[0].Length)
        //    break; // I don't think these will ever happen?
        if (!grid.Contains((ny,x))) //[ny][x] == 0)
        {
            y = ny;
            continue;
        }
        // Ah, we can't move.  So settle here?
        grid.Add((y, x - 1));   //[y][x - 1] = 2;
        return true;
    }

    return false; // we've fallen out the bottom.
}

// Add rock bits to the grid.
void MapPoints((int, int) value1, (int, int) value2)
{
    (int x1, int y1) = value1;
    (int x2, int y2) = value2;

    if (x1 == x2) // we're going vert
    {
        if (y1 > y2)
        {
            for (int i = y2; i <= y1; i++)
            {
                grid.Add((i, (x1 - xl)));
            }
        }
        else
        {
            for (int i = y1; i <= y2; i++)
            {
                grid.Add((i, (x1 - xl)));
            }
        }
    }
    else // must be horiz
    {
        if (x1 < x2)
        {
            for (int i = x1 - xl; i <= x2 - xl; i++)
            {
                grid.Add((y1,i));
            }
        }
        else
        {
            for (int i = x2 - xl; i <= x1 - xl; i++)
            {
                grid.Add((y1, i));
            }
        }
    }
}

(int, int)[] ParseLine(string s)
{
    var splits = s.Split(new char[] { ' ', '-', '>' }, StringSplitOptions.RemoveEmptyEntries);
    (int, int)[] res = new (int, int)[splits.Length];
    for (int i = 0; i < splits.Length; i++)
    {
        res[i] = ParsePair(splits[i]);
    }
    return res;
}

(int, int) ParsePair(string s)
{
    var splits = s.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
    int x = int.Parse(splits[0]);
    int y = int.Parse(splits[1]);
    return (x, y);
}