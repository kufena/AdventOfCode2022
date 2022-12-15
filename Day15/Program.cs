// See https://aka.ms/new-console-template for more information
using System.Data.Common;
using System.Diagnostics;
using System.Runtime.ExceptionServices;

Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines(args[0]);

long t = Stopwatch.GetTimestamp();
//Part1(args, lines);
//Part2(args, lines);
Part2Circles(args, lines);
Console.WriteLine($"Took {Stopwatch.GetTimestamp() - t}");

((long bx, long by), (long sx, long sy)) ParseLine(string line)
{
    // Sensor at x=8, y=7: closest beacon is at x=2, y=10
    var splits = line.Split(new char[] { '=', ',', ':' }, StringSplitOptions.RemoveEmptyEntries);
    long sx = long.Parse(splits[1]);
    long sy = long.Parse(splits[3]);
    long bx = long.Parse(splits[5]);
    long by = long.Parse(splits[7]);
    return ((bx, by), (sx, sy));
}

void pointsAbout((long, long) point, long dist, long row, HashSet<(long,long)> res)
{
    (long x, long y) = point;
    long xdist = dist - Math.Abs(y - row);
    long numpoints = (xdist * 2) + 1;
    //sHashSet<(long, long)> res = new();
    for (long i = x - xdist; i <= x + xdist; i++)
    {
        //for (long j = y - dist; j <= y + dist; j++)
        //{
        long d = Math.Abs(x - i) + Math.Abs(y - row);
        if (d <= dist)
            res.Add((i, row));
        //}
    }
    //Debug.Assert(res.Count == numpoints);
    //return res;
}

void pointsAbout2((long, long) point, long dist, long row, HashSet<(long, long)> res, long minx, long maxx)
{
    (long x, long y) = point;
    long xdist = dist - Math.Abs(y - row);
    long numpoints = (xdist * 2) + 1;
    long lowi = x - xdist;
    if (lowi < minx) lowi = minx;
    long highi = x + xdist;
    if (highi > maxx) highi = maxx;
    //sHashSet<(long, long)> res = new();
    for (long i = lowi; i <= highi; i++)
    {
        //for (long j = y - dist; j <= y + dist; j++)
        //{
        long d = Math.Abs(x - i) + Math.Abs(y - row);
        if (d <= dist)
            res.Add((i, row));
        //}
    }
    //Debug.Assert(res.Count == numpoints);
    //return res;
}

static long CountPointsOnLine(long row, HashSet<(long, long)> map, long lowx, long highx)
{
    long count = 0;
    for (long i = lowx; i <= highx; i++)
    {
        if (map.Contains((i, row)))
        {
            //Console.WriteLine($"point ({i},{row}) is covered.");
            count += 1;
        }
    }

    return count;
}

static long CountPointsOnLine2(long row, HashSet<(long, long)> map, long lowx, long highx, out HashSet<(long,long)> uncovered)
{
    long count = 0;
    uncovered = new HashSet<(long, long)>();
    for (long i = lowx; i <= highx; i++)
    {
        if (map.Contains((i, row)))
        {
            //Console.WriteLine($"point ({i},{row}) is covered.");
            count += 1;
        }
        else
        {
            uncovered.Add((i, row));
        }
    }

    return count;
}
void Part1(string[] args, string[] lines)
{
    long row = long.Parse(args[1]);

    HashSet<(long, long)> map = new();
    Dictionary<(long, long), long> distances = new();
    HashSet<(long, long)> beacons = new();
    HashSet<(long, long)> sensors = new();

    long lowx = long.MaxValue;
    long lowy = long.MaxValue;
    long highx = long.MinValue;
    long highy = long.MinValue;
    long maxdistance = 0;

    foreach (var line in lines)
    {
        ((long bx, long by), (long sx, long sy)) = ParseLine(line);
        long distance = Math.Abs(bx - sx) + Math.Abs(by - sy);

        distances.Add((sx, sy), distance);
        beacons.Add((bx, by));
        sensors.Add((sx, sy));

        map.Add((sx, sy));
        map.Add((bx, by));

        Console.WriteLine($"Sensor at ({sx},{sy}) beacon at ({bx},{by}): {distance}");

        if (bx < lowx) lowx = bx;
        if (bx > highx) highx = bx;
        if (sx < lowx) lowx = sx;
        if (sx > highx) highx = sx;

        if (by < lowy) lowy = by;
        if (by > highy) highy = by;
        if (sy < lowy) lowy = sy;
        if (sy > highy) highy = sy;

        if (distance > maxdistance) maxdistance = distance;
    }

    lowx -= maxdistance;
    highx += maxdistance;
    lowy -= maxdistance;
    highy += maxdistance;

    Console.WriteLine($"High x = {highx} Low x = {lowx}");
    Console.WriteLine($"High y = {highy} Low y = {lowy}");

    foreach (((long xs, long ys), long dist) in distances)
    {
        //Console.WriteLine($"({xs},{ys}) dist = {dist} ys-dist = {ys-dist} ys+dist = {ys + dist} row = {row}");
        if ((row <= ys && (ys - dist) <= row) || (row >= ys && (ys + dist) >= row))
        {
            Console.WriteLine($"Handling point ({xs},{ys})...");
            //var npoints = 
            pointsAbout((xs, ys), dist, row, map);
            //foreach (var p in npoints) map.Add(p);
        }
    }

    long count = CountPointsOnLine(row, map, lowx, highx);

    Console.WriteLine($"There are {count} points covered.");
    foreach ((long x, long y) in beacons)
        if (y == row) count += -1;
    Console.WriteLine($"There are {count} points covered, minus beacons.");
}

