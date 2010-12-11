using SIL.WordWorks.GAFAWS.PositionAnalysis;
using StructureMap.Configuration.DSL;

namespace SIL.WordWorks.GAFAWS.AffixPositionAnalyzer
{
	/// <summary>
	/// Register various classes.
	/// </summary>
	public class AppRegistry : Registry
	{
		public AppRegistry()
		{
			// Main app form class.
			For<AffixPositionAnalyzer>()
				.Singleton()
				.Use<AffixPositionAnalyzer>();

			Scan(scanner =>
				{
					scanner.AssembliesFromApplicationBaseDirectory();
					// Register converters that are in any assembly.
					scanner.AddAllTypesOf<IGafawsConverter>();

					// Load up any registries in any assembly.
					// The main Gafaws assembly has one that deals with all basic stuff.
					scanner.LookForRegistries();
				});

		}
	}
}