// See https://aka.ms/new-console-template for more information
using Microsoft.VisualBasic.FileIO;

Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines(args[0]);

//Part1(lines);
Part2(lines);

void Part1(string[] lines)
{
    State st = new State();
    long total = 0;
    foreach (var l in lines)
    {
        if (l.StartsWith("noop")) {
            total += IncCycle(st);
        }
        if (l.StartsWith("addx")) {
            long c = long.Parse(l.Split(' ')[1]);
            total += IncCycle(st);
            total += IncCycle(st);
            st.registerX += c;
        }
    }
    Console.WriteLine($"Total is {total}");
}

void Part2(string[] lines)
{
    State st = new State(); 
    long total = 0;
    foreach (var l in lines)
    {
        if (l.StartsWith("noop"))
        {
            OutputDotOrDash(st);
            total += IncCycle(st);
        }
        if (l.StartsWith("addx"))
        {
            long c = long.Parse(l.Split(' ')[1]);
            OutputDotOrDash(st);
            total += IncCycle(st);
            OutputDotOrDash(st);
            total += IncCycle(st);
            st.registerX += c;
        }
    }
}
long IncCycle(State s)
{
    s.cycle += 1;
    if (s.cycle == 20 || s.cycle == 60 || s.cycle == 100 || s.cycle == 140 || s.cycle == 180 || s.cycle == 220) //% 20 == 0)
    {
        //Console.WriteLine($"Updating cycle {s.cycle} * {s.registerX} to get {s.cycle * s.registerX}");
        return s.cycle * s.registerX;
    }
    return 0;
}
void OutputDotOrDash(State st)
{
    long pos = st.cycle % 40;
    if (st.registerX >= (pos-1) && st.registerX <= (pos+1)) //st.cycle >= st.registerX - 1 && st.cycle <= st.registerX + 1)
    {
        Console.Write("#");
    }
    else
        Console.Write(".");
    if (st.cycle % 40 == 0)
        Console.WriteLine();
}

record State
{
    public long registerX = 1;
    public int cycle = 0;
}

