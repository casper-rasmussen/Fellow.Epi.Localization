using System;
using System.Data;
using EPiServer.Data;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;

namespace Fellow.Epi.Localization.Infrastructure.Initialization
{
	[InitializableModule]
	[ModuleDependency(typeof(EPiServer.Data.DataInitialization))]
	public class CreateDatabaseForCustomDataStoreInitialization : IInitializableModule
	{
		public const string TableNameTranslation = "Localization_Translations";

		public void Initialize(InitializationEngine context)
		{
			IDatabaseHandler databaseHandler = context.Locate.Advanced.GetInstance<IDatabaseHandler>();

			databaseHandler.Execute(() =>
			{
				//Statement creatin Dynamic Data Store compatible table if it doesnt exist.
				string command = String.Format(@"IF  NOT EXISTS (SELECT * FROM sys.tables 
								WHERE object_id = OBJECT_ID(N'[dbo].[{0}]'))

								BEGIN

								CREATE TABLE [dbo].[{0}]
								(
								  [pkId] bigint not null,
								  [Row] int not null CONSTRAINT [DF_{0}_Row] DEFAULT(1) 
									  CONSTRAINT CH_{0} check ([Row]>=1),
								  [StoreName] nvarchar(375) not null,
								  [ItemType] nvarchar(2000) not null,
								  [TranslationKey] nvarchar(100) not null,
								  [Translation] nvarchar(max) not null,
								  [CultureCode] nvarchar(5) not null, 
								  CONSTRAINT [PK_{0}] primary key clustered
								  (
									[pkId]
								  ),
							      CONSTRAINT [UC_{0}] UNIQUE 
								  (
									[TranslationKey],
									[CultureCode]
								  ),
								  CONSTRAINT [FK_{0}_tblBigTableIdentity] foreign key
								  (
									[pkId]
								  )
								  REFERENCES [tblBigTableIdentity]
								  (
									[pkId]
								  )
								)
								CREATE UNIQUE INDEX [INDEX_{0}_Key] ON {0} ([TranslationKey], [CultureCode])
								END", CreateDatabaseForCustomDataStoreInitialization.TableNameTranslation);

				IDbCommand dbCommand = databaseHandler.CreateCommand();
				dbCommand.CommandType = CommandType.Text;
				dbCommand.CommandText = command;

				dbCommand.ExecuteNonQuery();
				
			});
		}

		public void Preload(string[] parameters) { }

		public void Uninitialize(InitializationEngine context)
		{
			//Add uninitialization logic
		}
	}
}
