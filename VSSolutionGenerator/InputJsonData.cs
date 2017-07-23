using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSSolutionGenerator
{
	class ProjectFileJsonData
	{
		public string path { get; set; }
		public string projectName { get; set; }
		public List<string> includePaths { get; set; }
		public string precompiledHeaderName { get; set; }
		public string configType { get; set; }
	}
}
