using EPiServer.Data;
using EPiServer.Data.Dynamic;

namespace Fellow.Epi.Localization.Repository.Translation.Storage
{
	[EPiServerDataStore(AutomaticallyCreateStore = true, AutomaticallyRemapStore = true, StoreName = "Localization_Translation")]
	internal class TranslationDDSEntity : IDynamicData
	{
		public Identity Id { get; set; }

		[EPiServer.Data.Dynamic.EPiServerDataColumn(ColumnName = "Translation")]
		public string Translation { get; set; }

		[EPiServer.Data.Dynamic.EPiServerDataColumn(ColumnName = "CultureCode")]
		public string CultureCode { get; set; }

		[EPiServer.Data.Dynamic.EPiServerDataColumn(ColumnName = "TranslationKey")]
		public string Key { get; set; }
	}
}
