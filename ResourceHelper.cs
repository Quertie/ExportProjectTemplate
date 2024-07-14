namespace ExportProjectTemplate
{
	internal class ResourceHelper
	{
		public static bool FileIsNotResource(string filename)
		{
			if (filename.EndsWith(".ico")) return false;
			if (filename.EndsWith(".png")) return false;
			return true;
		}
	}
}