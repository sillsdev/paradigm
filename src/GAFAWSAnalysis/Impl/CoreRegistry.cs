using StructureMap.Configuration.DSL;

namespace SIL.WordWorks.GAFAWS.PositionAnalysis.Impl
{
	public class CoreRegistry : Registry
	{
		public CoreRegistry()
		{
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
