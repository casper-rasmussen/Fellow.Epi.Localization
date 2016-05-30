using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EPiServer.Data.Dynamic;
using EPiServer.Logging.Compatibility;
using Fellow.Epi.Localization.Infrastructure.Initialization;
using Fellow.Epi.Localization.Repository.Translation.Exceptions;
using Fellow.Epi.Localization.Repository.Translation.Storage;

namespace Fellow.Epi.Localization.Repository.Translation
{
	class TranslationRepository : ITranslationRepository
	{
		private readonly ILog _logManager;

		public TranslationRepository()
		{
			this._logManager = LogManager.GetLogger(typeof(TranslationRepository));
		}

		/// <exception cref="TranslationNotFoundException">Thrown when Translation was not found</exception>
		public virtual string Get(string key, CultureInfo culture)
		{
			using (DynamicDataStore store = DynamicDataStoreFactory.Instance.CreateStore(typeof(TranslationDDSEntity), new StoreDefinitionParameters() { TableName = CreateDatabaseForCustomDataStoreInitialization.TableNameTranslation }))
			{
				try
				{
					store.KeepObjectsInContext = true;

					IDictionary<string, object> query = new Dictionary<string, object>();
					query.Add("Key", key);
					query.Add("CultureCode", culture.Name);

					TranslationDDSEntity entity = store.Find<TranslationDDSEntity>(query).FirstOrDefault();

					if(entity == null)
						throw new TranslationNotFoundException(key, culture);

					return entity.Translation;
				}
				catch (StoreInconsistencyException e)
				{
					this._logManager.Error("Could not get TranslationDDSEntity", e);
					return String.Empty;
				}
			}
		}

		public virtual void Remove(string key, CultureInfo culture)
		{	
			using (DynamicDataStore store = DynamicDataStoreFactory.Instance.CreateStore(typeof(TranslationDDSEntity), new StoreDefinitionParameters() { TableName = CreateDatabaseForCustomDataStoreInitialization.TableNameTranslation }))
			{
				try
				{
					store.KeepObjectsInContext = true;

					IDictionary<string, object> query = new Dictionary<string, object>();
					query.Add("Key", key);
					query.Add("CultureCode", culture.Name);

					TranslationDDSEntity entity = store.Find<TranslationDDSEntity>(query).FirstOrDefault();

					if (entity == null)
						return; //Does not exist

					store.Delete(entity);
				}
				catch (StoreInconsistencyException e)
				{
					this._logManager.Error("Could not delete TranslationDDSEntity", e);
					return;
				}
			}
			
		}

		public virtual void Add(string key, string translation, CultureInfo culture)
		{
			using (DynamicDataStore store = DynamicDataStoreFactory.Instance.CreateStore(typeof(TranslationDDSEntity), new StoreDefinitionParameters() { TableName = CreateDatabaseForCustomDataStoreInitialization.TableNameTranslation }))
			{
				try
				{
					store.KeepObjectsInContext = true;

					IDictionary<string, object> query = new Dictionary<string, object>();
					query.Add("Key", key);
					query.Add("CultureCode", culture.Name);

					TranslationDDSEntity existing = store.Find<TranslationDDSEntity>(query).FirstOrDefault();

					if (existing != null)
						store.Delete(existing);
					
					TranslationDDSEntity entity = new TranslationDDSEntity();
					entity.Translation = translation;
					entity.Key = key;
					entity.CultureCode = culture.Name;

					store.Save(entity);
				}
				catch (StoreInconsistencyException e)
				{
					this._logManager.Error("Could not save TranslationDDSEntity", e);
					return;
				}
			}
		}
	}
}
