using System.Collections.Generic;
using System.Linq;
using EPiServer.Cms.Shell.Search;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Framework.Localization;
using EPiServer.Globalization;
using EPiServer.Shell.Search;
using EPiServer.Web;
using EPiServer.Web.Routing;
using Fellow.Epi.Localization.Infrastructure.Editor.Content.Definition;
using Fellow.Epi.Localization.Manager.Translation;
using Fellow.Epi.Localization.Manager.Translation.Entities;
using Fellow.Epi.Localization.Models.Content;

namespace Fellow.Epi.Localization.Infrastructure.Editor.Content
{
	[SearchProvider]
	public class ExternalTranslationContentSearchProvider : ContentSearchProviderBase<DefaultTranslationType, ContentType>
	{
		private readonly ITranslationManager _translationManager;
		private readonly ExternalTranslationContentProvider _translationProvider;

		public ExternalTranslationContentSearchProvider(LocalizationService localizationService, SiteDefinitionResolver siteDefinitionResolver, IContentTypeRepository contentTypeRepository, IContentProviderManager contentProviderManager, EditUrlResolver editUrlResolver, ITranslationManager translationManager)
			: base(localizationService, siteDefinitionResolver, contentTypeRepository, editUrlResolver)
		{
			this._translationManager = translationManager;
			this._translationProvider = contentProviderManager.GetProvider(TranslationDataRepositoryDescriptor.RepositoryKey) as ExternalTranslationContentProvider;

		}

		public override IEnumerable<SearchResult> Search(Query query)
		{
			var searchResults = new List<SearchResult>();
			
			IEnumerable<ITranslation> translations = this._translationManager.GetAll(ContentLanguage.PreferredCulture);

			//We are searching in the Title and Translation
			translations = translations
				.Where(t => t.Name.Contains(query.SearchQuery) || t.TranslationText.Contains(query.SearchQuery))
				.ToList();

			if(translations.Count() > query.MaxResults)
				translations = translations.Take(query.MaxResults);

			foreach(ITranslation translation in translations) 
			{
				//Convert to a Content Reference
				ContentReference reference = this._translationProvider.CreateContentLink(translation);

				//Get the a search result for our content
				SearchResult content = CreateSearchResult(this._translationProvider.Load(reference, LanguageSelector.AutoDetect()) as DefaultTranslationType);

				searchResults.Add(content);
			}

			return searchResults;
		}

		public override string Area { get { return this._translationProvider.ProviderKey; } }

		public override string Category { get { return this._translationProvider.Name; } }

		protected override string IconCssClass { get { return "epi-resourceIcon epi-resourceIcon-block"; } }
	}
}
