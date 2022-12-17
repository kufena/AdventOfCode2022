// See https://aka.ms/new-console-template for more information
using Day16;
using System.Diagnostics;
using System.IO.Compression;

Console.WriteLine("Hello, World!");
var lines = File.ReadAllLines(args[0]);
Dictionary<string, Node> valves = new Dictionary<string, Node>();
foreach (var line in lines)
{
    var splits = line.Split(new char[] { ' ', ',', '=', ';' }, StringSplitOptions.RemoveEmptyEntries);
    string name = splits[1];
    int flow = int.Parse(splits[5]);
    string[] vertices = splits[10..];
    Node n = new Node()
    {
        name = name,
        flow = flow,
        vertexnames = vertices
    };
    valves.Add(name, n);
}

Node AA;

foreach ((string name, Node v) in valves)
{
    if (name == "AA") AA = v;
    foreach (var n in v.vertexnames)
    {
        v.vertices.Add(valves[n]);
    }
}

Console.WriteLine($"We have {valves.Count} valves.");

List<string> nonZero = new();
foreach (var n in valves) if (n.Value.flow > 0) nonZero.Add(n.Value.name);

var rc = new RecursiveSolve(valves);
//var val = rc.Solve(valves["AA"], 30, new HashSet<string>());
var val = rc.SolveWithElephant(valves["AA"], 26, new HashSet<string>(), true);

Console.WriteLine($"value = {val}");

/*
// build routing tables.
Dictionary<string, Dictionary<string, int>> routingTables = new Dictionary<string, Dictionary<string, int>>();
foreach ((string name, Node v) in valves)
{
    routingTables[name] = new Dictionary<string, int>();
    foreach (var vert in v.vertices)
        routingTables[name].Add(vert.name, 1); // always 1 to a neighbour.
}

int shuffle = 0;
while (true)
{
    bool anyChange = false; // we stop if there's no change after a cycle.

    foreach ((string name, var table) in routingTables)
    {
        Node node = valves[name];
        foreach (var vertex in node.vertices)
        {
            if (!table.ContainsKey(vertex.name))
            {
                anyChange = true;
                table.Add(vertex.name, 1);
            }
            var vtable = routingTables[vertex.name];
            foreach (string destination in vtable.Keys)
            {
                if (!(destination == name)) // not us
                {
                    int step = vtable[destination] + 1;
                    if (!table.ContainsKey(destination)) // we don't know the destination, so add it + 1.
                    {
                        table.Add(destination, step);
                        anyChange = true;
                    }
                    else if (step < table[destination])
                    {
                        table[destination] = step;
                        anyChange = true;
                    }
                }
            }
        }
    }

    // see - we stop if there's no change.
    if (!anyChange)
    {
        shuffle += 1;
        if (shuffle > 3)
            break;
    }
}

// OK, let's route!

//Part1(valves, routingTables);
Part2(valves, routingTables);


//
Console.WriteLine("done?");
*/


//
// Find what's the best step, at clock time t, to release the most pressure.

