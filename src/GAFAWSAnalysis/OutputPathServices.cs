// --------------------------------------------------------------------------------------------
// Copyright (C) 2003-2013 SIL International. All rights reserved.
//
// Distributable under the terms of the MIT License, as specified in the license.rtf file.
// --------------------------------------------------------------------------------------------
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
				RemoveFileFromUrl(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase)),
				xslFile);
		}

		public static string RemoveFileFromUrl(string pathname)
		{
			if (String.IsNullOrEmpty(pathname))
				throw new ArgumentException("Input is null or an empty string.", "pathname");

			var match = pathname.StartsWith(@"file:///") ? @"file:///" : @"file:/";
			return pathname.Replace(match, Environment.OSVersion.Platform == PlatformID.Unix ? "/" : null);
		}
	}
}