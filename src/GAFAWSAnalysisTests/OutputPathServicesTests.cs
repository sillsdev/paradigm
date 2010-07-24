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
		[Test, ExpectedException(typeof(ArgumentException))]
		public void NullArgumentThrows()
		{
			OutputPathServices.GetOutputPathname(null);
		}

		[Test, ExpectedException(typeof(ArgumentException))]
		public void EmptyStringArgumentThrows()
		{
			OutputPathServices.GetOutputPathname("");
		}

		[Test, ExpectedException(typeof(FileNotFoundException))]
		public void NonExistingFileThrows()
		{
			OutputPathServices.GetOutputPathname("Bogus.txt");
		}

		[Test]
		public void GoodFileStartsWithOUT()
		{
			var srcPathname = Assembly.GetExecutingAssembly().CodeBase.Replace(@"file:///", null);
			var outputPathname = OutputPathServices.GetOutputPathname(srcPathname);

			var srcFilename = Path.GetFileName(srcPathname);
			var outFilename = Path.GetFileName(outputPathname);
			Assert.IsTrue("OUT" + srcFilename == outFilename);
		}
	}
}