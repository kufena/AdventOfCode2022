// See https://aka.ms/new-console-template for more information
using System.Text;

Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines(args[0]);
//var res = Part1(lines);
var res = Part2(lines);
Console.WriteLine(res);

// Part 1 - moving crates on stacks.
string Part1(string[] lines)
{
    int stackLines = 0;
    foreach (var line in lines)
    {
        if (line.Contains("["))
            stackLines++;
        else
            break;
    }

    int numberLine = stackLines;
    int instructions = stackLines + 2;

    // need to parse the stacks.
    var nums = lines[numberLine].Split(' ', StringSplitOptions.RemoveEmptyEntries);
    int len = int.Parse(nums.Last());

    Stack<string>[] stacks = new Stack<string>[len];
    for (int i = 0; i < len; i++) stacks[i] = new Stack<string>();

    for (int i = stackLines - 1; i >= 0; i--) // work bottom up since stacks!
    {
        var stline = lines[i];
        for (int j = 0; j < len; j++)
        {
            if (stline.Substring(j * 4, 3).Trim().StartsWith('['))
            {
                string x = stline.Substring((j * 4) + 1, 1);
                Console.WriteLine($"Pushing {x} onto stack {j}");
                stacks[j].Push(x);
            }
        }
    }

    // we have stacks, now to follow the instructions.
    for (int i = instructions; i < lines.Length; i++)
    {
        var instr = lines[i];
        if (instr.StartsWith("move"))
        {
            var from = instr.IndexOf("from");
            var to = instr.IndexOf("to");
            var countstr = instr.Substring(4, from - 4);
            var fromstackstr = instr.Substring(from + 4, to - from - 4);
            var tostackstr = instr.Substring(to + 2);

            var count = int.Parse(countstr);
            var fromstack = int.Parse(fromstackstr) - 1;
            var tostack = int.Parse(tostackstr) - 1;

            for (int k = 0; k < count; k++)
            {
                var popper = stacks[fromstack].Pop();
                stacks[tostack].Push(popper);
            }

        }
    }

    // return
    var result = new StringBuilder();
    for (int i = 0; i < len; i++)
    {
        if (stacks[i].Count > 0)
        {
            var top = stacks[i].Pop();
            result.Append(top);
        }
    }
    return result.ToString();
}

// Part 2 - moving crates on stacks but order preserving.
string Part2(string[] lines)
{
    int stackLines = 0;
    foreach (var line in lines)
    {
        if (line.Contains("["))
            stackLines++;
        else
            break;
    }

    int numberLine = stackLines;
    int instructions = stackLines + 2;

    // need to parse the stacks.
    var nums = lines[numberLine].Split(' ', StringSplitOptions.RemoveEmptyEntries);
    int len = int.Parse(nums.Last());

    Stack<string>[] stacks = new Stack<string>[len];
    for (int i = 0; i < len; i++) stacks[i] = new Stack<string>();

    for (int i = stackLines - 1; i >= 0; i--) // work bottom up since stacks!
    {
        var stline = lines[i];
        for (int j = 0; j < len; j++)
        {
            if (stline.Substring(j * 4, 3).Trim().StartsWith('['))
            {
                string x = stline.Substring((j * 4) + 1, 1);
                Console.WriteLine($"Pushing {x} onto stack {j}");
                stacks[j].Push(x);
            }
        }
    }

    // we have stacks, now to follow the instructions.
    for (int i = instructions; i < lines.Length; i++)
    {
        var instr = lines[i];
        if (instr.StartsWith("move"))
        {
            var from = instr.IndexOf("from");
            var to = instr.IndexOf("to");
            var countstr = instr.Substring(4, from - 4);
            var fromstackstr = instr.Substring(from + 4, to - from - 4);
            var tostackstr = instr.Substring(to + 2);

            var count = int.Parse(countstr);
            var fromstack = int.Parse(fromstackstr) - 1;
            var tostack = int.Parse(tostackstr) - 1;

            var substack = new Stack<string>();
            for (int k = 0; k < count; k++)
            {
                var popper = stacks[fromstack].Pop();
                substack.Push(popper);
            }
            for (int k = 0; k < count; k++)
            {
                var popper = substack.Pop();
                stacks[tostack].Push(popper);
            }

        }
    }

    // return
    var result = new StringBuilder();
    for (int i = 0; i < len; i++)
    {
        if (stacks[i].Count > 0)
        {
            var top = stacks[i].Pop();
            result.Append(top);
        }
    }
    return result.ToString();
}
