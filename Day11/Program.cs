// See https://aka.ms/new-console-template for more information
using System.Linq.Expressions;

Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines(args[0]);
int iters = int.Parse(args[1]);

//Part1(lines, iters);
Part2(lines, iters);
Console.WriteLine("done!");

void Part1(string[] lines, int iters)
{
    // Get the monkeys.
    Dictionary<int, Monkey> monkeys = new();
    int lineC = 0;
    while (true)
    {
        Monkey m = ParseMonkey(lines, lineC);
        monkeys.Add(m.id, m);
        lineC += 7;
        if (lineC >= lines.Length) break;
    }

    // Do some iterations.
    for (int i = 0; i < iters; i++)
    {
        for (int j = 0; j < monkeys.Count; j++)
        {
            Monkey monkey = monkeys[j];
            List<long> newitems = new List<long>();
            foreach (var item in monkey.items)
            {
                long rem;
                long newworry = CalculateWorry(item, monkey);
                newworry = Math.DivRem(newworry, 3, out rem);

                if (newworry % monkey.divisor == 0)
                {
                    if (monkey.throwTrue == monkey.id)
                        newitems.Add(newworry);
                    else
                        monkeys[monkey.throwTrue].items.Add(newworry);
                }
                else
                {

                    if (monkey.throwFalse == monkey.id)
                        newitems.Add(newworry);
                    else
                        monkeys[monkey.throwFalse].items.Add(newworry);
                }
                monkey.count += 1;
            }
            monkey.items = newitems;
        }
    }

    for (int i = 0; i < monkeys.Count; i++) {
        Console.WriteLine($"Monkey business for {i} is {monkeys[i].count}");
    }
}

// No more dividing by three, so what to do to keep the numbers low?
void Part2(string[] lines, int iters)
{
    Console.WriteLine("Deffo doing part 2 here.");

    // Get the monkeys.
    Dictionary<int, Monkey> monkeys = new();
    int lineC = 0;
    long alldivisors = 1;
    while (true)
    {
        Monkey m = ParseMonkey(lines, lineC);
        monkeys.Add(m.id, m);
        alldivisors = alldivisors * m.divisor;
        lineC += 7;
        if (lineC >= lines.Length) break;
    }

    // Do some iterations.
    for (int i = 0; i < iters; i++)
    {
        for (int j = 0; j < monkeys.Count; j++)
        {
            Monkey monkey = monkeys[j];
            List<long> newitems = new List<long>();
            foreach (var item in monkey.items)
            {
                long rem;
                long newworry = CalculateWorry(item, monkey);
                long what = Math.DivRem(newworry, alldivisors, out rem);
                newworry = rem;
                long divided = Math.DivRem(newworry, monkey.divisor, out rem);

                if (rem == 0)
                {
                    if (monkey.throwTrue == monkey.id)
                        throw new Exception(""); // newitems.Add(rem);
                    else
                        monkeys[monkey.throwTrue].items.Add(newworry); // % monkeys[monkey.throwTrue].divisor);
                }
                else
                {

                    if (monkey.throwFalse == monkey.id)
                        throw new Exception(""); // newitems.Add(rem);
                    else
                        monkeys[monkey.throwFalse].items.Add(newworry); // % monkeys[monkey.throwFalse].divisor);
                }
                monkey.count += 1;
            }
            monkey.items = newitems;
        }
    }

    for (int i = 0; i < monkeys.Count; i++)
    {
        Console.WriteLine($"Monkey business for {i} is {monkeys[i].count}");
    }
}


long CalculateWorry(long item, Monkey monkey)
{
    long left = 0;
    long right = 0;
    if (monkey.left.Equals("old"))
        left = item;
    else
        left = long.Parse(monkey.left);

    if (monkey.right.Equals("old"))
        right = item;
    else
        right = long.Parse(monkey.right);

    if (monkey.op == "+")
        return left + right;
    if (monkey.op == "*")
        return left * right;
    throw new Exception($"Unkown op {monkey.op}");
}

Monkey ParseMonkey(string[] lines, int index)
{
    Monkey monkey = new Monkey();
    var splits = lines[index].Split(new char[] { ' ', ':' }, StringSplitOptions.RemoveEmptyEntries);
    monkey.id = int.Parse(splits[1]);
    index += 1;
    splits = lines[index].Split(new char[] { ' ', ':', ',' }, StringSplitOptions.RemoveEmptyEntries);
    for (int i = 2; i < splits.Count(); i++)
    {
        monkey.items.Add(long.Parse(splits[i]));
    }
    index += 1;
    splits = lines[index].Split(' ', StringSplitOptions.RemoveEmptyEntries);
    monkey.left = splits[3];
    monkey.op = splits[4];
    monkey.right = splits[5];
    index += 1;
    splits = lines[index].Split(' ', StringSplitOptions.RemoveEmptyEntries);
    monkey.divisor = int.Parse(splits[3]);
    index += 1;
    splits = lines[index].Split(' ', StringSplitOptions.RemoveEmptyEntries);
    monkey.throwTrue = int.Parse(splits[5]);
    index += 1;
    splits = lines[index].Split(' ', StringSplitOptions.RemoveEmptyEntries);
    monkey.throwFalse = int.Parse(splits[5]);

    return monkey;
}

// Monkey model.
record Monkey
{
    public int id= 0;
    public List<long> items = new List<long>();
    public string op = "";
    public string left = "";
    public string right = "";

    public int divisor = 0;
    public int throwTrue = 0;
    public int throwFalse = 0;

    public int count = 0; // for the result.
}