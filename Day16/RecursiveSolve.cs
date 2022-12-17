using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Day16
{
    internal class RecursiveSolve
    {
        Dictionary<string, Node> valves;
        public RecursiveSolve(Dictionary<string, Node> vvs)
        {
            this.valves = vvs;
        }

        public string SetToString(HashSet<string> h) 
        {
            var st = new StringBuilder();
            var arr = h.ToArray<string>();
            Array.Sort(arr);
            for (int i = 0; i < arr.Length; i++) 
                st.Append(arr[i]);
            return st.ToString();
        }

        //
        //  This is a solution - a recursive solution - that uses a cache to cache return values from the
        //  Solve function, so that it doesn't take forever to run.  This is a neat solution, but I did not
        //  come up with it - see https://www.youtube.com/watch?v=rN4tVLnkgJU
        //
        //

//        Dictionary<(string, int, HashSet<string>), int> cache = new Dictionary<(string, int, HashSet<string>), int>();
        Dictionary<(string, int, string), int> cache = new Dictionary<(string, int, string), int>();

        public int Solve(Node node, int clock, HashSet<string> visited)
        {
            if (clock <= 0) return 0;

            string detisiv = SetToString(visited);

            if (cache.ContainsKey((node.name, clock, detisiv))) {
                return cache[(node.name, clock, detisiv)];
            }
            else {
                int withopen = 0;
                int notopen = 0;

                if (node.flow > 0 && !visited.Contains(node.name))
                {
                    int v = (clock - 1) * node.flow;
                    HashSet<string> newvisited = new HashSet<string>();
                    foreach (var s in visited) newvisited.Add(s);
                    newvisited.Add(node.name);
                    foreach (var vert in node.vertices)
                    {
                        int x = Solve(vert, clock - 2, newvisited);
                        if (x + v > withopen) 
                            withopen = x + v;
                    }
                }

                foreach (var vert in node.vertices)
                {
                    int x = Solve(vert, clock - 1, visited);
                    if (notopen < x) 
                        notopen = x;
                }

                int ret = withopen > notopen ? withopen : notopen;
                cache.Add((node.name, clock, detisiv), ret);
                return ret;
            }
        }

        //
        // This is a part two version.
        //
        Dictionary<(string, int, string, bool), int> ele_cache = new Dictionary<(string, int, string, bool), int>();

        public int SolveWithElephant(Node node, int clock, HashSet<string> visited, bool ele_waiting)
        {
            if (clock <= 0)
            {
                if (ele_waiting)
                    return SolveWithElephant(valves["AA"], 26, visited, false);
                return 0;
            }

            string detisiv = SetToString(visited);

            if (ele_cache.ContainsKey((node.name, clock, detisiv, ele_waiting)))
            {
                return ele_cache[(node.name, clock, detisiv, ele_waiting)];
            }
            else
            {
                int withopen = 0;
                int notopen = 0;

                if (node.flow > 0 && !visited.Contains(node.name))
                {
                    int v = (clock - 1) * node.flow;
                    HashSet<string> newvisited = new HashSet<string>();
                    foreach (var s in visited) newvisited.Add(s);
                    newvisited.Add(node.name);
                    foreach (var vert in node.vertices)
                    {
                        int x = SolveWithElephant(vert, clock - 2, newvisited, ele_waiting);
                        if (x + v > withopen)
                            withopen = x + v;
                    }
                }

                foreach (var vert in node.vertices)
                {
                    int x = SolveWithElephant(vert, clock - 1, visited, ele_waiting);
                    if (notopen < x)
                        notopen = x;
                }

                int ret = withopen > notopen ? withopen : notopen;
                ele_cache.Add((node.name, clock, detisiv, ele_waiting), ret);
                return ret;
            }
        }
    }
}
