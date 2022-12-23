// See https://aka.ms/new-console-template for more information
using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;

Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines(args[0]);

//Part1Easy(lines);
Part2Hard(lines, int.Parse(args[1]));

int Facingval(char f)
{
    if (f == 'R') return 0;
    if (f == 'D') return 1;
    if (f == 'L') return 2;
    return 3; // for 'U'
}

char MoveByNinety(char facing, char direction)
{
    if (direction == 'R')
    {
        if (facing == 'R') return 'D';
        if (facing == 'D') return 'L';
        if (facing == 'L') return 'U';
        return 'R';
    }
    else
    {
        if (facing == 'R') return 'U';
        if (facing == 'U') return 'L';
        if (facing == 'L') return 'D';
        return 'R';
    }
}


void Part2Hard(string[] lines, int dim) {
    char[][] grid = new char[lines.Length][];
    Dictionary<int, (int, int)> horizontalExtents = new();
    Dictionary<int, (int, int)> verticalExtents = new();

    int linelength = 0;
    for (int i = 0; i < lines.Length - 2; i++)
    {
        grid[i] = lines[i].ToCharArray();
        if (lines[i].Length > linelength) linelength = lines[i].Length;
        int leftextent = lines[i].IndexOfAny(new char[] { '.', '#' });
        int rightextent = lines[i].LastIndexOfAny(new char[] { '.', '#' });
        horizontalExtents.Add(i, (leftextent, rightextent));
    }

    Console.WriteLine($"Grid is {grid.Length} lines long.");

    // work out vertical extents.

    for (int i = 0; i < linelength; i++)
    {
        int top = -1;
        int bottom = -1;
        for (int j = 0; j < lines.Length; j++)
        {
            (int left, int right) = horizontalExtents[j];
            if (left <= i && i <= right)
            {
                top = j;
                break;
            }
        }
        for (int j = lines.Length - 3; j >= 0; j--)
        {
            (int left, int right) = horizontalExtents[j];
            if (left <= i && i <= right)
            {
                bottom = j;
                break;
            }
        }
        verticalExtents.Add(i, (top, bottom));
    }

    // work out panels.
    var panels = WorkOutPanels(grid, horizontalExtents, verticalExtents, dim);
    var faceauxs = LoadFaceAuxillary(args[2]);
    int[][] facegrid = new int[lines.Length-2][];
    for (int i = 0; i < lines.Length-2; i++)
    {
        facegrid[i] = new int[linelength];
        for (int j = 0; j < linelength; j++)
        {
            facegrid[i][j] = -1;
        }
    }

    foreach ((int fcc, (int top, int left)) in panels)
    {
        for (int rows = top; rows < top + dim; rows++)
        {
            for (int cols = left; cols < left + dim; cols++)
            {
                facegrid[rows][cols] = fcc;
            }
        }
    }

    // carry on.
    (int, int) whereami = (0, horizontalExtents[0].Item1); // 0'th row, leftextent exent of that row - assume it's a dot.
    char facing = 'R';
    int face = facegrid[whereami.Item1][whereami.Item2];

    var instructions = lines[lines.Length - 1].ToCharArray();
    int pos = 0;
    while (true)
    {
        if (pos >= instructions.Length)
            break;

        int count = 0;
        while (pos < instructions.Length && Char.IsDigit(instructions[pos]))
        {
            count = (count * 10) + (instructions[pos] - '0');
            pos++;
        }
        char direction = pos >= instructions.Length ? 'S' : instructions[pos];
        pos++;

        (int leftextent, int rightextent) = horizontalExtents[whereami.Item1];
        (int topextent, int bottomextent) = verticalExtents[whereami.Item2];

        for (int j = 0; j < count; j++)
        {
            if (facing == 'R')
            {
                int newcol = whereami.Item2 + 1;
                int newrow = whereami.Item1;
                char newfacing = facing;
                int toface = facegrid[newrow][newcol]; ;
                if (newcol > rightextent)
                {
                    (int newface, char newside, char[] turns) = faceauxs[(face, facing)]; // this tells us what face we're going to but not where.
                    (int toprow, int topcol) = panels[face];
                    int rowdiff = whereami.Item1 - toprow;
                    int coldiff = whereami.Item2 - topcol;
                    (int toprownew, int topcolnew) = panels[newface];
                    if (newside == 'U') { newcol = (topcolnew + dim) - coldiff; newrow = toprownew; }
                    if (newside == 'L') { newcol = topcolnew + coldiff; newrow = toprownew; }
                    if (newside == 'R') { newcol = topcolnew; newrow = (topcolnew + dim) - rowdiff; }
                    if (newside == 'D') { newrow = toprownew + dim; newcol = topcolnew + rowdiff;  }
                    newfacing = ApplyTransforms(facing, turns);
                    toface = newface;
                }
                if (grid[newrow][newcol] == '#')
                    break; // go no further.
                whereami = (newrow, newcol);
                facing = newfacing;
                face = toface;
                (topextent, bottomextent) = verticalExtents[whereami.Item2];
                (leftextent, rightextent) = horizontalExtents[whereami.Item1];
            }
            if (facing == 'L')
            {
                int newcol = whereami.Item2 - 1;
                int newrow = whereami.Item1;
                char newfacing = facing;
                int toface = facegrid[newrow][newcol]; ;

                if (newcol < leftextent)
                {
                    (int newface, char newside, char[] turns) = faceauxs[(face, facing)]; // this tells us what face we're going to but not where.
                    (int toprow, int topcol) = panels[face];
                    int rowdiff = whereami.Item1 - toprow;
                    int coldiff = whereami.Item2 - topcol;
                    (int toprownew, int topcolnew) = panels[newface];
                    if (newside == 'U') { newcol = topcolnew + rowdiff; newrow = toprownew + dim; }
                    if (newside == 'L') { newcol = topcolnew + dim; newrow = toprownew + rowdiff; }
                    if (newside == 'R') { newcol = topcolnew + dim; newrow = toprownew + rowdiff; }
                    if (newside == 'D') { newrow = toprownew + dim; newcol = topcolnew + rowdiff; }
                    newfacing = ApplyTransforms(facing, turns);
                    toface = newface;
                }
                if (grid[newrow][newcol] == '#')
                    break; // go no further.
                whereami = (newrow, newcol);
                facing = newfacing;
                face = toface;
                (topextent, bottomextent) = verticalExtents[whereami.Item2];
                (leftextent, rightextent) = horizontalExtents[whereami.Item1];
            }
            if (facing == 'U')
            {
                int newcol = whereami.Item2;
                int newrow = whereami.Item1 - 1;
                char newfacing = facing;
                int toface = facegrid[newrow][newcol]; ;

                if (newrow < topextent)
                {
                    (int newface, char newside, char[] turns) = faceauxs[(face, facing)]; // this tells us what face we're going to but not where.
                    (int toprow, int topcol) = panels[face];
                    int rowdiff = whereami.Item1 - toprow;
                    int coldiff = whereami.Item2 - topcol;
                    (int toprownew, int topcolnew) = panels[newface];
                    if (newside == 'U') { newcol = (topcolnew + dim) - coldiff; newrow = toprownew; }
                    if (newside == 'L') { newcol = topcolnew; newrow = toprownew + coldiff; }
                    if (newside == 'R') { newcol = topcolnew + dim; newrow = (toprownew + dim) - coldiff; }
                    if (newside == 'D') { newrow = toprownew + dim; newcol = topcolnew + coldiff; }
                    newfacing = ApplyTransforms(facing, turns);
                    toface = newface;
                }
                if (grid[newrow][newcol] == '#')
                    break; // go no further.
                whereami = (newrow, newcol);
                facing = newfacing;
                face = toface;
                (topextent, bottomextent) = verticalExtents[whereami.Item2];
                (leftextent, rightextent) = horizontalExtents[whereami.Item1];
            }
            if (facing == 'D')
            {
                int newcol = whereami.Item2;
                int newrow = whereami.Item1 + 1;
                char newfacing = facing;
                int toface = facegrid[newrow][newcol]; ;

                if (newrow > bottomextent)
                {
                    (int newface, char newside, char[] turns) = faceauxs[(face, facing)]; // this tells us what face we're going to but not where.
                    (int toprow, int topcol) = panels[face];
                    int rowdiff = whereami.Item1 - toprow;
                    int coldiff = whereami.Item2 - topcol;
                    (int toprownew, int topcolnew) = panels[newface];
                    if (newside == 'U') { newcol = topcolnew + coldiff; newrow = toprownew; }
                    if (newside == 'L') { newcol = topcolnew; newrow = (toprownew + dim) - coldiff; }
                    if (newside == 'R') { newcol = topcolnew + dim; newrow = toprownew + coldiff; }
                    if (newside == 'D') { newrow = toprownew + dim; newcol = (topcolnew + dim) - coldiff; }
                    newfacing = ApplyTransforms(facing, turns);
                    toface = newface;
                }
                if (grid[newrow][newcol] == '#')
                    break; // go no further.
                whereami = (newrow, newcol);
                facing = newfacing;
                face = toface;
                (topextent, bottomextent) = verticalExtents[whereami.Item2];
                (leftextent, rightextent) = horizontalExtents[whereami.Item1]; 
            }
        }

        // now turn on the spot.
        if (direction == 'S')
            break;

        facing = MoveByNinety(facing, direction);

    }

    int finalrow = whereami.Item1 + 1;
    int finalcol = whereami.Item2 + 1;

    Console.WriteLine($"Our final position is {finalrow} row and {finalcol} column.");
    int facingval = Facingval(facing);
    Console.WriteLine($"Final facing val is {facingval} although we are on face {face}");
    Console.WriteLine($"Final score is {(finalrow * 1000) + (finalcol * 4) + facingval}");
}

