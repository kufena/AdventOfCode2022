// See https://aka.ms/new-console-template for more information
using System.Security.Cryptography.X509Certificates;

Console.WriteLine("Hello, World!");
var lines = File.ReadAllLines(args[0]);

int row = 0;
int column = 0;

int originrow = 500;
int origincol = 500;
Dictionary<int, (int, int)> elves = new();
HashSet<(int, int)> occupied = new();

int elfcount = 0;
for (int i = 0; i < lines.Length; i++)
{
    for (int j = 0; j < lines[i].Length; j++)
    {
        if (lines[i][j] == '#')
        {
            elfcount++;
            elves.Add(elfcount, (originrow + i, origincol + j));
            occupied.Add((originrow + i, origincol + j));
        }
    }
}

var northmove = ((int rol, int col, HashSet<(int, int)> occupied) =>
                        (!(occupied.Contains((rol - 1, col - 1))) && !(occupied.Contains((rol - 1, col))) && !(occupied.Contains((rol - 1, col + 1)))) ? (rol - 1, col) : (int.MinValue, int.MinValue));
var southmove = ((int rol, int col, HashSet<(int, int)> occupied) =>
                        (!(occupied.Contains((rol + 1, col - 1))) && !(occupied.Contains((rol + 1, col))) && !(occupied.Contains((rol + 1, col + 1)))) ? (rol + 1, col) : (int.MinValue, int.MinValue));
var westmove = ((int rol, int col, HashSet<(int, int)> occupied) =>
                        (!(occupied.Contains((rol, col - 1))) && !(occupied.Contains((rol + 1, col - 1))) && !(occupied.Contains((rol - 1, col - 1)))) ? (rol, col - 1) : (int.MinValue, int.MinValue));
var eastmove = ((int rol, int col, HashSet<(int, int)> occupied) =>
                        (!(occupied.Contains((rol, col + 1))) && !(occupied.Contains((rol - 1, col + 1))) && !(occupied.Contains((rol + 1, col + 1)))) ? (rol, col + 1) : (int.MinValue, int.MinValue));

Func<int, int, HashSet<(int, int)>, (int, int)>[] Moves = new Func<int, int, HashSet<(int, int)>, (int, int)>[] { northmove, southmove, westmove, eastmove };

//Part1(elves, occupied, Moves);
Part2(elves, occupied, Moves);

bool EmptySpaceAllAround(int x, int y, HashSet<(int, int)> occupied)
{
    return !occupied.Contains((x - 1, y)) &&
        !occupied.Contains((x + 1, y)) &&
        !occupied.Contains((x, y - 1)) &&
        !occupied.Contains((x, y + 1)) &&
        !occupied.Contains((x + 1, y + 1)) &&
        !occupied.Contains((x + 1, y - 1)) &&
        !occupied.Contains((x - 1, y + 1)) &&
        !occupied.Contains((x - 1, y - 1));
}

static int PrintAndCountSpaces(HashSet<(int, int)> occupied)
{
    int top = int.MinValue;
    int bottom = int.MaxValue;
    int left = int.MaxValue;
    int right = int.MinValue;

    foreach ((int r, int c) in occupied)
    {
        //Console.WriteLine($"{r} {c} is occupied.");
        if (r > top) top = r;
        if (r < bottom) bottom = r;
        if (left > c) left = c;
        if (right < c) right = c;
    }

    Console.WriteLine($"{top} to {bottom} and {left} to {right}");
    int countSpaces = 0;
    for (int rowa = bottom; rowa <= top; rowa++)
    {
        for (int cola = left; cola <= right; cola++)
        {
            if (!occupied.Contains((rowa, cola)))
            {
                countSpaces++;
                Console.Write(".");
            }
            else
                Console.Write("#");
        }
        Console.WriteLine();
    }

    return countSpaces;
}

