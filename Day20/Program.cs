// See https://aka.ms/new-console-template for more information
using System.ComponentModel;
using System.Diagnostics;

Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines(args[0]);

Part1WithLinks(lines);

// Don't understand why this doesn't work, but there you are.
static void Part1WithLinks(string[] lines)
{
    Node[] inorder = new Node[lines.Length];
    Node zero = null;

    Node fst = null;
    Node lst = null;
    Node current = null;

    for (int i = 0; i < lines.Length; i++)
    {
        Node n = new Node { num = int.Parse(lines[i]), origindex = i, currentindex = i };
        inorder[i] = n;
        if (n.num == 0) zero = n;
        if (fst == null) fst = n;
        lst = n;
        if (current != null)
        {
            current.fwd = n;
            n.bck = current;
        }
        current = n;
    }

    fst.bck = lst;
    lst.fwd = fst;

    for (int i = 0; i < inorder.Length; i++)
    {
        Node bun = inorder[i];
        int position = 0;

        if (bun.num > 0)
            position = bun.num % inorder.Length;
        else
            position = bun.num % inorder.Length; // -(Math.Abs(bun.num) % inorder.Length);

        if (position == 0) continue;
        //if (position < 0) position = inorder.Length + position - 1;

        if (position > 0)
        {
            for (int j = 0; j < position; j++)
            {
                bun = bun.fwd;
            }
            var tst = inorder[i];
            for (int j = 0; j < (position % inorder.Length); j++)
                tst = tst.fwd;

            if (bun.num != tst.num)
            {
                Console.WriteLine($"Comparing {inorder[i].num} fwd to {inorder[i].num % inorder.Length} gives different nums.");
                Console.WriteLine($"{bun.num} and {tst.num} at line {i}");
                Console.ReadLine();
            }

            // insert after current.
            if (Object.ReferenceEquals(bun.fwd, inorder[i]))
            {
                Console.WriteLine($"Got that funny ordering thing where we do nothing! Positive num {inorder[i].num}");
                // we're inserting it after a thing but it's already us.
                continue;
            }
            // remove inorder[i] from the list.
            inorder[i].bck.fwd = inorder[i].fwd;
            inorder[i].fwd.bck = inorder[i].bck;
            // now insert after current.
            var oldfwd = bun.fwd;
            bun.fwd = inorder[i];
            inorder[i].bck = bun;
            oldfwd.bck = inorder[i];
            inorder[i].fwd = oldfwd;
        }
        else if (position < 0)
        {
            //Console.WriteLine("Not reachable!");
            for (int j = position; j < 0; j++)
            {
                bun = bun.bck;
            }
            // insert before current.
            if (Object.ReferenceEquals(bun.bck, inorder[i]))
            {
                Console.WriteLine($"Got that funny ordering thing where we do nothing! Negative num {inorder[i].num}");
                // we're inserting it after a thing but it's already us.
                continue;
            }
            // remove inorder[i] from the list.
            inorder[i].bck.fwd = inorder[i].fwd;
            inorder[i].fwd.bck = inorder[i].bck;
            // now insert after current.
            var oldbck = bun.bck;
            bun.bck = inorder[i];
            inorder[i].fwd = bun;
            oldbck.fwd = inorder[i];
            inorder[i].bck = oldbck;
        }
    }

    var onefst = zero;
    for (int i = 0; i < (1000 % inorder.Length); i++)
        onefst = onefst.fwd;
    var snd = zero;
    for (int i = 0; i < (2000 % inorder.Length); i++)
        snd = snd.fwd;
    var thd = zero;
    for (int i = 0; i < (3000 % inorder.Length); i++)
        thd = thd.fwd;
    Console.WriteLine($"Numbers are {onefst.num} {snd.num} {thd.num} so sum is {onefst.num + snd.num + thd.num}");

}

class Node
{
    public int num { get; set; }
    public Node fwd { get; set; }
    public Node bck { get; set; }
    public int origindex { get; set; }
    public int currentindex { get; set; }

}