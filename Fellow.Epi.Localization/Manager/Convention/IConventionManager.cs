namespace Fellow.Epi.Localization.Manager.Convention
{
	public interface IConventionManager
	{
		void IncludeArea(string area);

		bool IsIncluded(string resourceKey);
	}
}
