using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.DataAbstraction;
using Fellow.Epi.Localization.Repository.Resource.Entity;
using Fellow.Epi.Localization.Repository.Resource.Exception;

namespace Fellow.Epi.Localization.Repository.Resource
{
	class CategoryResourceRepository : IResourceRepository
	{
		private const string ResourcePrefix = "/core/category/";

		private readonly CategoryRepository _categoryRepository;

		public CategoryResourceRepository(CategoryRepository categoryRepository)
		{
			this._categoryRepository = categoryRepository;
		}

		public IResource Get(string key, System.Globalization.CultureInfo culture)
		{
			string name = key.Replace(ResourcePrefix, String.Empty);

			Category category = this._categoryRepository.Get(name);

			if(category == null)
				throw new ResourceNotFoundException(key);

			IResource found = this.GetResourceForCategory(category);

			if (found == null)
				throw new ResourceNotFoundException(key);

			return found;
		}

		public IEnumerable<IResource> GetAll(System.Globalization.CultureInfo culture)
		{
			Category root = this._categoryRepository.GetRoot();

			return this.RecursiveResources(root)
				.Select(category => GetResourceForCategory(category))
				.ToList();
		}

		private IEnumerable<Category> RecursiveResources(Category category)
		{
			// Return the parent before its children
			if(!category.Equals(this._categoryRepository.GetRoot()))
				yield return category;

			foreach (Category node in category.Categories)
			{
				foreach (Category subNode in RecursiveResources(node))
				{
					yield return subNode;
				}
			}
		}

		private IResource GetResourceForCategory(Category category)
		{
			return new Entity.Resource()
			{
				DefaultValue = category.LocalizedDescription, 
				Key = String.Format("{0}{1}", ResourcePrefix, category.Name)
			};
		}

		public bool Exists(string key, System.Globalization.CultureInfo culture)
		{
			string name = key.Replace(ResourcePrefix, String.Empty);

			Category category = this._categoryRepository.Get(name);

			return category != null;
		}
	}
}
