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

                    if (IsClearAllTempFiles())
                        jetbrainsData.ClearTempFiles();
                    if (IsClearSpecialJavaMemory())
                        jetbrainsData.ClearSpecialJavaMemory();
                
                    jetbrainsData.ClearFolderMemory();
                    jetbrainsData.ClearRegistryMemory();

                    log.Invoke("License reset!");
                }
                catch (Exception e)
                {
                    log.Invoke($"Error reset eval with exception: {e.Message}");
                }   
            }
            else
            {
                log.Invoke("Reset skipped");
            }

            Console.ReadKey();
        }

        private static bool IsClearSpecialJavaMemory()
        {
            Console.WriteLine("(yes/no) Clean up special folders from java? (if it didn't work without it)");
            return IsUserAgreed();
        }
        
        private static bool IsRunResetLicense()
        {
            Console.WriteLine("(yes/no) Reset license?");
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