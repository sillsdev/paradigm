using SIL.WordWorks.GAFAWS.PositionAnalysis;
using SIL.WordWorks.GAFAWS.PositionAnalysis.Impl;
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
			For<AffixPositionAnalyzer>()
				.Singleton()
				.Use<AffixPositionAnalyzer>();
			Scan(scanner =>
				{
					scanner.AssembliesFromApplicationBaseDirectory();
					scanner.AddAllTypesOf<IGafawsConverter>();
				});
			For<IPositionAnalyzer>()
				.Singleton()
				.Use<PositionAnalyzer>();
			For<IGafawsData>()
				.Singleton()
				.Use<GafawsData>();

			For<IWordRecordFactory>()
				.AlwaysUnique()
				.Use<WordRecordFactory>();
			For<IAffixFactory>()
				.AlwaysUnique()
				.Use<AffixFactory>();
			For<IStemFactory>()
				.AlwaysUnique()
				.Use<StemFactory>();
			For<IMorphemeFactory>()
				.AlwaysUnique()
				.Use<MorphemeFactory>();
		}
	}
}