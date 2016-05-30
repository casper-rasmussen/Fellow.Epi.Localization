SETLOCAL

set "module=Fellow.Epi.Localization"
set output="../ModulePackages/%module%"

rmdir /s /q %output%
mkdir %output%

nuget pack "Fellow.Epi.Localization\Fellow.Epi.Localization.csproj" -Build -OutputDirectory %output% -Prop Configuration=Release;Platform=AnyCPU