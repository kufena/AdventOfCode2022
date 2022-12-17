// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines(args[0]);

Console.WriteLine(lines.Count());
Console.WriteLine(lines[0].Length);
Console.WriteLine(lines[0]);

