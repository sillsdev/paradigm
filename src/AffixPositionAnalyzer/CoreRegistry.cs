using SIL.WordWorks.GAFAWS.PositionAnalysis;
using StructureMap.Configuration.DSL;

namespace SIL.WordWorks.GAFAWS.AffixPositionAnalyzer
{
	/// <summary>
	/// Register various classes.
	/// </summary>
	public class CoreRegistry : Registry
	{
		public CoreRegistry()
		{
			Scan(scanner =>
				{
					scanner.AssembliesFromApplicationBaseDirectory();
					scanner.AddAllTypesOf<IGafawsConverter>();
				});
			For<IPositionAnalyzer>()
				.Singleton()
				.Use<PositionAnalyzer>();  // x => new PositionAnalyzer()
			For<IGafawsData>()
				.Singleton()
				.Use<GafawsData>(); // x => new GafawsData()
		}
	}
}