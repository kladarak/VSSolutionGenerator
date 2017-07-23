using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSSolutionGenerator
{
	class FileUtils
	{
		public static string NormaliseFilename(string inFilename)
		{
			return inFilename.Replace("/", "\\");
		}
	}
}
