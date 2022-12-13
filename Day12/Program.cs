// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines(args[0]);

int Y = lines.Length;
int X = lines[0].Length;

// create grid.
List<(int, int)> starts;

Node end;

Node[][] grid;

ParseFiles(lines, Y, X, out starts, out end, out grid);

Console.WriteLine("we have our graph.!");

//Part1(grid, start, end);
Part2(lines, starts);

void Part2(string[] lines, List<(int, int)> starts)
{
    int shortest = int.MaxValue;
    foreach ((int y, int x) in starts)
    {
        Node[][] grid;
        Node end;
        List<(int, int)> news;
        ParseFiles(lines, Y, X, out news, out end, out grid);
        try
        {
            Part1(grid, grid[y][x], end);
            if (end.dist < shortest)
                shortest = end.dist;
        }
        catch
        {
            Console.WriteLine($"No path from ({y},{x})");
        }
    }
    Console.WriteLine($"Shortest is {shortest}");
}

void Part1(Node[][] grid, Node start, Node end)
{
    List<Node> unvisited = new List<Node>();

    for (int i = 0; i < X; i++)
        for (int j = 0; j < Y; j++)
            unvisited.Add(grid[j][i]);

    start.dist = 0;

    while (unvisited.Count > 0)
    {
        (List<Node> rest, Node n) = FindSmallest(unvisited);
        n.visited = true;
        foreach (var v in n.vertices)
        {
            if (!v.visited && v.dist > n.dist + 1)
                v.dist = n.dist + 1;
        }
        if (Object.ReferenceEquals(n, end))
            break;
        unvisited = rest;
    }

    Console.WriteLine($"{end.dist}");
}

// Oh what a hack hack hack!
(List<Node>, Node) FindSmallest(List<Node> nodes)
{
    Node smol = new Node();
    int sz = nodes.Count;
    foreach (var n in nodes)
    {
        if (n.dist < smol.dist)
            smol = n;
    }
    if (smol.dist == int.MaxValue)
        throw new Exception("dist not not oh yeah!");
    nodes.Remove(smol);
    return (nodes, smol);

    /*
    List<Node> rem = new List<Node>();
    foreach (var n in nodes)
    {
        if (!Object.ReferenceEquals(n, smol))
            rem.Add(n);
    }
    if (rem.Count == sz) // it isn't getting smaller.
        throw new Exception("No can do thank you very much!");

    return (rem, smol);
    */
}

static void ParseFiles(string[] lines, int Y, int X, out List<(int, int)> starts, out Node end, out Node[][] grid)
{
    starts = new List<(int, int)>();
    Node start = new Node();
    end = start;
    grid = new Node[Y][];
    for (int j = 0; j < Y; j++)
    {
        grid[j] = new Node[X];
        for (int i = 0; i < X; i++)
        {
            grid[j][i] = new Node();
            grid[j][i].id = lines[j][i];
            grid[j][i].val = lines[j][i];

            if (grid[j][i].id == 'S')
            {
                grid[j][i].val = 'a';
                start = grid[j][i];
                starts.Add((j, i));
            }
            if (grid[j][i].id == 'a')
                starts.Add((j, i));
            if (grid[j][i].id == 'E')
            {
                grid[j][i].val = 'z';
                end = grid[j][i];
            }
        }
    }


    for (int j = 0; j < Y; j++)
    {
        for (int i = 0; i < X; i++)
        {
            var node = grid[j][i];
            var nodeid = node.val;
            if (i - 1 >= 0 && grid[j][i - 1].val <= nodeid)
                node.vertices.Add(grid[j][i - 1]);
            if (j - 1 >= 0 && grid[j - 1][i].val <= nodeid)
                node.vertices.Add(grid[j - 1][i]);
            if (i + 1 < X && grid[j][i + 1].val <= nodeid)
                node.vertices.Add(grid[j][i + 1]);
            if (j + 1 < Y && grid[j + 1][i].val <= nodeid)
                node.vertices.Add(grid[j + 1][i]);

            var nodeidp1 = nodeid + 1;
            if (i - 1 >= 0 && grid[j][i - 1].val == nodeidp1)
                node.vertices.Add(grid[j][i - 1]);
            if (j - 1 >= 0 && grid[j - 1][i].val == nodeidp1)
                node.vertices.Add(grid[j - 1][i]);
            if (i + 1 < X && grid[j][i + 1].val == nodeidp1)
                node.vertices.Add(grid[j][i + 1]);
            if (j + 1 < Y && grid[j + 1][i].val == nodeidp1)
                node.vertices.Add(grid[j + 1][i]);
        }
    }
}

class Node
{
    public char id { get; set; }
    public char val { get; set; }
    public List<Node> vertices { get; set; } = new List<Node>();
    public int dist { get; set; } = int.MaxValue;
    public bool visited { get; set; } = false;
}
