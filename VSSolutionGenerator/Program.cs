using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.IO;

namespace VSSolutionGenerator
{
    class JsonInputFile
    {
        public string path { get; set; }
        public string projectName { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                System.Console.WriteLine("Please pass json input file.");
                return;
            }

            string jsonInputFilename = args[0];
            string jsonInputFileContents = File.ReadAllText(jsonInputFilename);

            if (jsonInputFileContents.Length == 0)
            {
                System.Console.WriteLine("Failed to read json input file \"" + jsonInputFilename + "\"");
                return;
            }

            JsonInputFile jsonInput = JsonConvert.DeserializeObject<JsonInputFile>(jsonInputFileContents);
            System.Console.WriteLine("path = " + jsonInput.path);
            System.Console.WriteLine("projectName = " + jsonInput.projectName);
        }
    }
}
