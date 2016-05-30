using EPiServer.Shell;
using EPiServer.Shell.ViewComposition;

namespace Fellow.Epi.Localization.Infrastructure.Editor.Content.Definition
{
	[Component()]
	public class TranslationDataPaneNavigationComponent : ComponentDefinitionBase
	{
		public TranslationDataPaneNavigationComponent()
			: base("epi-cms.widget.HierarchicalList")
        {
            Categories = new[] { "content" };
            Title = "Translations";
            SortOrder = 1000;
			PlugInAreas = new[] { PlugInArea.AssetsDefaultGroup };
            Settings.Add(new Setting("repositoryKey", TranslationDataRepositoryDescriptor.RepositoryKey));

        }
	}
}
