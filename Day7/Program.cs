using Day7;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines(args[0]);
Dir root = new Dir("/");

// we assume here that the first command will be 'cd /'
Parse(lines, 1, root);
root.setSize();

//long val = root.part1Total();
//Console.WriteLine($"Part 1 total is {val}");

long free = 70000000 - root.size;
long required = 30000000 - free;

Console.WriteLine($"Free space is {free} so need to free up {required}");

var candidates = root.findAtLeast(required);

// just finding the smallest candidate really.
long closeness = long.MaxValue;
Dir? dr = null;
foreach (var d in candidates)
{
    long t = d.size;
    if (t < closeness)
    {
        closeness = t;
        dr = d;
    }
}
Console.WriteLine($"Delete {dr.name} of size {dr.size} closeness {closeness}");
Console.WriteLine($"all done!");

int Parse(string[] lines, int index, Dir root)
{
    Dictionary<string, Systeam> entries = new Dictionary<string, Systeam>();
    while (index < lines.Length && lines[index].StartsWith("$"))
    {
        if (lines[index].ToLower().Equals("$ cd .."))
            return index + 1;

        else if (lines[index].StartsWith("$ cd"))
        {
            string n = lines[index].Substring(5); // name of dir
            if (entries.ContainsKey(n))
            {
                var s = entries[n];
                if (s is Dir)
                {
                    Dir ss = (Dir)s;
                    index = Parse(lines, index + 1, ss);
                }
            }
        }

        else if (lines[index].StartsWith("$ ls"))
        {
            index += 1;
            while (index < lines.Length && !(lines[index].StartsWith("$")))
            {
                if (lines[index].StartsWith("dir"))
                {
                    string na = lines[index].Substring(4);
                    var d = new Dir(na);
                    entries.Add(na, d);
                    root.entries.Add(d);
                }
                else
                {
                    long sz = 0;
                    string na;
                    var splits = lines[index].Split(' ');
                    na = splits[1].Trim();
                    sz = long.Parse(splits[0]);
                    var f = new Fiel(na, sz);
                    entries.Add(na, f);
                    root.entries.Add(f);
                }
                index += 1;
            }
        }
    }

    // we only get here if all lines consumed, I think.
    return index;
}