using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAccess;
using EPiServer.Globalization;
using EPiServer.Security;
using Fellow.Epi.Localization.Infrastructure.Editor.Content.Definition;
using Fellow.Epi.Localization.Manager.Translation;
using Fellow.Epi.Localization.Manager.Translation.Entities;
using Fellow.Epi.Localization.Models.Content;

namespace Fellow.Epi.Localization.Infrastructure.Editor.Content
{
	class ExternalTranslationContentProvider : ContentProvider
	{
		private readonly ITranslationManager _translationManager;
		private readonly IdentityMappingService _identityMappingService;
		private readonly IContentRepository _contentRepository;
		private readonly ILanguageBranchRepository _languageBranchRepository;

		public ExternalTranslationContentProvider(IContentRepository contentRepository, IdentityMappingService identityMappingService, ITranslationManager translationManager, ILanguageBranchRepository languageBranchRepository)
		{
			this._contentRepository = contentRepository;
			this._identityMappingService = identityMappingService;
			this._translationManager = translationManager;
			this._languageBranchRepository = languageBranchRepository;
		}

		public ContentReference GetEntryPoint(string name)
		{
			var folder = this._contentRepository.GetBySegment(ContentReference.RootPage, name, LanguageSelector.AutoDetect()) as ContentFolder;
			
			if (folder == null)
			{
				//if no existing folder, create one
				folder = this._contentRepository.GetDefault<ContentFolder>(ContentReference.RootPage);
				folder.Name = name;

				this._contentRepository.Save(folder, SaveAction.Publish, AccessLevel.NoAccess);
			}
			
			return folder.ContentLink;
		}

		private static string RemoveEndingSlash(string virtualPath)
		{
			return virtualPath == null ? null : virtualPath.TrimEnd('/');
		}

		protected override void SetCacheSettings(ContentReference contentReference, IEnumerable<GetChildrenReferenceResult> children, CacheSettings cacheSettings)
		{
			cacheSettings.AbsoluteExpiration = DateTime.Now.AddHours(12);
			cacheSettings.CacheKeys.Add(ContentLanguage.PreferredCulture.Name);
			base.SetCacheSettings(contentReference, children, cacheSettings);
		}

		protected override void SetCacheSettings(ContentReference parentLink, string urlSegment, IEnumerable<MatchingSegmentResult> childrenMatches,
			CacheSettings cacheSettings)
		{
			cacheSettings.AbsoluteExpiration = DateTime.Now.AddHours(12);
			cacheSettings.CacheKeys.Add(ContentLanguage.PreferredCulture.Name);
			base.SetCacheSettings(parentLink, urlSegment, childrenMatches, cacheSettings);
		}

		protected override void SetCacheSettings(IContent content, CacheSettings cacheSettings)
		{
			cacheSettings.AbsoluteExpiration = DateTime.Now.AddHours(12);
			cacheSettings.CacheKeys.Add(ContentLanguage.PreferredCulture.Name);
			base.SetCacheSettings(content, cacheSettings);
		}


		private T CreateContentAndAssignIdentity<T>(MappedIdentity mappedIdentity, string name, CultureInfo culture) where T : IContent
		{
			// find parent 
			ContentReference parentLink;
			if (typeof(T) != typeof(ContentFolder))
			{
				var parentUri = MappedIdentity.ConstructExternalIdentifier(ProviderKey, RemoveEndingSlash(mappedIdentity.ExternalIdentifier.Segments[1]));
				var parentElement = this._identityMappingService.Get(parentUri, true);
				parentLink = parentElement.ContentLink;
			}
			else
			{
				parentLink = EntryPoint;
			}
			
			// initialize content properly
			var content = this._contentRepository.GetDefault<T>(parentLink, culture);
			content.ContentGuid = mappedIdentity.ContentGuid;
			content.ContentLink = mappedIdentity.ContentLink;
			content.Name = HttpUtility.UrlDecode(name);

			var versionable = content as IVersionable;
			if (versionable != null)
			{
				versionable.Status = VersionStatus.Published;
			}
			var changeTrackable = content as IChangeTrackable;
			if (changeTrackable != null)
			{
				changeTrackable.Changed = DateTime.Now;
			}
			return content;
		}