void Part2(string[] args, string[] lines)
{
    long maxrow = long.Parse(args[1]);
    long minrow = 0;

    HashSet<(long, long)> protomap = new();
    Dictionary<(long, long), long> distances = new();
    HashSet<(long, long)> beacons = new();
    HashSet<(long, long)> sensors = new();

    long lowx = long.MaxValue;
    long lowy = long.MaxValue;
    long highx = long.MinValue;
    long highy = long.MinValue;
    long maxdistance = 0;
    
    foreach (var line in lines)
    {
        ((long bx, long by), (long sx, long sy)) = ParseLine(line);
        long distance = Math.Abs(bx - sx) + Math.Abs(by - sy);

        distances.Add((sx, sy), distance);
        beacons.Add((bx, by));
        sensors.Add((sx, sy));

        protomap.Add((sx, sy));
        protomap.Add((bx, by));

        Console.WriteLine($"Sensor at ({sx},{sy}) beacon at ({bx},{by}): {distance}");

        if (bx < lowx) lowx = bx;
        if (bx > highx) highx = bx;
        if (sx < lowx) lowx = sx;
        if (sx > highx) highx = sx;

        if (by < lowy) lowy = by;
        if (by > highy) highy = by;
        if (sy < lowy) lowy = sy;
        if (sy > highy) highy = sy;

        if (distance > maxdistance) maxdistance = distance;
    }

    lowx -= maxdistance;
    highx += maxdistance;
    lowy -= maxdistance;
    highy += maxdistance;

    Console.WriteLine($"High x = {highx} Low x = {lowx}");
    Console.WriteLine($"High y = {highy} Low y = {lowy}");

    for (long x = 0; x <= maxrow; x += 1000)
    {
        Console.WriteLine("+");
        long qhighx = x + 999;
        if (qhighx > maxrow) qhighx = maxrow;
        long val = (qhighx - x) + 1;
        for (long row = minrow; row <= maxrow; row++)
        {
            HashSet<(long, long)> map = new();
            foreach (((long xs, long ys), long dist) in distances)
            {
                //Console.WriteLine($"({xs},{ys}) dist = {dist} ys-dist = {ys-dist} ys+dist = {ys + dist} row = {row}");
                if ((row <= ys && (ys - dist) <= row) || (row >= ys && (ys + dist) >= row))
                {
                    //Console.WriteLine($"Handling point ({xs},{ys})...");
                    //var npoints =
                    pointsAbout2((xs, ys), dist, row, map, x, qhighx);
                    //foreach (var p in npoints) map.Add(p);
                }
            }

            HashSet<(long, long)> uncovered;
            long count = map.Count; // 

            //Console.WriteLine($"There are {count} points covered.");

            if (count < val)
            {
                CountPointsOnLine2(row, map, minrow, maxrow, out uncovered);
                if (uncovered.Count == 1)
                {
                    (long ux, long uy) = uncovered.First();
                    Console.WriteLine($"One point found at {ux},{uy} = calculation of {(ux * 4000000) + uy}");
                    break;
                }
            }
        }
    }
}

