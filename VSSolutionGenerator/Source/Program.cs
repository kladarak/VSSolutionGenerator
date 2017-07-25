using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.IO;

namespace VSSolutionGenerator
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Length < 1)
			{
				System.Console.WriteLine("Please pass json input file.");
				return;
			}

			SolutionGenerator gen = new SolutionGenerator(args[0]);
			gen.Generate();
		}
	}
}
