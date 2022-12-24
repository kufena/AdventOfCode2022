// See https://aka.ms/new-console-template for more information
using Day17;
using System.Diagnostics;

Console.WriteLine("Hello, World!");

long t1 = Stopwatch.GetTimestamp();

var lines = File.ReadAllLines(args[0]);
long count = long.Parse(args[1]);

string instructions = lines[0];

Console.WriteLine(instructions.Length);
Console.WriteLine(instructions);



DoneWithGridAndShortening(t1, ref count, instructions);

long height(long[][] board, long xh, long yh)
{
    for (long j = yh - 1; j >= 0; j--)
        for (long i = 0; i < xh; i++)
            if (board[i][j] == 1) return j;
    return 0;
}

long findheuristicheight(long[][] board, long xh, long yh, long current)
{
    long j = current + 1;
    for (; j < yh; j++)
    {
        bool allzeros = true;
        for (long i = 0; i < xh; i++)
        {
            if (board[i][j] == 1)
            {
                allzeros = false;
                break;
            }
        }
        if (allzeros)
            break;
    }
    return j - 1;
}

long shortenboard(long[][] board, long boardh)
{
    long[] heights = new long[7] { 0, 0, 0, 0, 0, 0, 0 };
    for (int j = 0; j < 7; j++)
    {
        for (long i = boardh - 1; i >= 0; i--)
        {
            if (board[j][i] == 1)
            {
                heights[j] = i;
                break;
            }
        }
    }

    long cutoff = heights.Min();
    if (cutoff == 0) return 0;

    for (int i = 0; i < 7; i++)
    {
        for (long j = cutoff; j < boardh; j++)
        {
            board[i][j - cutoff] = board[i][j];
        }
    }
    for (int i = 0; i < 7; i++)
    {
        for (long j = boardh - cutoff; j < boardh; j++)
        {
            board[i][j] = 0;
        }
    }

    return cutoff;
}

void FloorHeightsVersion()
{
    long rock = 0;
    long posx = 2;
    long posy = 3;
    int instructionsPos = 0;

    long boardh = 1000;
    long[][] board = new long[7][];

    for (long i = 0; i < 7; i++)
    {
        board[i] = new long[boardh];
        for (long j = 0; j < boardh; j++) board[i][j] = 0;
    }
    for (long i = 0; i < 7; i++) board[i][0] = 1;

    long hnow = height(board, 7, boardh);
    long[] floorheights = new long[7] { 0, 0, 0, 0, 0, 0, 0 };

    while (count > 0)
    {
        var shape = Shapes.ShapeList[rock];
        rock = (rock + 1) % Shapes.ShapeList.Length;
        posy = floorheights.Max() + 4;
        posx = 2;

        while (!Landed(shape, floorheights, posx, posy))
        {
            // Jet This Rock.
            if (instructions[instructionsPos] == '<')
            {
                if (posx - 1 >= 0 && !Covers(shape, floorheights, posx - 1, posy)) //  floorheights[posx-1] < (posy + shape.leftshifth))
                    posx -= 1;
                //if (posx < 0) posx = 0;
            }
            if (instructions[instructionsPos] == '>')
            {
                if (posx + 1 + (shape.width - 1) < 7 && //floorheights[posx + 1 + (shape.width - 1)] < (posy + shape.rightshifth))
                    !Covers(shape, floorheights, posx + 1, posy))
                    posx = posx + 1;
            }
            instructionsPos = (instructionsPos + 1) % instructions.Length;
            posy = posy - 1;

            //if (Landed(shape, floorheights, posx, posy)) break;
        }

        for (long j = posx; j < posx + shape.width; j++)
        {
            floorheights[j] = (posy + shape.height) - (shape.topProfile[j - posx]);
        }

        for (int j = 0; j < 7; j++)
            Console.Write($"|{floorheights[j]}|");
        Console.WriteLine();

        count -= 1;
    }

    Console.WriteLine($"Max height is {floorheights.Max()}");
}
bool Landed(Shapes shape, long[] floor, long posx, long posy)
{
    for (long i = posx; i < posx + shape.width; i++)
    {
        if ((posy + shape.botProfile[i - posx]) == floor[i])
            return true;
        //if ((posy + shape.botProfile[i - posx]) < floor[i])
        //    return true;

    }
    return false;
}

bool Covers(Shapes shape, long[] floor, long posx, long posy)
{
    for (long i = posx; i < posx + shape.width; i++)
    {
        if ((posy + shape.botProfile[i - posx]) <= floor[i])
            return true;
    }
    return false;
}

void DoneWithGridAndShortening(long t1, ref long count, string instructions)
{
    long rock = 0;
    long posx = 2;
    long posy = 3;
    int instructionsPos = 0;

    long boardh = 1000;
    long[][] board = new long[7][];

    long forshortening = 0;

    for (long i = 0; i < 7; i++)
    {
        board[i] = new long[boardh];
        for (long j = 0; j < boardh; j++) board[i][j] = 0;
    }
    for (long i = 0; i < 7; i++) board[i][0] = 1;

    long hnow = height(board, 7, boardh);
    
    while (count > 0)
    {
        hnow = findheuristicheight(board, 7, boardh, hnow);

        if (hnow + 100 > boardh)
        {
            long t = shortenboard(board, boardh);
            forshortening += t;
            //Console.WriteLine($"{count}");
            hnow = height(board, 7, boardh);
        }

        var shape = Shapes.ShapeList[rock];
        rock = (rock + 1) % Shapes.ShapeList.Length;
        posy = hnow + 4;
        posx = 2;

        while (true)
        {
            if (instructions[instructionsPos] == '<')
            {
                if (posx - 1 >= 0 && shape.IsClear(board, posx - 1, posy, 7, boardh)) //  floorheights[posx-1] < (posy + shape.leftshifth))
                    posx -= 1;
            }
            if (instructions[instructionsPos] == '>')
            {
                if (posx + 1 + (shape.width - 1) < 7 && shape.IsClear(board, posx + 1, posy, 7, boardh))
                    posx = posx + 1;
            }
            instructionsPos = (instructionsPos + 1) % instructions.Length;
            if (posy - 1 > 0 && shape.IsClear(board, posx, posy - 1, 7, boardh))
                posy = posy - 1;
            else
            {
                shape.Place(board, posx, posy, 7, boardh);
                break;
            }
        }

        count -= 1;
    }

    long h = height(board, 7, boardh);
    Console.WriteLine($"board height is {h}");
    Console.WriteLine($"With foreshortening, we get {h + forshortening}");
    Console.WriteLine($"Took time {Stopwatch.GetTimestamp() - t1}");
    /*
    for (long j = boardh - 1; j >= 0; j--)
    {
        bool show = false;
        for (long i = 0; i < 7; i++)
        {
            if (board[i][j] == 1)
            {
                show = true;
                break;
            }
        }
        if (show)
        {
            Console.Write("|");
            for (long i = 0; i < 7; i++)
            {
                if (board[i][j] == 1)
                    Console.Write("#");
                else
                    Console.Write(".");
            }
            Console.WriteLine("|");
        }
    }
    */
}