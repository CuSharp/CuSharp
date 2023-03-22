dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura 
reportgenerator -reports:"coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html