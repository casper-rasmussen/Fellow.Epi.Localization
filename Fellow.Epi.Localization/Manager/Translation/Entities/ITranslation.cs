namespace Fellow.Epi.Localization.Manager.Translation.Entities
{
	public interface ITranslation
	{
		string Key { get; }

		string Name { get; }

		string TranslationText { get; }

		string Area { get; }
	}
}
