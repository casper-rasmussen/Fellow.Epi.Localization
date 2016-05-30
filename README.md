# Fellow.Epi.Localization: Translations as part of the Episerver UI

This translation add-on provides Content Editors with an intuitive way of working with pre-defined translatable elements through a dedicated Translations gadget.

## Installation and Usage

You can get the latest version of Fellow.Epi.Localization through [Episervers NuGet Feed](http://nuget.episerver.com/en/OtherPages/Package/?packageId=Fellow.Epi.Localization).
Be aware that Fellow.Epi.Localization requires EPiServer.CMS.Core version 9.0.0.0 or higher.

Please use this GitHub project for any issues, questions or other kinds of feedback.

## Configuration

Fellow.Epi.Localization requires two minor code adjustments in order to be fully integrated to your Episerver solution.

### Configuration changes

Please ensure that Episerver is aware of the translation add-on by adding the custom localization provider through the Episerver.Framework configuration section. Even though we transform configuration files during installation of the NuGet package, some manual configuration changes may be needed in case your solution e.g. includes the Episerver.Framework configuration section via a separate file.

```
<add name="translations" virtualPath="~/Your-Path-Goes-Here" type="Fellow.Epi.Localization.Infrastructure.Providers.TranslationLocalizationProvider, Fellow.Epi.Localization" />
```

### Changing default conventions

Adjusting the rule detailing which translatable fields to include is needed in order to use any customized structure in your XML Resource Files. It is a matter of implementing an IIncludeConvention and telling Fellow.Epi.Localization about this through Structuremap.

<script src="https://gist.github.com/casper-rasmussen/0d1c1b3ad5f61f610454bedddb751a66.js"></script>

<script src="https://gist.github.com/casper-rasmussen/0faf40eb84907131c7316808818e055a.js"></script>

Above implementation ensures that all resource elements below &lt;/header&gt;, &lt;/commerce&gt; and &lt;/form&gt; are included in the <em>Translations</em> gadget. Below is an example on a Resource File matching these conventions.

<script src="https://gist.github.com/casper-rasmussen/e1d19cc02b672bb48586f8b616c78981.js"></script>

