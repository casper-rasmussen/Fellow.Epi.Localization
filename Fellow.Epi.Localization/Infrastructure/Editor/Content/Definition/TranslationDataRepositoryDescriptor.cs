using System;
using System.Collections.Generic;
using EPiServer.Core;
using EPiServer.Shell;
using Fellow.Epi.Localization.Models.Content;

namespace Fellow.Epi.Localization.Infrastructure.Editor.Content.Definition
{
	class TranslationDataRepositoryDescriptor : ContentRepositoryDescriptorBase
	{
		private readonly IContentProviderManager _contentProviderManager;

		public TranslationDataRepositoryDescriptor(IContentProviderManager contentProviderManager)
		{
			this._contentProviderManager = contentProviderManager;
		}

		public override IEnumerable<Type> CreatableTypes
		{
			get
			{
				return new[]
				{
					typeof (DefaultTranslationType)

				};
			}
		}

		public override IEnumerable<Type> ContainedTypes
		{
			get
			{
				return new[]
                {
					typeof(ContentFolder),
                    typeof(DefaultTranslationType)
                };
			}
		}

		public override IEnumerable<Type> MainNavigationTypes
		{
			get { 

				return new[]
				{
					typeof(ContentFolder)
				}; 
			}
		}
		
		public const string RepositoryKey = "translations";

		public override string Key
		{
			get { return RepositoryKey; }
		}

		public override string Name
		{
			get { return "Translations"; }
		}

		public override IEnumerable<ContentReference> Roots
		{
			get 
			{ 
				return new List<ContentReference>()
				{
					this._contentProviderManager.GetProvider(RepositoryKey).EntryPoint
				}; 
			}
		}
	}
}
