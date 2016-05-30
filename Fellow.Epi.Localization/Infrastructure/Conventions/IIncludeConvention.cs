using Fellow.Epi.Localization.Manager.Convention;

namespace Fellow.Epi.Localization.Infrastructure.Conventions
{
	public interface IIncludeConvention
	{
		void Apply(IConventionManager conventionManager);
	}
}
