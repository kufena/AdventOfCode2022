// See https://aka.ms/new-console-template for more information
using System.Diagnostics.CodeAnalysis;
using System.Runtime;

Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines(args[0]);

var lookup = new Dictionary<char, int>();
lookup.Add('0', 0);
lookup.Add('1', 1);
lookup.Add('2', 2);
lookup.Add('-', -1);
lookup.Add('=',-2);

long total = 0;
char[] btotal = new char[] { '0' };
foreach (var line in lines)
{
    long v = CharsToVal(lookup, line);
    long w = CharsToVal(lookup, new string(btotal));

    total += v;
    Console.Write($"Adding {new string(btotal)} to {line} or {w} to {v} ");
    btotal = Addition(btotal, line.ToCharArray());
    Console.WriteLine($"= {new string(btotal)} or {CharsToVal(lookup, new string(btotal))} ({v+w})");
}

Console.WriteLine($"Sum is {total}");
Console.WriteLine($"BSUm is {new String(btotal)} or {CharsToVal(lookup, new string(btotal))}");

Console.WriteLine($"Order of {Math.Log(total, 5)}");

char[] Addition(char[] a, char[] b)
{
    char rem = '0';
    int len = a.Length > b.Length ? a.Length : b.Length;
    len += 1;
    char[] result = new char[len];
    for (int j = 0; j < len; j++)
    {
        result[j] = '0';
    }

    int i;
    for (i = 0; i < len; i++)
    {
        if (i >= a.Length && i >= b.Length)
            break;

        int charind = len - (i + 1);
        if (i >= a.Length)
        {
            if (rem != '0')
            {
                result[charind] = AddDigits(rem, b[b.Length - (1+i)], out rem);
            }
            else
            {
                result[charind] = b[b.Length - (1+i)];
            }
        }
        else if (i >= b.Length)
        {
            if (rem != '0')
            {
                result[charind] = AddDigits(rem, a[a.Length - (1+i)], out rem);
            }
            else
            {
                result[charind] = a[a.Length-(1+i)];
            }
        }
        else
        {
            char subrem;
            char sub = AddDigits(rem, a[a.Length - (1 + i)], out subrem);
           
            result[charind] = AddDigits(sub, b[b.Length-(1+i)], out rem);
            if (subrem != '0')
            {
                rem = AddDigits(subrem, rem, out subrem);
            }

        }
    }

    if (rem != '0')
    {
        result[len-(i+1)] = rem;
    }
    return result;
}

char AddDigits(char a, char b, out char rem)
{
    rem = '0';
    switch (a)
    {
        case '-':
            switch (b)
            {
                case '=':
                    rem = '-';
                    return '2';
                case '-':
                    rem = '0';
                    return '=';
                case '0':
                    rem = '0';
                    return '-';
                case '1':
                    rem = '0';
                    return '0';
                case '2':
                    rem = '0';
                    return '1';
                default:
                    throw new Exception("what?");
            }
        case '=':
            switch (b)
            {
                case '=':
                    rem = '-';
                    return '1';
                case '-':
                    rem = '-';
                    return '2';
                case '0':
                    rem = '0';
                    return '=';
                case '1':
                    rem = '0';
                    return '-';
                case '2':
                    rem = '0';
                    return '0';
                default:
                    throw new Exception("what?");
            }
        case '0':
            return b;
        case '1':
            switch (b)
            {
                case '=':
                    rem = '0';
                    return '-';
                case '-':
                    rem = '0';
                    return '0';
                case '0':
                    rem = '0';
                    return '1';
                case '1':
                    rem = '0';
                    return '2';
                case '2':
                    rem = '1';
                    return '=';
                default:
                    throw new Exception("what?");
            }
        case '2':
            switch (b)
            {
                case '=':
                    rem = '0';
                    return '0';
                case '-':
                    rem = '0';
                    return '1';
                case '0':
                    rem = '0';
                    return '2';
                case '1':
                    rem = '1';
                    return '=';
                case '2':
                    rem = '1';
                    return '-';
                default:
                    throw new Exception("what?");
            }
        default:
            throw new Exception("no no no");

    }
}

static long CharsToVal(Dictionary<char, int> lookup, string line)
{
    char[] charline = line.ToCharArray();
    long count = 0;
    long sum = 0;
    for (int x = line.Length - 1; x >= 0; x--)
    {
        sum += (long)((lookup[charline[x]]) * (Math.Pow(5, count)));
        count++;
    }
    //Console.WriteLine($"{line} = {sum}");
    return sum;
}