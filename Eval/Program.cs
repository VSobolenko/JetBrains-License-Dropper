using System;

namespace Eval
{
internal static class Program
{
    public static void Main()
    {
        Action<string> log = Console.WriteLine;

        if (IsRunResetLicense())
        {
            try
            {
                var jetbrainsData = new JetBrainsLicenseReset(log);

                InvokeResetMethod(IsClearAllTempFiles, () => jetbrainsData.ClearTempFiles(),
                    "Cleaning up temporary files", log);
                InvokeResetMethod(IsClearSpecialJavaMemory, () => jetbrainsData.ClearSpecialJavaMemory(),
                    "Clearing Java Special Data", log);
                InvokeResetMethod(() => true, () => jetbrainsData.ClearFolderMemory(),
                    "Clearing folders", log);
                InvokeResetMethod(() => true, () => jetbrainsData.ClearRegistryMemory(),
                    "Clearing registry", log);

                log?.Invoke("License reset complete!");
            }
            catch (Exception e)
            {
                log?.Invoke($"Error reset eval with exception: {e.Message}");
            }
        }
        else
            log?.Invoke("Reset skipped");

        Console.ReadKey();
    }

    private static void InvokeResetMethod(Func<bool> condition, Action action, string description, Action<string> log)
    {
        if (condition.Invoke() == false)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            
            log?.Invoke($"[Skip] {description}");
            Console.ForegroundColor = ConsoleColor.White;

            return;
        }
        Console.ForegroundColor = ConsoleColor.Green;
        log?.Invoke($"[Start] {description}");

        Console.ForegroundColor = ConsoleColor.DarkGreen;
        action?.Invoke();
        Console.ForegroundColor = ConsoleColor.Green;

        log?.Invoke($"[End] {description}\n");
        Console.ForegroundColor = ConsoleColor.White;
    }

    private static bool IsClearSpecialJavaMemory()
    {
        Console.WriteLine("(yes/no) Clean up special folders from java? (if it didn't work without it)");

        return IsUserAgreed();
    }

    private static bool IsRunResetLicense()
    {
        Console.WriteLine("(yes/no) Reset license?\n");

        return IsUserAgreed();
    }

    private static bool IsClearAllTempFiles()
    {
        Console.WriteLine("(yes/no) Clear all temp files? (desirable)?");

        return IsUserAgreed();
    }

    private static bool IsUserAgreed()
    {
        var userInput = Console.ReadLine();

        return userInput == "yes";
    }
}
}