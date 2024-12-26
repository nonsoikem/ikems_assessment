using System.Collections.Concurrent;

namespace ikems_assessment
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            //Challenge one
            var folderPath = @"C:\MyTestFolderPath"; 
            var searchString = "testString";
            var fileResults = await FileScanner.ScanFolderAsync(folderPath, searchString);


            //challenge two
            var collectionA = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
            var collectionS = new List<int> { 5, 15, 3, 19, 35, 50, -1, 0 };
            var duplicateResults = DuplicateIdentifier.FindDuplicates(collectionA, collectionS);
        }
    }
    public static class FileScanner
    {
        private static readonly HashSet<string> AllowedExtensions = new HashSet<string>
        {
            ".txt", ".csv", ".json", ".xml", ".html", ".cs", ".js", ".css", ".sql"
        };

        public static async Task<ConcurrentBag<string>> ScanFolderAsync(string folderPath, string searchString)
        {
            //Used ConcurrentBag here for thread safety
            var results = new ConcurrentBag<string>();

            if (string.IsNullOrEmpty(folderPath))
            {
                Console.WriteLine($"Folder path cannot be null or empty");
                return results;
            }
               

            if (string.IsNullOrEmpty(searchString))
            {
                Console.WriteLine($"Search string cannot be null or empty.");
                return results;
            }
               

            if (!Directory.Exists(folderPath))
            {
                Console.WriteLine($"The folder path '{folderPath}' does not exist.");
                return results;
            }
              

           
            var tasks = new List<Task>();

            foreach (var file in Directory.GetFiles(folderPath))
            {
                //using concurrent async tasks to increase performance 
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        //Checking if the file is readable and skipping non text files
                        if (IsTextFile(file))
                        {
                            var content = await File.ReadAllTextAsync(file);
                           
                                if (content.Contains(searchString))
                                {
                                    results.Add($"Present:{Path.GetFileName(file)}");
                                    Console.WriteLine($"Present:{Path.GetFileName(file)}");
                                }
                                else
                                {
                                    results.Add($"Absent:{Path.GetFileName(file)}");
                                    Console.WriteLine($"Absent:{Path.GetFileName(file)}");
                            }
                            
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing file {file}: {ex.Message}");
                    }
                }));
            }

            await Task.WhenAll(tasks);
            return results;
        }
        private static bool IsTextFile(string filePath)
        {
            var extension = Path.GetExtension(filePath)?.ToLowerInvariant();
            return extension != null && AllowedExtensions.Contains(extension);
        }
    }

    public static class DuplicateIdentifier
    {
        public static List<string> FindDuplicates<T>(List<T> collectionA, List<T> collectionS) where T : IEquatable<T>
        {
            var results = new List<string>();
            if (collectionA == null || collectionS == null)
            {
                Console.WriteLine($"Error: collection has a null value ");
                return results;
            }

            
            // Using a HashSet here for a more efficient lookup
            var setA = new HashSet<T>(collectionA);

            foreach (var item in collectionS)
            {
                if (setA.Contains(item))
                {
                    results.Add($"{item}:true");
                    Console.WriteLine($"{item}:true");
                }
                else
                {

                    results.Add($"{item}:false");
                    Console.WriteLine($"{item}:false");
                }
            }

            return results;
        }
    }
}