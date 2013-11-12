using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Maestro.Configuration
{
	public static class ConventionExpressionExtensions
	{
		public static IConventionExpression StartupDirectory(this IConventionExpression expression, bool? includeSubDirectories)
		{
			var directory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
			return expression.Directory(directory, includeSubDirectories);
		}

		public static IConventionExpression Directory(this IConventionExpression expression, string directory, bool? includeSubDirectories)
		{
			var assemblies = new List<Assembly>();
			var option = includeSubDirectories.GetValueOrDefault() ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
			const string pattern = @".+\.(dll|exe)^";
			var files = System.IO.Directory.GetFiles(directory, "*.*", option)
				.Where(x => Regex.IsMatch(x, pattern, RegexOptions.IgnoreCase));
			foreach (var file in files)
			{
				Assembly assembly;
				if (!TryLoadAssembly(file, out assembly)) continue;
				assemblies.Add(assembly);
			}
			return expression.Assemblies(assemblies);
		}

		private static bool TryLoadAssembly(string path, out Assembly assembly)
		{
			assembly = null;

			try
			{
				assembly = Assembly.LoadFrom(path);
				return true;
			}
			catch (BadImageFormatException)
			{
				return false;
			}
		}
	}
}