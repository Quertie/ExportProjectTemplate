using System.Collections.Generic;
using DotNetProjectParser;

namespace ExportProjectTemplate
{
	public class Folder
	{
		public string Name { get; set; }
		public List<Folder> Folders { get; set; } = new List<Folder>();
		public List<ProjectItem> Files { get; set; } = new List<ProjectItem>();
	}
}