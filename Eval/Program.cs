using System;

namespace Eval
{
    internal static class Program
    {
        public static void Main()
        {
            if (IsRunResetLicense())
            {
                try
                {
                    var jetbrainsData = new JetBrainsLicenseReset();
                
                    if (IsClearSpecialJavaMemory())
                        jetbrainsData.ClearSpecialJavaMemory();
                
                    jetbrainsData.ClearFolderMemory();
                    jetbrainsData.ClearRegistryMemory();

                    Console.WriteLine("License reset!");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error reset eval with exception: {e.Message}");
                }   
            }
            else
            {
                Console.WriteLine("Reset skipped");
            }

            Console.ReadKey();
        }

        private static bool IsClearSpecialJavaMemory()
        {
            Console.WriteLine("(yes/no) Clean up special folders from java? (if it didn't work without it)");
            var clearJavaFolder = Console.ReadLine();
            return clearJavaFolder == "yes";
        }
        
        private static bool IsRunResetLicense()
        {
            Console.WriteLine("(yes/no) Reset license?");
            var clearJavaFolder = Console.ReadLine();
            return clearJavaFolder == "yes";
        }
    }
}