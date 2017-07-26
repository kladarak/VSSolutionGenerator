using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace VSSolutionGenerator
{
	class ProjectSourceFiles
	{
		public List<string> mIncludeFiles = new List<string>();
		public List<string> mCompileFiles = new List<string>();
		public List<string> mFilters = new List<string>();

		public string mSearchDirectory;
		private string mRootFiltersName;

		public ProjectSourceFiles(string inSearchDirectory)
		{
			mSearchDirectory = Path.GetFullPath(inSearchDirectory);

			mRootFiltersName = "Source";
			mFilters.Add(mRootFiltersName);

			AddIncludeFilesWithExt(".h");
			AddIncludeFilesWithExt(".hpp");
			AddIncludeFilesWithExt(".inl");
			AddCompileFilesWithExt(".cpp");
			AddCompileFilesWithExt(".c");

			mIncludeFiles = RemoveSearchDirectoryPath(mIncludeFiles);
			mCompileFiles = RemoveSearchDirectoryPath(mCompileFiles);

			ExtractFilters(mIncludeFiles);
			ExtractFilters(mCompileFiles);
		}

		private void AddIncludeFilesWithExt(string inExtension)
		{
			mIncludeFiles.AddRange(GetFilesWithExt(inExtension));
		}

		private void AddCompileFilesWithExt(string inExtension)
		{
			mCompileFiles.AddRange(GetFilesWithExt(inExtension));
		}

		private string[] GetFilesWithExt(string inExtension)
		{
			return Directory.GetFiles(mSearchDirectory, "*" + inExtension, SearchOption.AllDirectories);
		}

		private List<string> RemoveSearchDirectoryPath(List<string> inFilenames)
		{
			List<string> newList = new List<string>();

			foreach (var filename in inFilenames)
			{
				newList.Add(filename.Replace(mSearchDirectory, ""));
			}

			return newList;
		}

		private void ExtractFilters(List<string> inFilenames)
		{
			foreach (var filename in inFilenames)
			{
				string filter = GetFilter(filename);	
				if (!mFilters.Contains(filter))
				{
					mFilters.Add(filter);
				}
			}
		}

		public string GetFilter(string inFilename)
		{
			int lastIndex = inFilename.LastIndexOf("\\");
			if (lastIndex >= 0)
			{
				return mRootFiltersName + "\\" + inFilename.Substring(0, lastIndex);	
			}
			else
			{
				return mRootFiltersName;
			}
		}
	}
}
