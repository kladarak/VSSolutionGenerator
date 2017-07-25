using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace VSSolutionGenerator
{
	class SolutionExporter
	{
		StreamWriter mFile;

		public bool Export(SolutionData inData)
		{
			mFile = new StreamWriter(inData.mSolutionFilename + ".sln");

			WriteLine("");
			WriteLine("Microsoft Visual Studio Solution File, Format Version 12.00");
			WriteLine("# Visual Studio 14");
			WriteLine("VisualStudioVersion = 14.0.25420.1");
			WriteLine("MinimumVisualStudioVersion = 10.0.40219.1");

			foreach (var project in inData.mProjects)
			{
				string format = "Project(\"{0}\") = \"{1}\", \"{2}\", \"{3}\"";
				WriteLine(String.Format(format, inData.mSolutionGuid, project.mProjectName, project.mProjectFilename, project.mProjectUID));

				if (project.mProjectDependencies.Count > 0)
				{
					WriteLine("\tProjectSection(ProjectDependencies) = postProject");

					foreach (var dep in project.mProjectDependencies)
					{
						WriteLine(String.Format("\t\t{0} = {0}", dep));
					}

					WriteLine("\tEndProjectSection");
				}

				WriteLine("EndProject");
			}

			WriteLine("Global");

			WriteLine("\tGlobalSection(SolutionConfigurationPlatforms) = preSolution");

			foreach (var config in inData.mSolutionConfigs)
			{
				String entry = String.Format("{0}|{1}", config.mName, config.mArchitecture.ToString());
				WriteLine(String.Format("\t\t{0} = {0}", entry));
			}

			WriteLine("\tEndGlobalSection");

			WriteLine("\tGlobalSection(ProjectConfigurationPlatforms) = postSolution");

			foreach (var project in inData.mProjects)
			{
				foreach (var config in inData.mSolutionConfigs)
				{
					string format = "\t\t{0}.{1}|{2}.{3} = {4}|{5}";

					WriteLine(String.Format(format, 
								project.mProjectUID,
								config.mName, config.mArchitecture.ToString(),
								"ActiveCfg",
								config.mBuildType.ToString(), config.mPlatform.ToString()));

					WriteLine(String.Format(format, 
								project.mProjectUID,
								config.mName, config.mArchitecture.ToString(),
								"Build.0",
								config.mBuildType.ToString(), config.mPlatform.ToString()));
				}
			}

			WriteLine("\tEndGlobalSection");

			WriteLine("\tGlobalSection(SolutionProperties) = preSolution");
			WriteLine("\t\tHideSolutionNode = FALSE");
			WriteLine("\tEndGlobalSection");

			WriteLine("EndGlobal");

			mFile.Close();

			return true;
		}

		private void WriteLine(string inLine)
		{
			mFile.WriteLine(inLine);
		}
	}
}