char ApplyTransforms(char facing, char[] turns)
{
    char newfacing = facing;
    for (int i = 0; i < turns.Length; i++)
    {
        if (turns[i] == 'S') return facing;
        newfacing = MoveByNinety(newfacing, turns[i]);
    }
    return newfacing;
}

Dictionary<(int, char), (int, char, char[])> LoadFaceAuxillary(string v)
{
    var auxlines = File.ReadAllLines(v);
    Dictionary<(int, char), (int, char, char[])> faceauxs = new();
    for (int i = 0; i < auxlines.Length; i++)
    {
        var line = auxlines[i];
        var splits = line.Split(new char[] { ' ', '-', '>' }, StringSplitOptions.RemoveEmptyEntries);
        int facesource = int.Parse(splits[0]);
        int facetarget = int.Parse(splits[2]);
        char sidesource = splits[1].First();
        if (sidesource == 'T') sidesource = 'U';
        if (sidesource == 'B') sidesource = 'D';
        char sidetarget = splits[3].First();
        if (sidetarget == 'T') sidetarget = 'U';
        if (sidetarget == 'B') sidetarget = 'D';
        char[] transforms = new char[splits.Length - 4];
        for (int k = 4; k < splits.Length; k++)
        {
            transforms[k - 4] = splits[k].First();
        }
        faceauxs.Add((facesource, sidesource), (facetarget, sidetarget, transforms));
    }
    return faceauxs;
}

