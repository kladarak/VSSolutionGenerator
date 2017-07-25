using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSSolutionGenerator
{
	class SolutionSettingsJsonData
	{
		public string solutionName = "";
		public string projectSourceDirectory = "";

		public string solutionDirectory = "";
		public string binariesDirectory = "";
	}

	class ProjectSettingsJsonData
	{
		public string projectName = "";
		public string configType = "";
		public string sourceDirectory = "";
		public string precompiledHeaderName = "";

		public List<string> includePaths = new List<string>();
		public List<string> projectDependencies = new List<string>();

		public string mSettingsFilename = "";
	}
}
