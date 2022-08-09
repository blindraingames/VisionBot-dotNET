using System;
using System.IO;
using BlindRainGames.Utils.VisionBot;

namespace VisionBotTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // User interface
            var input = string.Empty;
            Console.WriteLine("Program to recognize images with VisionBot service.");
            if ((args != null) && (args.Length >= 1) && (args[0] != string.Empty))
            {
                Console.WriteLine("Provided image by path: " + args[0]);
                input = args[0];
            }
            else
            {
                Console.WriteLine("Please, type full path to image.");
                input = Console.ReadLine();
            }

            if (File.Exists(input))
            {
                Console.WriteLine("Begin working... Please, wait...");

                // Read file and send it to VisionBot async
                var bytes = File.ReadAllBytes(input);
                var vb = new VisionBot();
                var tsk = vb.GetImageDescriptionAsStringAsync(bytes, ERecognitionType.all, true, "en", true, 1000, 15);

                // Wait for task completed and print result of recognition.
                tsk.Wait();
                if (tsk.Result != String.Empty)
                    Console.WriteLine($"Image recognized. Text is:{Environment.NewLine}{tsk.Result}");
                else
                    Console.WriteLine("Error happens!");
                vb.Dispose();
            } // input is valid
            else
                Console.WriteLine("Sorry, you have mistakes in your path.");

            Console.WriteLine("Program is finished. Press any key.");
            Console.ReadKey();
        }
    }
}
//EndFile//