/*
long FindStep(int clock, Node current, long pressure, List<string> released, out Node next, out int steps)
{
    next = null;
    long forseeable = 0;
    steps = 0;

    foreach ((string dest, int dist) in routingTables[current.name])
    {
        if (released.Contains(dest)) continue;
        var valve = valves[dest];
        long prob = ((clock - (dist + 1)) * valves[dest].flow);
        if (prob > forseeable)
        {
            next = valve;
            forseeable = prob;
            steps = dist;
        }
    } // at the end we should have the best destination to go to.

    return forseeable;
}

long FindTwoStep(int clock, Node current, long pressure, List<string> released, out Node next, out int steps)
{
    next = null;
    long forseeable = 0;
    steps = 0;
    long combined = 0;

    foreach ((string dest, int dist) in routingTables[current.name])
    {
        if (released.Contains(dest)) continue;
        var valve = valves[dest];
        long prob = ((clock - (dist + 1)) * valves[dest].flow);
        List<string> newrelease = new();
        foreach (var n in released) newrelease.Add(n);
        newrelease.Add(dest);
        Node next2 = null;
        int steps2 = 0;
        long step2 = FindStep(clock - (dist + 1), valve, pressure, newrelease, out next2, out steps2);

        if (prob + step2 > combined)
        {
            next = valve;
            forseeable = prob;
            steps = dist;
            combined = prob + step2;
        }
    } // at the end we should have the best destination to go to.

    return forseeable;
}

long FindThreeStep(int clock, Node current, long pressure, List<string> released, out Node next, out int steps)
{
    next = null;
    long forseeable = 0;
    steps = 0;
    long combined = 0;

    foreach ((string dest, int dist) in routingTables[current.name])
    {
        if (released.Contains(dest)) continue;

        List<string> newrelease = new();
        foreach (var n in released) newrelease.Add(n);
        newrelease.Add(dest);

        var valve = valves[dest];
        long prob = ((clock - (dist + 1)) * valves[dest].flow);
        int newclock = clock - (dist + 1);

        Node next2 = null;
        int steps2 = 0;
        foreach ((string dest2, int dist2) in routingTables[valve.name])
        {
            if (newrelease.Contains(dest2)) continue;

            List<string> newrelease2 = new();
            foreach (var n in newrelease) newrelease2.Add(n);
            newrelease2.Add(dest2);

            long prob2 = (newclock - (dist2 + 1)) * valves[dest2].flow;
            int newnewclock = newclock - (dist2 + 1);

            foreach ((string dest3, int dist3) in routingTables[valves[dest2].name])
            {
                if (newrelease2.Contains(dest3)) continue;

                long prob3 = (newnewclock - (dist3 + 1)) * valves[dest3].flow;

                if (prob + prob2 + prob3 > combined)
                {
                    forseeable = prob;
                    steps = dist;
                    combined = prob + prob2 + prob3;
                    next = valve;
                }
            }
        }
    } // at the end we should have the best destination to go to.

    return forseeable;
}

long FindFourStep(int clock, Node current, long pressure, List<string> released, out Node next, out int steps)
{
    next = null;
    long forseeable = 0;
    steps = 0;
    long combined = 0;

    foreach ((string dest, int dist) in routingTables[current.name])
    {
        if (released.Contains(dest)) continue;

        List<string> newrelease = new();
        foreach (var n in released) newrelease.Add(n);
        newrelease.Add(dest);

        var valve = valves[dest];
        long prob = ((clock - (dist + 1)) * valves[dest].flow);
        int newclock = clock - (dist + 1);

        Node next2 = null;
        int steps2 = 0;
        foreach ((string dest2, int dist2) in routingTables[valve.name])
        {
            if (newrelease.Contains(dest2)) continue;

            List<string> newrelease2 = new();
            foreach (var n in newrelease) newrelease2.Add(n);
            newrelease2.Add(dest2);

            long prob2 = (newclock - (dist2 + 1)) * valves[dest2].flow;
            int newnewclock = newclock - (dist2 + 1);

            foreach ((string dest3, int dist3) in routingTables[valves[dest2].name])
            {
                if (newrelease2.Contains(dest3)) continue;

                long prob3 = (newnewclock - (dist3 + 1)) * valves[dest3].flow;
                List<string> newrelease3 = new();
                foreach (var n in newrelease2) newrelease3.Add(n);
                newrelease3.Add(dest3);

                int newnewnewclock = newnewclock - (dist3 + 1);

                foreach ((string dest4, int dist4) in routingTables[valves[dest3].name])
                {
                    if (newrelease3.Contains(dest4)) continue;

                    long prob4 = (newnewnewclock - (dist4 + 1)) * valves[dest4].flow;

                    if (prob + prob2 + prob3 + prob4 > combined)
                    {
                        forseeable = prob;
                        steps = dist;
                        combined = prob + prob2 + prob3 + prob4;
                        next = valve;
                    }
                }
            }
        }
    } // at the end we should have the best destination to go to.

    return forseeable;
}

void Part1(Dictionary<string, Node> valves, Dictionary<string, Dictionary<string, int>> routingTables)
{
    int clock = 30; // we use one moving to AA?
    Node current = valves["AA"];
    long pressure = 0;
    List<string> released = new();

    while (clock > 0)
    {
        Console.WriteLine($"Release {current.name}");
        Node next;
        int steps;
        long prob = 0;
        released.Add(current.name);

        long step1 = FindFourStep(clock, current, pressure, released, out next, out steps);

        if (next == null)
            break;

        current = next;
        clock -= (steps + 1); // 
        pressure += step1;
    }

    Console.WriteLine($"Pressure max is {pressure}");
}

void Part2(Dictionary<string, Node> valves, Dictionary<string, Dictionary<string, int>> routingTables)
{
    int myclock = 26; 
    int elephantclock = 26;
    Node mycurrent = valves["AA"];
    long mypressure = 0;
    Node elephantcurrent = valves["AA"];
    long elephantpressure = 0;
    List<string> released = new() { "AA" };

    while (true) //clock > 0)
    {
        Node next;
        int steps;
        Node enext;
        int esteps;

        long step1 = FindFourStep(myclock, mycurrent, mypressure, released, out next, out steps);
        if (next == null) break;
        released.Add(next.name);
        long step2 = FindFourStep(elephantclock, elephantcurrent, elephantpressure, released, out enext, out esteps);


        if (next != null)
        {
            Console.WriteLine($"I'm doing {next.name} at {myclock - (steps+1)}");
            mycurrent = next;
            myclock -= (steps + 1); // 
            mypressure += step1;
        }

        if (enext != null)
        {
            Console.WriteLine($"The elephant is doing {enext.name} at {elephantclock - (esteps + 1)}");
            elephantclock -= (esteps + 1);
            elephantpressure += step2;
            elephantcurrent = enext;
            released.Add(enext.name);
        }

        if (enext == null || next == null)
            break;

    }

    Console.WriteLine($"Pressure max is {mypressure} + {elephantpressure} = {mypressure + elephantpressure}");
}
*/

class Node
{
    public string name;
    public int flow;
    public string[] vertexnames;
    public List<Node> vertices = new List<Node>();
    public bool closed = true;
    public bool open { get => !closed; }
}
