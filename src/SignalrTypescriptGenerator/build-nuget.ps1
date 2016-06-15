$version = Read-Host "What is the version number?"

nuget pack .\SignalrTypescriptGenerator.nuspec -Prop Configuration=Release -Version $version