Dictionary<int, (int, int)> WorkOutPanels(char[][] grid, Dictionary<int, (int, int)> horizontalExtents, Dictionary<int, (int, int)> verticalExtents, int dim)
{
    Dictionary<int, (int, int)> faces = new();
    if (grid.Length / dim == 4)
    {
        // we are four panels wide and three high.
        int face = 1;
        for (int rows = 0; rows < (dim * 4); rows += dim)
        {
            (int x1, int x2)= horizontalExtents[rows];
            for (int cols = x1; cols < x2; cols += dim)
            {
                faces.Add(face, (rows, cols));
                face++;
            }
        }
    }
    else
    {
        // must be three panels wide and four high.
        // we are four panels wide and three high.
        int face = 1;
        for (int rows = 0; rows < (dim*3); rows += dim)
        {
            (int x1, int x2) = horizontalExtents[rows];
            for (int cols = x1; cols < x2; cols += dim)
            {
                faces.Add(face, (rows, cols));
                face++;
            }
        }
    }

    Debug.Assert(faces.Keys.Count == 6);
    foreach (var key in faces.Keys)
    {
        (int rows, int cols) = faces[key];
        Console.WriteLine($"Face {key} top left corner is {rows},{cols} to {rows + dim-1},{cols + dim-1} bottom right.");
    }
    return faces;

}