		protected override IContent LoadContent(ContentReference contentLink, ILanguageSelector languageSelector)
		{
			MappedIdentity mappedIdentity = this._identityMappingService.Get(contentLink);

			if (mappedIdentity == null)
				return null;

			// figure out if we are on a content folder
			if (mappedIdentity.ExternalIdentifier.Segments.Length == 2)
			{
				// can't use contentrepository as we'll get deadlock victim error
				var contentFolder = CreateContentAndAssignIdentity<ContentFolder>(mappedIdentity, mappedIdentity.ExternalIdentifier.Segments[1], ContentLanguage.PreferredCulture);
				// make the content folder read only
				contentFolder.MakeReadOnly();
				return contentFolder;
			}

			CultureInfo culture = ContentLanguage.Instance.FinalFallbackCulture;

			if (!languageSelector.Language.Equals(CultureInfo.InvariantCulture))
				culture = languageSelector.Language;


			if (mappedIdentity.ExternalIdentifier.Segments.Length != 3)
				return null;

			string translationKeyString = HttpUtility.UrlDecode(mappedIdentity.ExternalIdentifier.Segments[2]);
		
			ITranslation translation;

			bool found = this._translationManager.TryGet(translationKeyString, ContentLanguage.PreferredCulture, out translation);

			if (!found)
				return null;
			
			// finally create the content, put in it cache and return it
			DefaultTranslationType content = CreateContentAndAssignIdentity<DefaultTranslationType>(mappedIdentity, translation.Name, ContentLanguage.PreferredCulture);
			
			// need to set LinkURL this peculiar way (not being a PropertyUrl and not accessing the LinkURL property) 
			// because otherwise stack overflow happens when caching is disabled (pageCacheSlidingExpiration=0)
			content.Property["PageLinkURL"] = new PropertyString
			{
				Value = ConstructContentUri(content.ContentTypeID, mappedIdentity.ContentLink, mappedIdentity.ContentGuid).ToString(),
				OwnerTab = -1,
				IsLanguageSpecific = true
			};
			content.Translation = translation.TranslationText;
			content.Key = translation.Key;
			content.Language = culture;
			content.ExistingLanguages = this._languageBranchRepository.ListEnabled().Select(languageBranch => languageBranch.Culture);
			content.MasterLanguage = ContentLanguage.Instance.FinalFallbackCulture;

			// make the content read only
			content.MakeReadOnly();

			return content;
		}

		protected override IList<GetChildrenReferenceResult> LoadChildrenReferencesAndTypes(ContentReference contentLink, string languageId, out bool languageSpecific)
		{
			languageSpecific = true;

			IEnumerable<ITranslation> articles = this._translationManager
				.GetAll(ContentLanguage.PreferredCulture);
			
			// create and return GetChildrenReferenceResults; the ContentReference (ContentLink) is fetched using the IdentityMappingService
			var childReferences = new List<GetChildrenReferenceResult>();

			if (EntryPoint.CompareToIgnoreWorkID(contentLink))
			{
				// create prefixed alphabetical content folder structure

				var caseSensitiveComparer = StringComparer.Create(CultureInfo.InvariantCulture, true);

				childReferences.AddRange(articles.
					Select(translation => GetFolderName(translation)).
					Distinct(caseSensitiveComparer).
					OrderBy(name => name, caseSensitiveComparer).
					Select(name => MappedIdentity.ConstructExternalIdentifier(ProviderKey, name)).
					Select(folderUri => this._identityMappingService.Get(folderUri, true)).
					Select(mappedIdentity => mappedIdentity.ContentLink).
					Select(folderContentReference => new GetChildrenReferenceResult
					{
						ContentLink = folderContentReference,
						IsLeafNode = false,
						ModelType = typeof(ContentFolder)
					}));
				return childReferences;
			}

			// create recipes for each sub folder
			var mappedItem = this._identityMappingService.Get(contentLink);

			string folderName = mappedItem.ExternalIdentifier.Segments[1];

			IEnumerable<ITranslation> transationsForFolder = articles.Where(video => String.Equals(this.GetFolderName(video), folderName, StringComparison.InvariantCultureIgnoreCase));

			childReferences.AddRange(transationsForFolder.
				Select(translation => CreateContentLink(translation)).
				Select(contentReference => new GetChildrenReferenceResult
				{
					ContentLink = contentReference,
					IsLeafNode = true,
					ModelType = typeof(DefaultTranslationType)
				}));

			return childReferences;
		}

		public ContentReference CreateContentLink(ITranslation translation)
		{
			var providerUniquePath = string.Format(CultureInfo.InvariantCulture, "{0}/{1}", GetFolderName(translation), HttpUtility.UrlEncode(translation.Key));
			var uri = MappedIdentity.ConstructExternalIdentifier(TranslationDataRepositoryDescriptor.RepositoryKey, providerUniquePath);
			var mappedIdentity = this._identityMappingService.Get(uri, true);
			return mappedIdentity.ContentLink;
		}

		private string GetFolderName(ITranslation translation)
		{
			return translation.Area;
		}

		public override ContentReference Save(IContent content, SaveAction action)
		{
			// we have a new version. Either the attendee has been autosaved, or it has been published or a new one has been created   
			var translation = content as DefaultTranslationType;

			if(translation == null)
				throw new ArgumentException("Content was not of type DefaultTranslationType", "content");

			this._translationManager.Update(translation.Translation, translation.Key, ContentLanguage.PreferredCulture);

			return translation.ContentLink;

		}
	}
}
