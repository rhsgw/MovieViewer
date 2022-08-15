using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieViewer
{
	static class Model
	{
		public static bool TryGetMovie(out Uri? uri)
		{
			var file = Environment.GetCommandLineArgs().FirstOrDefault(x => x.EndsWith(".mp4"));
			uri = File.Exists(file) ? new Uri(Path.GetFullPath(file)) : null;
			return uri != null;
		}
	}
}