void Part1Easy(string[] lines)
{
    char[][] grid = new char[lines.Length][];
    Dictionary<int, (int, int)> horizontalExtents = new();
    Dictionary<int, (int, int)> verticalExtents = new();

    int linelength = 0;
    for (int i = 0; i < lines.Length - 2; i++)
    {
        grid[i] = lines[i].ToCharArray();
        if (lines[i].Length > linelength) linelength = lines[i].Length;
        int leftextent = lines[i].IndexOfAny(new char[] { '.', '#' });
        int rightextent = lines[i].LastIndexOfAny(new char[] { '.', '#' });
        horizontalExtents.Add(i, (leftextent, rightextent));
    }

    Console.WriteLine($"Grid is {grid.Length} lines long.");

    // work out vertical extents.

    for (int i = 0; i < linelength; i++)
    {
        int top = -1;
        int bottom = -1;
        for (int j = 0; j < lines.Length; j++)
        {
            (int left, int right) = horizontalExtents[j];
            if (left <= i && i <= right)
            {
                top = j;
                break;
            }
        }
        for (int j = lines.Length - 3; j >= 0; j--)
        {
            (int left, int right) = horizontalExtents[j];
            if (left <= i && i <= right)
            {
                bottom = j;
                break;
            }
        }
        verticalExtents.Add(i, (top, bottom));
    }

    // carry on.
    (int, int) whereami = (0, horizontalExtents[0].Item1); // 0'th row, leftextent exent of that row - assume it's a dot.
    char facing = 'R';

    var instructions = lines[lines.Length - 1].ToCharArray();
    int pos = 0;
    while (true)
    {
        if (pos >= instructions.Length)
            break;

        int count = 0;
        while (pos < instructions.Length && Char.IsDigit(instructions[pos]))
        {
            count = (count * 10) + (instructions[pos] - '0');
            pos++;
        }
        char direction = pos >= instructions.Length ? 'S' : instructions[pos];
        pos++;

        (int leftextent, int rightextent) = horizontalExtents[whereami.Item1];
        (int topextent, int bottomextent) = verticalExtents[whereami.Item2];

        for (int j = 0; j < count; j++)
        {
            if (facing == 'R')
            {
                int move = whereami.Item2 + 1;
                if (move > rightextent)
                {
                    move = leftextent;
                }
                if (grid[whereami.Item1][move] == '#')
                    break; // go no further.
                whereami = (whereami.Item1, move);
                (topextent, bottomextent) = verticalExtents[whereami.Item2];
            }
            if (facing == 'L')
            {
                int move = whereami.Item2 - 1;
                if (move < leftextent)
                {
                    move = rightextent;
                }
                if (grid[whereami.Item1][move] == '#')
                    break; // go no further.
                whereami = (whereami.Item1, move);
                (topextent, bottomextent) = verticalExtents[whereami.Item2];
            }
            if (facing == 'U')
            {
                int move = whereami.Item1 - 1;
                if (move < topextent)
                    move = bottomextent;
                if (grid[move][whereami.Item2] == '#')
                    break;
                whereami = (move, whereami.Item2);
                (leftextent, rightextent) = horizontalExtents[whereami.Item1];
            }
            if (facing == 'D')
            {
                int move = whereami.Item1 + 1;
                if (move > bottomextent)
                    move = topextent;
                if (grid[move][whereami.Item2] == '#')
                    break;
                whereami = (move, whereami.Item2);
                (leftextent, rightextent) = horizontalExtents[whereami.Item1];
            }
        }

        // now turn on the spot.
        if (direction == 'S')
            break;

        facing = MoveByNinety(facing, direction);

    }

    int finalrow = whereami.Item1 + 1;
    int finalcol = whereami.Item2 + 1;

    Console.WriteLine($"Our final position is {finalrow} row and {finalcol} column.");
    int facingval = Facingval(facing);
    Console.WriteLine($"Final score is {(finalrow * 1000) + (finalcol * 4) + facingval}");
}