void Part2Retry(string[] args, string[] lines)
{
    long maxrow = long.Parse(args[1]);
    long minrow = 0;

    HashSet<(long, long)> protomap = new();
    Dictionary<(long, long), long> distances = new();
    HashSet<(long, long)> beacons = new();
    HashSet<(long, long)> sensors = new();

    long lowx = long.MaxValue;
    long lowy = long.MaxValue;
    long highx = long.MinValue;
    long highy = long.MinValue;
    long maxdistance = 0;

    foreach (var line in lines)
    {
        ((long bx, long by), (long sx, long sy)) = ParseLine(line);
        long distance = Math.Abs(bx - sx) + Math.Abs(by - sy);

        distances.Add((sx, sy), distance);
        beacons.Add((bx, by));
        sensors.Add((sx, sy));

        protomap.Add((sx, sy));
        protomap.Add((bx, by));

        Console.WriteLine($"Sensor at ({sx},{sy}) beacon at ({bx},{by}): {distance}");

        if (bx < lowx) lowx = bx;
        if (bx > highx) highx = bx;
        if (sx < lowx) lowx = sx;
        if (sx > highx) highx = sx;

        if (by < lowy) lowy = by;
        if (by > highy) highy = by;
        if (sy < lowy) lowy = sy;
        if (sy > highy) highy = sy;

        if (distance > maxdistance) maxdistance = distance;
    }

    lowx -= maxdistance;
    highx += maxdistance;
    lowy -= maxdistance;
    highy += maxdistance;

    long count = 1;
    for (long y = 0; y < maxrow; y++)
    {
        bool done = false;
        Console.WriteLine($"row {y}");
        for (long x = 0; x < maxrow; x++)
        {

            count++;
            bool ok = false;
            foreach (((long sx, long sy), long dist) in distances)
            {
                ok = ok || ((Math.Abs(sx - x) + Math.Abs(sy - y)) <= dist);
            }
            if (!ok) // we've found it.
            {
                done = true;
                Console.WriteLine($" {x} {y} {(x * 4000000) + y}");
                break;
            }
            if (done) break;
        }

    }

}

void DistanceCircle(long xs, long ys, long dist, HashSet<(long,long)> res)
{
    dist += 1;
    int c = 0;
    for (long xd = 0; xd <= dist; xd++)
    {
        long x1 = xs + xd;
        long x2 = xs - xd;
        long q = dist - xd;
        res.Add((x1, ys + q));
        res.Add((x1, ys - q));
        res.Add((x2, ys + q));
        res.Add((x2, ys - q));
        c += 4;
    }
    Console.WriteLine($"Sensor at {xs} {ys} add {c} points.");
}

bool TestPoint(long xt, long yt, Dictionary<(long, long), long> distances)
{
    foreach (((long xs, long ys), long dist) in distances)
    {
        if (Math.Abs(xs - xt) + Math.Abs(ys - yt) <= dist)
            return true;
    }
    return false;
}

void TestAllCircles(Dictionary<(long, long), long> distances, long max)
{
    HashSet<(long, long)> allCirclePoints = new();
    foreach (((long xs, long ys), long dist) in distances)
    {
        DistanceCircle(xs, ys, dist, allCirclePoints);
    }
    Console.WriteLine($"Testing {allCirclePoints.Count} points");
    foreach ((long x, long y) in allCirclePoints)
    {
        if (x < 0 || y < 0) continue;
        if (x > max || y > max) continue;
        if (!TestPoint(x, y, distances))
        {
            Console.WriteLine($"We've found {x} {y} - so {(x * 4000000) + y}");
            break;
        }
    }
}

void Part2Circles(string[] args, string[] lines)
{
    long maxrow = long.Parse(args[1]);
    long minrow = 0;

    HashSet<(long, long)> protomap = new();
    Dictionary<(long, long), long> distances = new();
    HashSet<(long, long)> beacons = new();
    HashSet<(long, long)> sensors = new();

    long lowx = long.MaxValue;
    long lowy = long.MaxValue;
    long highx = long.MinValue;
    long highy = long.MinValue;
    long maxdistance = 0;

    foreach (var line in lines)
    {
        ((long bx, long by), (long sx, long sy)) = ParseLine(line);
        long distance = Math.Abs(bx - sx) + Math.Abs(by - sy);

        distances.Add((sx, sy), distance);
        beacons.Add((bx, by));
        sensors.Add((sx, sy));

        protomap.Add((sx, sy));
        protomap.Add((bx, by));

        Console.WriteLine($"Sensor at ({sx},{sy}) beacon at ({bx},{by}): {distance}");

        if (bx < lowx) lowx = bx;
        if (bx > highx) highx = bx;
        if (sx < lowx) lowx = sx;
        if (sx > highx) highx = sx;

        if (by < lowy) lowy = by;
        if (by > highy) highy = by;
        if (sy < lowy) lowy = sy;
        if (sy > highy) highy = sy;

        if (distance > maxdistance) maxdistance = distance;
    }

    lowx -= maxdistance;
    highx += maxdistance;
    lowy -= maxdistance;
    highy += maxdistance;

    TestAllCircles(distances, maxrow);
}
