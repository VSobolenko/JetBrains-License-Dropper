using System;

namespace Eval.Derail
{
    public class LicenseHandler
    {
        public void Start(Action<string> log)
        {
            if (!IsRunResetLicense())
            {
                log("Reset skipped");
                Console.ReadKey();
                return;
            }

            try
            {
                ResetLicense(log);
            }
            catch (Exception e)
            {
                log?.Invoke($"Error reset eval with exception: {e.Message}");
            }
        }

        private void ResetLicense(Action<string> log)
        {
            var jetbrainsData = new JetBrainsLicenseReset(log);

            InvokeResetMethod(IsClearAllTempFiles, () => jetbrainsData.ClearTempFiles(),
                "Cleaning up temporary files", log);
            InvokeResetMethod(IsClearSpecialJavaMemory, () => jetbrainsData.ClearSpecialJavaMemory(),
                "Clearing Java Special Data", log);
            InvokeResetMethod(() => true, () => jetbrainsData.ClearFolderMemoryWithSavingSettings(),
                "Clearing folders", log);
            InvokeResetMethod(() => true, () => jetbrainsData.ClearRegistryMemory(),
                "Clearing registry", log);

            log("License reset complete!");
        }

        private static void InvokeResetMethod(Func<bool> condition, Action action, string description,
            Action<string> log)
        {
            if (condition.Invoke() == false)
            {
                Console.ForegroundColor = ConsoleColor.Red;

                log($"[Skip] {description}");
                Console.ForegroundColor = ConsoleColor.White;

                return;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            log($"[Start] {description}");

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            action?.Invoke();
            Console.ForegroundColor = ConsoleColor.Green;

            log($"[End] {description}\n");
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