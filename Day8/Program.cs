// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines(args[0]);
int horiz = lines[0].Length;
int vert = lines.Length;

int[][] input = new int[vert][];
for (int i = 0; i < vert; i++)
{
    input[i] = new int[horiz];
    var linearr = lines[i].ToCharArray();
    for (int j = 0; j < horiz; j++)
    {
        input[i][j] = (int)(linearr[j] - '0');
    }
}

//Part1(horiz, vert, input);
Part2(horiz, vert, input);
Console.WriteLine("done!");

void Part2(int horiz, int vert, int[][] input)
{
    var grid = CreateZeroGrid(horiz, vert);
    int max = 0;
    int maxi = -1;
    int maxj = -1;

    for (int i = 0; i < vert; i++)
    {
        for (int j = 0; j < horiz; j++)
        {
            grid[i][j] = CountVisibleTrees(i, j, input, vert, horiz);
            if (grid[i][j] > max)
            {
                max = grid[i][j];
                maxi = i;
                maxj = j;
            }
        }
    }
    Console.WriteLine($"At {maxi} {maxj} we have count {max}");
}

int[][] Part1Top(int[][] lines, int horiz, int vert)
{
    int[][] result = CreateGrid(vert, horiz);
    for (int i = 1; i < horiz - 1; i++)
    {
        int h = lines[0][i];
        for (int j = 1; j < vert - 1; j++)
        {
            if (lines[j][i] > h)
            {
                result[j][i] = 1;
                h = lines[j][i];
            }
        }
    }
    return result;
}

int[][] Part1Bottom(int[][] lines, int horiz, int vert)
{
    int[][] result = CreateGrid(vert, horiz);
    for (int i = 1; i < horiz - 1; i++)
    {
        int h = lines[vert - 1][i];
        for (int j = vert - 2; j > 0; j--)
        {
            if (lines[j][i] > h)
            {
                result[j][i] = 1;
                h = lines[j][i];
            }
        }
    }
    return result;
}

int[][] Part1Left(int[][] lines, int horiz, int vert)
{
    var result = CreateGrid(vert, horiz);
    for (int i = 1; i < vert - 1; i++)
    {
        int h = lines[i][0];
        for (int j = 1; j < horiz - 1; j++)
        {
            if (lines[i][j] > h)
            {
                result[i][j] = 1;
                h = lines[i][j];
            }
        }
    }
    return result;
}

int[][] Part1Right(int[][] lines, int horiz, int vert)
{
    var result = CreateGrid(vert, horiz);
    for (int i = 1; i < vert - 1; i++)
    {
        int h = lines[i][horiz - 1];
        for (int j = horiz - 2; j > 0; j--)
        {
            if (lines[i][j] > h)
            {
                result[i][j] = 1;
                h = lines[i][j];
            }
        }
    }
    return result;
}


int[][] CreateGrid(int horiz, int vert)
{
    int[][] result = new int[vert][];
    for (int i = 0; i < vert; i++)
    {
        result[i] = new int[horiz];
        result[i][0] = 1;
        result[i][horiz - 1] = 1;
    }
    for (int i = 0; i < horiz; i++)
    {
        result[0][i] = 1;
        result[vert - 1][i] = 1;
    }

    return result;
}

int[][] CreateZeroGrid(int horiz, int vert)
{
    int[][] result = new int[vert][];
    for (int i = 0; i < vert; i++)
    {
        result[i] = new int[horiz];
        for (int j = 0; j < horiz; j++)
            result[i][j] = 0;
    }

    return result;
}

void Part1(int horiz, int vert, int[][] input)
{
    int[][] top = Part1Top(input, horiz, vert);
    int[][] bottom = Part1Bottom(input, horiz, vert);
    var left = Part1Left(input, horiz, vert);
    var right = Part1Right(input, horiz, vert);

    int count = 0;
    var final = CreateGrid(horiz, vert);
    for (int i = 0; i < vert; i++)
    {
        for (int j = 0; j < horiz; j++)
        {
            if (top[i][j] == 1 || bottom[i][j] == 1 || left[i][j] == 1 || right[i][j] == 1)
            {
                count++;
                final[i][j] = 1;
            }
            else
            {
                final[i][j] = 0;
            }
        }
    }
    Console.WriteLine($"Part1 count is {count}");
}

int CountVisibleTrees(int i, int j, int[][] input, int vert, int horiz)
{
    int count = 1;
    count *= CountUp(i, j, input);
    count *= CountDown(i, j, vert, input);
    count *= CountLeft(i, j, input);
    count *= CountRight(i, j, horiz, input);
    return count;
}

int CountUp(int i, int j, int[][] input)
{
    // i is vert, j is horizontal
    if (i == 0) return 0;
    int count = 0;
    for (int y = i - 1; y >= 0; y--)
    {
        if (input[y][j] < input[i][j]) count++;
        if (input[y][j] >= input[i][j])
        {
            count++;
            break;
        }
    }
    return count;
}

int CountDown(int i, int j, int vert, int[][] input)
{
    // i is vert, j is horizontal
    if (i == vert-1) return 0;
    int count = 0;
    for (int y = i + 1; y < vert; y++)
    {
        if (input[y][j] < input[i][j]) count++;
        if (input[y][j] >= input[i][j])
        {
            count++;
            break;
        }
    }
    return count;
}

int CountLeft(int i, int j, int[][] input)
{
    if (j == 0) return 0;
    int count = 0;
    for (int x = j - 1; x >= 0; x--)
    {
        if (input[i][x] < input[i][j]) count++;
        if (input[i][x] >= input[i][j])
        {
            count++;
            break;
        }
    }
    return count;
}

int CountRight(int i, int j, int horiz, int[][] input)
{
    if (j == horiz-1) return 0;
    int count = 0;
    for (int x = j + 1; x < horiz; x++)
    {
        if (input[i][x] < input[i][j]) count++;
        if (input[i][x] >= input[i][j])
        {
            count++;
            break;
        }
    }
    return count;
}
