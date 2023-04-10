REM ******************************************************************
REM PERCENTAGES OF COVERAGE REPORT ARE NOT CORRECT
REM Reason: Several reports with different arguments are merged.
REM Number of lines and branches are therefore counted several times.
REM ******************************************************************

dotnet test --filter "TestCategory=Integration" /p:CollectCoverage=true /p:Configuration=Release /p:CoverletOutput=./
dotnet test --filter "TestCategory=Unit" /p:CollectCoverage=true /p:Configuration=Release /p:CoverletOutput=./ /p:MergeWith="./coverage.json"
dotnet test --filter "TestCategory=UnitReleaseOnly" /p:CollectCoverage=true /p:Configuration=Release /p:CoverletOutput=./ /p:MergeWith="./coverage.json"
dotnet test --filter "TestCategory=UnitDebugOnly" /p:CollectCoverage=true /p:Configuration=Debug /p:CoverletOutput=./ /p:MergeWith="./coverage.json" /p:CoverletOutputFormat=\"cobertura\"
reportgenerator -reports:"coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html

REM ******************************************************************
REM PERCENTAGES OF COVERAGE REPORT ARE NOT CORRECT
REM Reason: Several reports with different arguments are merged.
REM Number of lines and branches are therefore counted several times.
REM ******************************************************************
