// See https://aka.ms/new-console-template for more information
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Net.Http.Headers;
using System.Runtime.Serialization;

Console.WriteLine("Hello, World!");
var lines = File.ReadAllLines(args[0]);
var count = int.Parse(args[1]);

Dictionary<int, int[]>[] blueprints = new Dictionary<int, int[]>[lines.Length];
//Dictionary<(string, int), int> cache = new();
Dictionary<((long,long), int), int> cache = new();

int c = 0;
foreach (var line in lines)
{
    var splits = line.Split(new char[] { ':', '.' }, StringSplitOptions.RemoveEmptyEntries);
    int id = int.Parse(splits[0].Split(' ', StringSplitOptions.RemoveEmptyEntries)[1]);
    Dictionary<int,int[]> robotcosts = new Dictionary<int,int[]>();
    blueprints[c] = robotcosts;
    c++;

    for (int i = 1; i < splits.Length; i++)
    {
        var rulesplits = splits[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);
        int robot = MapToInt(rulesplits[1]);
        int[] costs = new int[4];
        for (int j = 4; j < rulesplits.Length; j+=2)
        {
            if (rulesplits[j] == "and") j += 1; // skip the 'and'
            int num = int.Parse(rulesplits[j]);
            int what = MapToInt(rulesplits[j + 1]);
            costs[what] = num;
        }
        robotcosts.Add(robot, costs);
    }
}

Console.WriteLine($"We have some rules.");

long qualityTotal = 0;

//foreach (var blueprint in blueprints)
for(int k = 0; k < 3; k++) // blueprints.Length; k++)
{
    var costs = blueprints[k];

    int[] initRobotsOwned = new int[] { 1, 0, 0, 0 };
    int[] initMined = new int[] { 0, 0, 0, 0 };

    int[] maxrobots = new int[] { 0, 0, 0, 0 };
    foreach ((int rb, int[] spends) in costs)
    {
        for (int i = 0; i < 4; i++)
        {
            if (spends[i] > maxrobots[i]) maxrobots[i] = spends[i];
        }
    }
    maxrobots[3] = int.MaxValue; // we can have as many geode robots as we want.
    //cache = new Dictionary<(string, int), int>();
    cache = new Dictionary<((long,long), int), int>();

    int numgeodes = CalculateGeodes(count, costs, maxrobots, new State() { mined = new int[] { 0, 0, 0, 0 }, robotsOwned = new int[] { 1, 0, 0, 0 } }, 0);

    Console.WriteLine($"Blueprint {k+1} complete!  Geodes mined is {numgeodes}");

    qualityTotal += (numgeodes * (k+1));
}

Console.WriteLine($"Total quality is {qualityTotal}");


static int MapToInt(string s) {

    if (s == "ore") return 0;
    if (s == "clay") return 1;
    if (s == "obsidian") return 2;
    if (s == "geode") return 3;
    return -1;
}

int CalculateGeodes(int count, Dictionary<int, int[]> costs, int[] maxrobots, State state, int clock)
{
    int maxGeodes = 0;
    //string stateKey = StateStr(state);
    var stateKey = StateLongs(state);

    if (clock == count)
    {
        //cache.Add((state, clock), state.mined[3]);
        return state.mined[3];
    }

    if (cache.ContainsKey((stateKey, clock))) return cache[(stateKey, clock)];

    var robotsOwned = state.robotsOwned;
    var mined = state.mined;

    // Purchases first.
    HashSet<int> potentialPurchases = new HashSet<int>();
    while (true)
    {
        int purchaseType = -1;
        for (int rt = 3; rt >= 0; rt--) // get list of potential purchasable robots.
        {
            var immediateCosts = costs[rt];
            if (immediateCosts[0] <= mined[0] &&
                immediateCosts[1] <= mined[1] &&
                immediateCosts[2] <= mined[2] &&
                immediateCosts[3] <= mined[3] &&
                robotsOwned[rt] < maxrobots[rt]) // we can buy one!
            {
                potentialPurchases.Add(rt);
            }
        }
        if (purchaseType == -1) break;
    }

    // Then mining action.
    for (int i = 0; i < 4; i++)
    {
        mined[i] += robotsOwned[i];
    }

    int[] nmined;
    int[] nRO;

    // Add the buy nothing state.
    // But if we've collected more than we could spend then don't do a no-spend option.
    if (!(mined[0] > maxrobots[0] && mined[1] > maxrobots[1] && mined[2] > maxrobots[2]))
    {
        nmined = new int[4];
        nRO = new int[4];
        Array.Copy(mined, nmined, 4);
        Array.Copy(robotsOwned, nRO, 4);
        var nochangedstate = new State() { mined = nmined, robotsOwned = nRO };
        maxGeodes = CalculateGeodes(count, costs, maxrobots, nochangedstate, clock + 1);
    }

    // Then update owned robots.
    foreach (int robottype in potentialPurchases)
    {
        nRO = new int[4];
        Array.Copy(robotsOwned, nRO, 4);
        nRO[robottype] += 1;
        nmined = new int[4];
        Array.Copy(mined, nmined, 4);

        var immediateCosts = costs[robottype];
        nmined[0] -= immediateCosts[0];
        nmined[1] -= immediateCosts[1];
        nmined[2] -= immediateCosts[2];
        nmined[3] -= immediateCosts[3];

        var alteredstate = new State() { mined = nmined, robotsOwned = nRO };
        int alteredg = CalculateGeodes(count, costs, maxrobots, alteredstate, clock + 1);
        if (alteredg > maxGeodes) maxGeodes = alteredg;
    }

    cache.Add((stateKey, clock), maxGeodes);
    return maxGeodes;
}

static string StateStr(State st)
{
    return $"{st.mined[0]}-{st.mined[1]}-{st.mined[2]}-{st.mined[3]}-{st.robotsOwned[0]}-{st.robotsOwned[1]}-{st.robotsOwned[2]}-{st.robotsOwned[3]}";
}

static (long,long) StateLongs(State st)
{
    long p1 = ((long)Math.Pow(3, st.mined[0])) +
        ((long)Math.Pow(5, st.mined[1])) +
        ((long)Math.Pow(7, st.mined[2])) +
        ((long)Math.Pow(11, st.mined[3]));

    long p2 = ((long)Math.Pow(3, st.robotsOwned[0])) +
        ((long)Math.Pow(5, st.robotsOwned[1])) +
        ((long)Math.Pow(7, st.robotsOwned[2])) +
        ((long)Math.Pow(11, st.robotsOwned[3]));


    return (p1, p2);
}

// State of current cycle.
record struct State
{
    public int[] robotsOwned { get; init; }
    public int[] mined { get; init; }

}

