using System;

namespace Eval
{
internal static class Program
{
    public static void Main()
    {
        Action<string> log = Console.WriteLine;

        new LicenseHandler().Start(log);

        Console.ReadKey();
    }
}
}