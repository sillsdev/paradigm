using System;
using System.IO;
using System.Reflection;

namespace SIL.WordWorks.GAFAWS.PositionAnalysis
{
	/// <summary>
	/// Provides services for the output.
	/// </summary>
	public static class OutputPathServices
	{
		/// -----------------------------------------------------------------------------------
		/// <summary>
		/// Gets the output name, which is based on the original input name.
		/// </summary>
		/// <param name="inputPathname">The input pathname.</param>
		/// <returns>The output pathname.</returns>
		/// <exception cref="ArgumentException">
		/// Thrown if <paramref name="inputPathname"/> is null or an empty string.
		/// </exception>
		/// <exception cref="FileNotFoundException">
		/// Thrown if <paramref name="inputPathname"/> does not exist.
		/// </exception>
		/// -----------------------------------------------------------------------------------
		public static string GetOutputPathname(string inputPathname)
		{
			if (String.IsNullOrEmpty(inputPathname))
				throw new ArgumentException("Input is null or an empty string.", "inputPathname");
			if (!File.Exists(inputPathname))
				throw new FileNotFoundException("File does not exist.", "inputPathname");

			return Path.Combine(Path.GetDirectoryName(inputPathname),
				"OUT" + Path.GetFileName(inputPathname));
		}

		public static string GetXslPathname(string xslFile)
		{
			return Path.Combine(
				Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase.Replace(@"file:///", null)),
				xslFile);
		}
	}
}