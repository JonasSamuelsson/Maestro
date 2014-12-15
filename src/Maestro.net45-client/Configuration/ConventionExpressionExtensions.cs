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
		/// <summary>
		/// Adds all types from assemblies found in startup directory.
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="includeSubDirectories"></param>
		/// <returns></returns>
		public static IConventionExpression StartupDirectory(this IConventionExpression expression, bool? includeSubDirectories = null)
		{
			var directory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
			return expression.Directory(directory, includeSubDirectories);
		}

		/// <summary>
		/// Adds all types from assemblies found in <paramref name="directory"/>.
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="directory"></param>
		/// <param name="includeSubDirectories"></param>
		/// <returns></returns>
		public static IConventionExpression Directory(this IConventionExpression expression, string directory, bool? includeSubDirectories = null)
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