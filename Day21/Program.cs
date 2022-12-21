// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using System.Net.NetworkInformation;

Console.WriteLine("Hello, World!");
var lines = File.ReadAllLines(args[0]);
Node[] node = new Node[lines.Length];
Dictionary<string, Node> map = new Dictionary<string, Node>();

for (int i = 0; i < lines.Length; i++)
{
    var splits = lines[i].Split(new char[] { ' ', ':' }, StringSplitOptions.RemoveEmptyEntries);
    if (splits.Length == 2)
    {
        node[i] = new Node() { id = splits[0], value = long.Parse(splits[1]), isValue = true };
    }
    else
    {
        node[i] = new Node() { id = splits[0], name1 = splits[1], name2 = splits[3], op = splits[2], isValue = false };
    }
    map.Add(splits[0], node[i]);
}

//Part1(map);
Part2(map);

void Part1(Dictionary<string, Node> map)
{
    Node root = map["root"];
    Tree tree = BuildTree(root, map);
    var val = Calculate(tree);
    Console.WriteLine(val);
}

void Part2(Dictionary<string, Node> map)
{
    Node root = map["root"];
    Node humn = map["humn"];
    Debug.Assert(humn.isValue);

    humn.value = 1;

    Tree tree = BuildTree(root, map);

    Branch rootb = (Branch)tree;
    Tree rearranged;

    if (FindHumn(rootb.left))
    {
        long v = Calculate(rootb.right);
        Console.WriteLine($"Humn is to the left. So we will use right as target. Aim is {v}");
        rearranged = Rearrange(rootb.left, new Leaf() { node = new Node() { id = "", isValue = true, value = v } });
    }
    else
    {
        long v = Calculate(rootb.left);
        Console.WriteLine($"Humn must be on the right. So we will use left as target. Aim is {v}");
        rearranged = Rearrange(rootb.right, new Leaf() { node = new Node() { id = "", isValue = true, value = v } });
    }
    long val = Calculate(rearranged);
    Console.WriteLine($"I think humn = {val}");
}

Tree Rearrange(Tree target, Tree value)
{
    if (target.node.isValue)
    {
        // we shouldn't get here.
        throw new Exception("say what?");
    }

    Branch b = (Branch)target;
    if (b.left.node.id == "humn")
    {
        // so we have "humn" op (something), so need to do opposite op.
        string newop = OppositeOp(b.node.op);
        Branch neb = new Branch() { node = new Node() { id = b.node.id, op = newop, isValue = false }, left = value, right = b.right };
        return neb;
    }
    if (b.right.node.id == "humn")
    {
        Branch neb = null;
        if (b.node.op == "*") { neb = new Branch() { node = new Node() { id = b.node.id, op = "/", isValue = false }, left = value, right = b.left }; }
        if (b.node.op == "/") { neb = new Branch() { node = new Node() { id = b.node.id, op = "/", isValue = false }, left = b.left, right = value }; }
        if (b.node.op == "+") { neb = new Branch() { node = new Node() { id = b.node.id, op = "-", isValue = false }, left = value, right = b.left }; }
        if (b.node.op == "-") { neb = new Branch() { node = new Node() { id = b.node.id, op = "-", isValue = false }, left = b.left, right = value }; }

        return neb;
    }

    // ok, we haven't found humn yet.
    if (FindHumn(b.left))
    {
        Tree newvalue = new Branch() { node = new Node() { id = b.node.id, op = OppositeOp(b.node.op) }, left = value, right = b.right };
        return Rearrange(b.left, newvalue);
    }
    else 
    {
        Tree newvalue = null;
        if (b.node.op == "*") {
            newvalue = new Branch() { node = new Node() { id = b.node.id, op = "/", isValue = false },
                left = value,
                right = b.left };
        }
        if (b.node.op == "/") {
            newvalue = new Branch() { node = new Node() { id = b.node.id, op = "/", isValue = false },
                left = b.left,
                right = value };
        }
        if (b.node.op == "-") {
            newvalue = new Branch() { node = new Node() { id = b.node.id, op = "-", isValue = false },
                left = b.left,
                right = value };
        }
        if (b.node.op == "+") {
            newvalue = new Branch() { node = new Node() { id = b.node.id, op = "-", isValue = false },
                left = value,
                right = b.left };
        }
        return Rearrange(b.right, newvalue);
    }


}

string OppositeOp(string op)
{
    if (op == "+") return "-";
    if (op == "-") return "+";
    if (op == "*") return "/";
    if (op == "/") return "*";
    throw new Exception($"Unknown op {op}");
}

bool FindHumn(Tree t)
{
    if (t.node.id == "humn")
        return true;
    else
    {
        if (t is Branch)
        {
            Branch tb = (Branch)t;
            return FindHumn(tb.right) || FindHumn(tb.left);
        }
    }
    return false;
}

Tree BuildTree(Node root, Dictionary<string, Node> map)
{
    if (root.isValue)
    {
        return new Leaf() { node = root };
    }

    else
    {
        var left = BuildTree(map[root.name1], map);
        var right = BuildTree(map[root.name2], map);
        return new Branch() { left = left, right = right, node = root };
    }
}

long Calculate(Tree tree)
{
    if (tree is Leaf)
    {
        Leaf l = (Leaf)tree;
        Debug.Assert(l.node.isValue);
        return l.node.value;
    }

    else
    {
        Branch b = (Branch)tree;
        if (b.node.isValue)
        {
            return b.node.value;
        }

        if (!b.right.node.isValue)
        {
            Calculate(b.right);
        }
        if (!b.left.node.isValue)
        {
            Calculate(b.left);
        }
        long res = 0;
        if (b.node.op == "+")
            res = b.left.node.value + b.right.node.value;
        if (b.node.op == "*")
            res = b.left.node.value * b.right.node.value;
        if (b.node.op == "-")
            res = b.left.node.value - b.right.node.value;
        if (b.node.op == "/")
            res = b.left.node.value / b.right.node.value;
        b.node.value = res;
        b.node.isValue = true;
        return res;
    }
}

class Node
{
    public string id { get; init; }
    public long value { get; set; } = 0;
    public string name1 { get; set; }
    public string name2 { get; set; }
    public string op { get; set; }

    public bool isValue { get; set; } = false;
}

abstract class Tree
{
    public Node node { get; set; }
}
class Leaf : Tree
{
}
class Branch : Tree
{
    public Tree left { get; set; }
    public Tree right { get; set; }
}