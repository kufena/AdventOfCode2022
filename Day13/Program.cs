// See https://aka.ms/new-console-template for more information
using System.ComponentModel;

Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines(args[0]);

//Part1(lines);
Part2(lines);

(Thing, int) Parse(string line, int index)
{
    Thing result = new Thing();
    if (line[index] == '[')
    {
        result.number = false;
        result.list = new List<Thing>();

        index++;
        while (line[index] != ']')
        {
            (Thing t, int i) = Parse(line, index);
            result.list.Add(t);
            index = i;
        }
        if (index + 1 < line.Length && line[index + 1] == ',') index++;
        return (result, index + 1);
    }
    else
    {
        int v = 0;
        while (Char.IsDigit(line[index]))
        {
            v = (v * 10) + (line[index] - '0');
            index++;
        }
        if (index < line.Length && line[index] == ',') index++;
        result.number = true;
        result.num = v;
        return (result, index);
    }
}

int ThingCompare(Thing one, Thing two)
{
    if (one.number && two.number) // both numbers.
    {
        if (one.num == two.num) return 0;
        if (one.num < two.num) return 1;
        return -1;
    }
    if (!one.number && !two.number) // both lists.
    {
        if (one.list == null) throw new Exception("Err");
        if (two.list == null) throw new Exception("Rre");

        int ind = 0;
        int cmp = 0;
        while (ind < one.list.Count && ind < two.list.Count && cmp == 0)
        {
            cmp = ThingCompare(one.list[ind], two.list[ind]);
            ind += 1;
        }
        if (cmp != 0) return cmp; // we had a result, not just a botch.

        if (ind == 0) // one list is empty
        {
            if (two.list.Count == 0 && one.list.Count == 0) // both empty
                return 0;
            if (two.list.Count == 0)
                return -1;
            return 1;
        }

        if (one.list.Count < two.list.Count && ind >= one.list.Count)
        {
            if (cmp == 0) return 1;
            return cmp;
        }

        //if (ind <= one.list.Count && ind <= two.list.Count) // we got the result we wanted, so
        //{
        //    if (cmp == 0) return 1;
        //    return cmp;
        //}

        if (ind > one.list.Count && ind <= one.list.Count) // likewise, we ran out of left items, so
        {
            if (cmp == 0) return 1;
            return cmp;
        }

        if (one.list.Count == two.list.Count)
            return cmp;

        // can only mean right ran out of items, so...
        return -1;
    }
    if (one.number && !two.number)
    {
        var newone = new Thing()
        {
            number = false,
            list = new List<Thing>() { one }
        };
        return ThingCompare(newone, two);
    }
    if (!one.number && two.number)
    {
        var newtwo = new Thing()
        {
            number = false,
            list = new List<Thing>() { two }
        };
        return ThingCompare(one, newtwo);
    }

    throw new Exception("Shouldnt get here");
}

void Part1(string[] lines)
{
    int index = 0;
    int count = 0;
    int item = 1;
    while (index < lines.Length)
    {
        string one = lines[index];
        string two = lines[index + 1];

        index += 3;

        (Thing t1, int i) = Parse(one, 0);
        (Thing t2, int j) = Parse(two, 0);

        //Console.WriteLine($"A = {one}");
        //Console.Write("P = ");  t1.Print(); Console.WriteLine();
        //Console.WriteLine($"A = {two}");
        //Console.Write("P = ");  t2.Print(); Console.WriteLine();

        int cmp = ThingCompare(t1, t2);
        if (cmp > 0) count += item;
        Console.WriteLine($"Outcome of item {item} compare is {cmp}");
        item += 1;
    }
    Console.WriteLine($"Found {count} successes");
}

void Part2(string[] lines)
{
    
    SortedList<Thing, Thing> allthings = new SortedList<Thing, Thing>(new Comparer());
    Thing decode1 = new Thing()
    {
        number = false,
        list = new List<Thing>()
        {
            new Thing() {
                number = false,
                list = new List<Thing>() {
                    new Thing() {
                        number = true,
                        num = 2
                    }
                }
            }
        }
    };

    Thing decode2 = new Thing()
    {
        number = false,
        list = new List<Thing>()
        {
            new Thing() {
                number = false,
                list = new List<Thing>() {
                    new Thing() {
                        number = true,
                        num = 6
                    }
                }
            }
        }
    };

    allthings.Add(decode1, decode1);
    allthings.Add(decode2, decode2);
    foreach (string l in lines)
    {
        if (!l.Trim().Equals(""))
        {
            (Thing t, int i) = Parse(l, 0);
            allthings.Add(t,t);
        }
    }

    int index = 1;
    foreach ((Thing a, Thing b) in allthings)
    {
        Console.Write($"Index {index}  "); a.Print(); Console.WriteLine();
        index += 1;
    }


}

class Comparer : IComparer<Thing>
{
    public int Compare(Thing? x, Thing? y)
    {
        if (x == null || y == null)
            throw new Exception("null thing.");
        return MyThingCompare(y, x); // x, y);
    }

    internal int MyThingCompare(Thing one, Thing two)
    {
        if (one.number && two.number) // both numbers.
        {
            if (one.num == two.num) return 0;
            if (one.num < two.num) return 1;
            return -1;
        }
        if (!one.number && !two.number) // both lists.
        {
            if (one.list == null) throw new Exception("Err");
            if (two.list == null) throw new Exception("Rre");

            int ind = 0;
            int cmp = 0;
            while (ind < one.list.Count && ind < two.list.Count && cmp == 0)
            {
                cmp = MyThingCompare(one.list[ind], two.list[ind]);
                ind += 1;
            }
            if (cmp != 0) return cmp; // we had a result, not just a botch.

            if (ind == 0) // one list is empty
            {
                if (two.list.Count == 0 && one.list.Count == 0) // both empty
                    return 0;
                if (two.list.Count == 0)
                    return -1;
                return 1;
            }

            if (one.list.Count < two.list.Count && ind >= one.list.Count)
            {
                if (cmp == 0) return 1;
                return cmp;
            }

            //if (ind <= one.list.Count && ind <= two.list.Count) // we got the result we wanted, so
            //{
            //    if (cmp == 0) return 1;
            //    return cmp;
            //}

            if (ind > one.list.Count && ind <= one.list.Count) // likewise, we ran out of left items, so
            {
                if (cmp == 0) return 1;
                return cmp;
            }

            if (one.list.Count == two.list.Count)
                return cmp;

            // can only mean right ran out of items, so...
            return -1;
        }
        if (one.number && !two.number)
        {
            var newone = new Thing()
            {
                number = false,
                list = new List<Thing>() { one }
            };
            return MyThingCompare(newone, two);
        }
        if (!one.number && two.number)
        {
            var newtwo = new Thing()
            {
                number = false,
                list = new List<Thing>() { two }
            };
            return MyThingCompare(one, newtwo);
        }

        throw new Exception("Shouldnt get here");
    }

}

class Thing
{
    public bool number { get; set; } = true;
    public int num { get; set; }
    public List<Thing>? list { get; set; }
    public void Print()
    {
        if (number) Console.Write($"{num}");
        else
        {
            Console.Write("[");
            if (list == null)
            {
                Console.Write("err]");
                return;
            }
            if (list.Count > 0)
            {
                for (int i = 0; i < (list.Count - 1); i++)
                {
                    list[i].Print();
                    Console.Write(",");
                }
                list.Last().Print();
            }
            Console.Write("]");
        }
    }
}