// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines(args[0]);
//long score = Part1(lines);
long score = Part2(lines);
Console.WriteLine($"Score is {score}");

long Part1(string[]? lines) {

    if (lines == null) return 0;

    long myScore = 0;
    foreach (var line in lines)
    {
        var splits = line.Split(' ');
        int elfPlay = ABCToInt(splits[0]);
        int myPlay = XYZToInt(splits[1]);

        if (elfPlay == -1 || myPlay == -1)
            throw new Exception($"non standard play found {splits[0]} or {splits[1]}");

        var calcScore = calculateScore(elfPlay, myPlay);
        myScore += (myPlay + calcScore);
        Console.WriteLine($"ElfPlay == {splits[0]} {elfPlay}  MyPlay == {splits[1]} {myPlay}  {calcScore} {myScore}");
    }
    return myScore;
}

long Part2(string[]? lines)
{

    if (lines == null) return 0;

    long myScore = 0;
    foreach (var line in lines)
    {
        var splits = line.Split(' ');
        int elfPlay = ABCToInt(splits[0]);
        int myPlay = XYZToPlay(splits[1], elfPlay);

        if (elfPlay == -1 || myPlay == -1)
            throw new Exception($"non standard play found {splits[0]} or {splits[1]}");

        var calcScore = calculateScore(elfPlay, myPlay);
        myScore += (myPlay + calcScore);
        Console.WriteLine($"ElfPlay == {splits[0]} {elfPlay}  MyPlay == {splits[1]} {myPlay}  {calcScore} {myScore}");
    }
    return myScore;
}

int ABCToInt(string x) {
    if (x == "A") return 1; // Rock
    if (x == "B") return 2; // Paper
    if (x == "C") return 3; // Scissors
    return -1;
}

int XYZToInt(string x)
{
    if (x == "X") return 1; // Rock
    if (x == "Y") return 2; // Paper
    if (x == "Z") return 3; // Scissors
    return -1;
}

long calculateScore(int elf, int me) {

    if (elf == me) // A draw
        return 3;

    if (elf == 2 && me == 1) return 0; // Paper beats Rock
    if (elf == 3 && me == 2) return 0; // Scissors beats Paper
    if (elf == 1 && me == 3) return 0; // Rock beats Scissors

    return 6; // No draw, no elf win, oh, we must have won.
}

int XYZToPlay(string me, int elf)
{
    if (me == "Y") return elf; // Y means a draw
    if (me == "X") return losingPlay(elf); // X means we have to lose
    return winningPlay(elf); // Otherwise, Z means we have to win.
}

int winningPlay(int play)
{
    if (play == 1) return 2; // They play rock, we play paper
    if (play == 2) return 3; // They play paper, we play scissors
    if (play == 3) return 1; // They play scissors, we play rock
    return -1;
}

int losingPlay(int play)
{
    if (play == 1) return 3; // all this is mod 3 arithmetic
    if (play == 2) return 1; // if only we'd converted it all
    if (play == 3) return 2; // to 0,1,2 instead of 1,2,3!
    return -1;
}
