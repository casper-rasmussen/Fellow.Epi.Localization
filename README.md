# Fellow.Epi.Localization: Translations as part of the Episerver UI

This translation add-on provides Content Editors with an intuitive way of working with pre-defined translatable elements through a dedicated Translations gadget.

## Installation and Usage

You can get the latest version of Fellow.Epi.Localization through [Episervers NuGet Feed](http://nuget.episerver.com/en/OtherPages/Package/?packageId=Fellow.Epi.Localization).
Be aware that Fellow.Epi.Localization requires EPiServer.CMS.Core version 9.0.0.0 or higher.

Please use this GitHub project for any issues, questions or other kinds of feedback.

## Configuration

Fellow.Epi.Localization requires two minor code adjustments in order to be fully integrated with your Episerver solution.

### Configuration changes

Please ensure that Episerver is aware of the translation add-on by adding the custom localization provider through the Episerver.Framework configuration section. Even though we transform configuration files during installation of the NuGet package, some manual configuration changes may be needed in case your solution e.g. includes the Episerver.Framework configuration section via a separate file.

```
<add name="translations" virtualPath="~/Your-Path-Goes-Here" type="Fellow.Epi.Localization.Infrastructure.Providers.TranslationLocalizationProvider, Fellow.Epi.Localization" />
```

### Changing default conventions

Adjusting the rule detailing which translatable fields to include is needed in order to use any customized structure in your XML Resource Files. It is a matter of implementing an IIncludeConvention and telling Fellow.Epi.Localization about this through Structuremap.

```
class ExampleIncludeConvention : IIncludeConvention
{
	public void Apply(Fellow.Epi.Localization.Manager.Convention.IConventionManager conventionManager)
	{
		conventionManager.IncludeArea("header");
		conventionManager.IncludeArea("commerce");
		conventionManager.IncludeArea("form");
	}
}
```
```
[InitializableModule]
[ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
public class DependencyResolverInitialization : IConfigurableModule
{
    public void ConfigureContainer(ServiceConfigurationContext context)
    {
        context.Container.Configure(ConfigureContainer);

        DependencyResolver.SetResolver(new StructureMapDependencyResolver(context.Container));
    }

    private static void ConfigureContainer(ConfigurationExpression container)
    {
	    //Other implementations goes here

      container.For<IIncludeConvention>().Add<AlloyIncludeConvention>();

    }

    public void Initialize(InitializationEngine context)
    {
    }

    public void Uninitialize(InitializationEngine context)
    {
    }
}
```

Above implementation ensures that all resource elements below &lt;/header&gt;, &lt;/commerce&gt; and &lt;/form&gt; are included in the <em>Translations</em> gadget. Below is an example on a Resource File matching these conventions.

```
<?xml version="1.0" encoding="utf-8" ?>
<languages>
  <language name="Danish" id="da-DK">
    <header>
      <mainnavigation>
        <placeholders>
          <search>Search</search>
        </placeholders>
      </mainnavigation>
    </header>
    <commerce>
      <checkout>
        <step1>
          <labels>
            <address>Address Line</address>
            <postal>Postal Code</postal>
            <!-- Elements removed for the sake of readability-->
          </labels>
        </step1>
      </checkout>
      <cart>
        <inline>
            <!-- Elements removed for the sake of readability -->
          </labels>
        </inline>
        <dedicated>
          <!-- Elements removed for the sake of readability-->
        </dedicated>
      </cart>
    </commerce>
    <form>
        <!-- Elements removed for the sake of readability-->
    </form>
  </language>
</languages>
```

