// --------------------------------------------------------------------------------------------
// Copyright (C) 2003-2013 SIL International. All rights reserved.
//
// Distributable under the terms of the MIT License, as specified in the license.rtf file.
// --------------------------------------------------------------------------------------------
using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using SIL.WordWorks.GAFAWS.PositionAnalysis;

namespace SIL.WordWorks.GAFAWS.PositionAnalyser
{
	/// <summary>
	/// Test the OutputPathServices class.
	/// </summary>
	[TestFixture]
	public class OutputPathServicesTests
	{
		[Test]
		public void NullArgumentThrows()
		{
			Assert.Throws<ArgumentException>(() => OutputPathServices.GetOutputPathname(null));
		}

		[Test]
		public void EmptyStringArgumentThrows()
		{
			Assert.Throws<ArgumentException>(() => OutputPathServices.GetOutputPathname(""));
		}

		[Test]
		public void NonExistingFileThrows()
		{
			Assert.Throws<FileNotFoundException>(() => OutputPathServices.GetOutputPathname("Bogus.txt"));
		}

		[Test]
		public void GoodFileStartsWithOUT()
		{
			var srcPathname = OutputPathServices.RemoveFileFromUrl(Assembly.GetExecutingAssembly().CodeBase);
			var outputPathname = OutputPathServices.GetOutputPathname(srcPathname);

			var srcFilename = Path.GetFileName(srcPathname);
			var outFilename = Path.GetFileName(outputPathname);
			Assert.IsTrue("OUT" + srcFilename == outFilename);
		}
	}
}