using System;
using System.IO;
using SSED;

namespace SSED_Cli
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var info = new FileInfo(args[0]);

            if (!info.Exists)
            {
                Console.WriteLine($"The file {info.Name} does not exist.");
                return;
            }
            
            using var file = File.OpenRead(Path.Join(Directory.GetCurrentDirectory(), args[0]));
            using var reader = new StreamReader(file);

            var page = PageParser.Parse(reader.ReadToEndAsync().Result);

            File.WriteAllText(
                Path.Join(Directory.GetCurrentDirectory(), Path.GetFileNameWithoutExtension(args[0]) + ".html"),
                page.ToHtml().ToString());
        }
    }
}