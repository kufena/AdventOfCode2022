// See https://aka.ms/new-console-template for more information
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;

Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines(args[0]);

//Part1WithLinks(lines);
Part2WithLinks(lines);

static void Part2WithLinks(string[] lines)
{
    long key = 811589153;

    Node[] inorder = new Node[lines.Length];
    Node zero = null;

    Node fst = null;
    Node lst = null;

    // Build the nodes, double linking back/forward between nodes, in order.
    // The array holds the nodes as they were read from the file.
    Node current = null;

    for (int i = 0; i < lines.Length; i++)
    {
        Node n = new Node { num = long.Parse(lines[i]) * key };
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

    // Now loop over the nodes as they are, in file order.
    for (int ff = 0; ff < 10; ff++)
    {
        for (int i = 0; i < inorder.Length; i++)
        {
            // bun is the current node, a la file order.
            Node bun = inorder[i];

            // position will be how many moves we have to make.
            long position = 0;

            if (bun.num > 0)
                position = bun.num % (inorder.Length - 1);
            else
                position = bun.num % (inorder.Length - 1);

            if (position == 0) continue;

            if (position > 0)
            {
                // move forward by 'position' steps.
                for (long j = 0; j < position; j++)
                {
                    bun = bun.fwd;
                }
            }
            else if (position < 0)
            {
                // we're moving backwards so go position steps backwards.
                for (long j = position - 1; j < 0; j++)
                {
                    bun = bun.bck;
                }

                /*
                // remove inorder[i] from the list.
                inorder[i].bck.fwd = inorder[i].fwd;
                inorder[i].fwd.bck = inorder[i].bck;
                // now insert before current.
                var oldbck = bun.bck;
                bun.bck = inorder[i];
                inorder[i].fwd = bun;
                oldbck.fwd = inorder[i];
                inorder[i].bck = oldbck;
                */
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

static void Part1WithLinks(string[] lines)
{
    Node[] inorder = new Node[lines.Length];
    Node zero = null;

    Node fst = null;
    Node lst = null;

    // Build the nodes, double linking back/forward between nodes, in order.
    // The array holds the nodes as they were read from the file.
    Node current = null;

    for (int i = 0; i < lines.Length; i++)
    {
        Node n = new Node { num = long.Parse(lines[i]) };
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

    // Now loop over the nodes as they are, in file order.
    for (int i = 0; i < inorder.Length; i++)
    {
        // bun is the current node, a la file order.
        Node bun = inorder[i];

        // position will be how many moves we have to make.
        long position = 0;

        if (bun.num > 0)
            position = bun.num % (inorder.Length - 1);
        else
            position = bun.num % (inorder.Length - 1);

        if (position == 0) continue;

        if (position > 0)
        {
            // move forward by 'position' steps.
            for (long j = 0; j < position; j++)
            {
                bun = bun.fwd;
            }
        }
        else if (position < 0)
        {
            // we're moving backwards so go position steps backwards.
            for (long j = position - 1; j < 0; j++)
            {
                bun = bun.bck;
            }

            /*
            // remove inorder[i] from the list.
            inorder[i].bck.fwd = inorder[i].fwd;
            inorder[i].fwd.bck = inorder[i].bck;
            // now insert before current.
            var oldbck = bun.bck;
            bun.bck = inorder[i];
            inorder[i].fwd = bun;
            oldbck.fwd = inorder[i];
            inorder[i].bck = oldbck;
            */
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
    public long num { get; set; }
    public Node fwd { get; set; }
    public Node bck { get; set; }
}