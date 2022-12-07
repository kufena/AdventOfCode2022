using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day7
{
    internal abstract class Systeam
    {
        public string name { get; set; }
        public long size { get; set; } = -1;

        public Systeam(string n)
        {
            this.name = n;
        }
        public abstract void setSize();

        public abstract long part1Total();
        public abstract List<Dir> findAtLeast(long sz);
    }

    internal class Fiel : Systeam
    {
        public Fiel(string n, long l) : base(n)
        {
            this.size = l;
        }

        public override void setSize()
        {
            return; // already known.
        }

        public override long part1Total()
        {
            throw new NotImplementedException();
        }

        public override List<Dir> findAtLeast(long sz)
        {
            throw new NotImplementedException();
        }
    }

    internal class Dir : Systeam
    {
        public Dir(string n) : base(n)
        {
        }
        public List<Systeam> entries { get; set; } = new();

        public override void setSize()
        {
            long tot = 0;
            foreach (var s in entries)
            {
                s.setSize();
                tot += s.size;
            }
            this.size = tot;
        }

        public override long part1Total()
        {
            long tot = 0;
            if (this.size <= 100000) // include us.
            {
                Console.WriteLine($"Including {this.name} of size {this.size}");
                tot += this.size;
            }

            foreach (var s in entries)
            {
                if (s is Dir)
                {
                    Dir d = (Dir)s;
                    tot += d.part1Total();                    
                }
            }
            return tot;
        }

        public override List<Dir> findAtLeast(long sz)
        {
            List<Dir> res = new();
            foreach (var s in entries)
            {
                if (s is Dir) 
                {
                    Dir d = (Dir)s;
                    var newl = d.findAtLeast(sz);
                    foreach (var q in newl) res.Add(q);
                }
            }

            if (this.size >= sz)
                res.Add(this);

            return res;
        }
    }
}
