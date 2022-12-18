// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");
var lines = File.ReadAllLines(args[0]);

(int, int, int)[] points = new (int, int, int)[lines.Length];
for (int i = 0; i < lines.Length; i++)
{
    var splits = lines[i].Split(',', StringSplitOptions.RemoveEmptyEntries);
    points[i] = (int.Parse(splits[0]), int.Parse(splits[1]), int.Parse(splits[2]));
}

Console.WriteLine($"We have {points.Length} points.");

int sides = Part1(points);
Part2(points, sides);

static int Part1((int, int, int)[] points)
{
    int sides = points.Length * 6;
    for (int i = 0; i < points.Length; i++)
    {
        for (int j = i + 1; j < points.Length; j++)
        {
            (int ix, int iy, int iz) = points[i];
            (int jx, int jy, int jz) = points[j];

            int diffx = Math.Abs(ix - jx);
            int diffy = Math.Abs(iy - jy);
            int diffz = Math.Abs(iz - jz);

            if (diffx == 1 && diffy == 0 && diffz == 0)
            {
                sides -= 2;
            }
            if (diffx == 0 && diffy == 1 && diffz == 0)
            {
                sides -= 2;
            }
            if (diffx == 0 && diffy == 0 && diffz == 1)
            {
                sides -= 2;
            }

        }
    }

    Console.WriteLine($"We have {sides} showing.");
    return sides;
}

static void Part2((int, int, int)[] points, int sides)
{
    int maxX = 0;
    int maxY = 0;
    int maxZ = 0;

    foreach ((int x, int y, int z) in points)
    {
        if (x > maxX) maxX = x;
        if (y > maxY) maxY = y;
        if (z > maxZ) maxZ = z;
    }

    Console.WriteLine($"Extent of points is {maxX} in X, {maxY} in Y, {maxZ} in Z");
    maxX++;
    maxY++;
    maxZ++;

    int[][][] grid = new int[maxZ][][];
    for (int i = 0; i < maxZ; i++)
    {
        grid[i] = new int[maxY][];
        for (int j = 0; j < maxY; j++)
        {
            grid[i][j] = new int[maxX];
            for (int k = 0; k < maxX; k++)
            {
                grid[i][j][k] = 0;
            }
        }
    }

    // plot points
    foreach ((int x, int y, int z) in points)
    {
        grid[z][y][x] = 1;
    }

    int a = GoingEastWest(points, grid, maxZ, maxY, maxX);
    int b = GoingNorthSouth(points, grid, maxZ, maxY, maxX);
    int c = GoingUpDown(points, grid, maxZ, maxY, maxX);

    while (true)
    {
        bool changed = false;
        for (int i = 0; i < maxZ; i++)
        {
            for (int j = 0; j < maxY; j++)
            {
                for (int k = 0; k < maxX; k++)
                {
                    if (grid[i][j][k] == 0)
                    {
                        if (i - 1 >= 0 && grid[i - 1][j][k] == -1 ||
                            i + 1 < maxZ && grid[i + 1][j][k] == -1 ||
                            j - 1 >= 0 && grid[i][j - 1][k] == -1 ||
                            j + 1 < maxY && grid[i][j + 1][k] == -1 ||
                            k - 1 >= 0 && grid[i][j][k - 1] == -1 ||
                            k + 1 < maxX && grid[i][j][k + 1] == -1)
                        {
                            changed = true;
                            grid[i][j][k] = -1;
                        }
                    }
                }
            }
        }

        if (!changed)
            break;
    }

    for (int i = 0; i < maxZ; i++)
    {
        for (int j = 0; j < maxY; j++)
        {
            for (int k = 0; k < maxX; k++)
            {
                if (grid[i][j][k] == 0)
                {
                    Console.WriteLine($"Point ({k},{j},{i}) is unreachable.");
                    // need to count the sides attached to the square.
                    int subsides =
                        (i-1 >= 0 && grid[i - 1][j][k] == 1 ? 1 : 0) +
                        (i+1 < maxZ && grid[i + 1][j][k] == 1 ? 1 : 0) +
                        (j-1 >= 0 && grid[i][j - 1][k] == 1 ? 1 : 0) +
                        (j+1 < maxY && grid[i][j + 1][k] == 1 ? 1 : 0) +
                        (k-1 >= 0 && grid[i][j][k - 1] == 1 ? 1 : 0) +
                        (k+1 < maxX && grid[i][j][k + 1] == 1 ? 1 : 0);
                    sides -= subsides;
                }
            }
        }
    }

    Console.WriteLine($"We now have {sides} sides left.");
    Console.WriteLine($"But the total from the projection is {a + b + c}");
}

static int GoingEastWest((int,int,int)[] points, int[][][] grid, int maxz, int maxy, int maxx) 
{
    int res = 0;
    // X and Y axis - vary the Z.
    for (int j = 0; j < maxy; j++)
    {
        for (int k = 0; k < maxx; k++)
        {
            for (int i = 0; i < maxz; i++)
            {
                if (grid[i][j][k] == 1)
                {
                    res += 1;
                    break;
                }
                if (grid[i][j][k] == -1)
                    break; // we can see it from another direction.
                grid[i][j][k] = -1;
            }
        }
    }

    for (int j = 0; j < maxy; j++)
    {
        for (int k = 0; k < maxx; k++)
        {
            for (int i = maxz-1; i >= 0; i--)
            {
                if (grid[i][j][k] == 1)
                {
                    res += 1;
                    break;
                }
                if (grid[i][j][k] == -1)
                    break; // we can see it from another direction.
                grid[i][j][k] = -1;
            }
        }
    }
    return res;
}

static int GoingNorthSouth((int, int, int)[] points, int[][][] grid, int maxz, int maxy, int maxx)
{
    int res = 0;
    // X and Z axis - vary the Y.
    for (int i = 0; i < maxz; i++)
    {
        for (int k = 0; k < maxx; k++)
        {
            for (int j = 0; j < maxy; j++)
            {
                if (grid[i][j][k] == 1)
                {
                    res += 1;
                    break;
                }
                if (grid[i][j][k] == -1)
                    break; // we can see it from another direction.
                grid[i][j][k] = -1;
            }

            for (int j = maxy - 1; j >= 0; j--)
            {
                if (grid[i][j][k] == 1)
                {
                    res += 1;
                    break;
                }
                if (grid[i][j][k] == -1)
                    break; // we can see it from another direction.
                grid[i][j][k] = -1;
            }
        }
    }
    return res;
}

static int GoingUpDown((int, int, int)[] points, int[][][] grid, int maxz, int maxy, int maxx)
{
    int res = 0;
    // Z and Y axis - vary the X.
    for (int i = 0; i < maxz; i++)
    {
        for (int j = 0; j < maxy; j++)
        {
            for (int k = 0; k < maxx; k++)
            {
                if (grid[i][j][k] == 1)
                {
                    res += 1;
                    break;
                }
                if (grid[i][j][k] == -1)
                    break; // we can see it from another direction.
                grid[i][j][k] = -1;
            }

            for (int k = maxx - 1; k >= 0; k--)
            {
                if (grid[i][j][k] == 1)
                {
                    res += 1;
                    break;
                }
                if (grid[i][j][k] == -1)
                    break; // we can see it from another direction.
                grid[i][j][k] = -1;
            }
        }
    }
    return res;
}
