Fellow.Epi.Localization

Installation
============

Thank you for installing Fellow.Epi.Localization. 

Fellow.Epi.Localization requires registration of a LocalizationProvider via changes to <episerver.framework/>. Web.config transformation was executed as part of install. Please remember to correct the virtualPath-attribute with your the Resource path required by your project.

<add name="translations" virtualPath="{resource-path}" type="Fellow.Epi.Localization.Infrastructure.Providers.TranslationLocalizationProvider, Fellow.Epi.Localization" />

Have fun.

See more Episerver topics on http://fellow.aagaardrasmussen.dk