void Part1(Dictionary<int, (int, int)> elves, HashSet<(int, int)> occupied, Func<int, int, HashSet<(int, int)>, (int, int)>[] Moves)
{
    int currentFunc = 0;

    PrintAndCountSpaces(occupied);
    Console.ReadLine();
    Console.WriteLine("Go....");

    for (int round = 0; round < 10; round++)
    {
        // Produce potential moves.
        Dictionary<(int, int), List<int>> coordsToEleves = new();
        Dictionary<int, (int, int)> elfToProposed = new();
        foreach (var elf in elves.Keys)
        {
            if (elf == 17)
                Console.WriteLine("num 17");

            (int elfrow, int elfcol) = elves[elf];
            if (!EmptySpaceAllAround(elfrow, elfcol, occupied))
            {
                // we'll propose a move if we can, since there are other elves nearby.
                for (int mo = 0; mo < 4; mo++)
                {
                    (int proposedrow, int proposedcol) = Moves[(mo + currentFunc) % 4](elfrow, elfcol, occupied);
                    if (proposedrow > int.MinValue)
                    {
                        elfToProposed.Add(elf, (proposedrow, proposedcol));
                        if (coordsToEleves.ContainsKey((proposedrow, proposedcol)))
                        {
                            coordsToEleves[(proposedrow, proposedcol)].Add(elf);
                        }
                        else
                        {
                            coordsToEleves.Add((proposedrow, proposedcol), new List<int>() { elf });
                        }

                        break;
                    }
                }
            }
        }

        // remember to cycle the functions.
        currentFunc++;

        // Do actual moves.
        foreach (var elf in elves.Keys)
        {
            (int currentrow, int currentcol) = elves[elf];
            if (elfToProposed.ContainsKey(elf)) // we have a proposal.
            {
                (int proposedrow, int proposedcol) = elfToProposed[elf];
                if (coordsToEleves[(proposedrow, proposedcol)].Count == 1) // it's a unique move, do it!
                {
                    occupied.Remove((currentrow, currentcol));
                    occupied.Add((proposedrow, proposedcol));
                    elves[elf] = (proposedrow, proposedcol);
                }
            }
        }

        Console.WriteLine($"Done Round {round}");
        PrintAndCountSpaces(occupied);
        Console.ReadLine();
    }

    Console.WriteLine();
    Console.WriteLine();
    Console.WriteLine();
    Console.WriteLine("Finale...");

    int countSpaces = PrintAndCountSpaces(occupied);

    Console.WriteLine($"woo - {countSpaces} spaces I think.");
}

void Part2(Dictionary<int, (int, int)> elves, HashSet<(int, int)> occupied, Func<int, int, HashSet<(int, int)>, (int, int)>[] Moves)
{
    int currentFunc = 0;

    //PrintAndCountSpaces(occupied);
    Console.ReadLine();
    Console.WriteLine("Go....");
    int round = 1;
    while(true)
    {
        // Produce potential moves.
        Dictionary<(int, int), List<int>> coordsToEleves = new();
        Dictionary<int, (int, int)> elfToProposed = new();
        foreach (var elf in elves.Keys)
        {
            if (elf == 17)
                Console.WriteLine("num 17");

            (int elfrow, int elfcol) = elves[elf];
            if (!EmptySpaceAllAround(elfrow, elfcol, occupied))
            {
                // we'll propose a move if we can, since there are other elves nearby.
                for (int mo = 0; mo < 4; mo++)
                {
                    (int proposedrow, int proposedcol) = Moves[(mo + currentFunc) % 4](elfrow, elfcol, occupied);
                    if (proposedrow > int.MinValue)
                    {
                        elfToProposed.Add(elf, (proposedrow, proposedcol));
                        if (coordsToEleves.ContainsKey((proposedrow, proposedcol)))
                        {
                            coordsToEleves[(proposedrow, proposedcol)].Add(elf);
                        }
                        else
                        {
                            coordsToEleves.Add((proposedrow, proposedcol), new List<int>() { elf });
                        }

                        break;
                    }
                }
            }
        }

        // remember to cycle the functions.
        currentFunc++;

        // Do actual moves.
        int moves = 0;
        foreach (var elf in elves.Keys)
        {
            (int currentrow, int currentcol) = elves[elf];
            if (elfToProposed.ContainsKey(elf)) // we have a proposal.
            {
                (int proposedrow, int proposedcol) = elfToProposed[elf];
                if (coordsToEleves[(proposedrow, proposedcol)].Count == 1) // it's a unique move, do it!
                {
                    occupied.Remove((currentrow, currentcol));
                    occupied.Add((proposedrow, proposedcol));
                    elves[elf] = (proposedrow, proposedcol);
                    moves++;
                }
            }
        }

        Console.WriteLine($"Done Round {round} with {moves} moves.");

        if (moves == 0) break;
        round++;

        //PrintAndCountSpaces(occupied);
        //Console.ReadLine();
    }

    Console.WriteLine();
    Console.WriteLine();
    Console.WriteLine();
    Console.WriteLine("Finale...");

    int countSpaces = PrintAndCountSpaces(occupied);

    Console.WriteLine($"woo - {countSpaces} spaces I think.");
}
