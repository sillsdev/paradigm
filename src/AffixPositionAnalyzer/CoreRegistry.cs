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
				.Use(x => new PositionAnalyzer());
			For<IGafawsData>()
				.AlwaysUnique()
				.Use(x => new GafawsData());
		}
	}
}