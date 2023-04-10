# ******************************************************************
# PERCENTAGES OF COVERAGE REPORT ARE NOT CORRECT
# Reason: Several reports with different arguments are merged.
# Number of lines and branches are therefore counted several times.
# ******************************************************************

dotnet test --filter "TestCategory=Integration" /p:CollectCoverage=true /p:Configuration=Release /p:CoverletOutput=./
dotnet test --filter "TestCategory=Unit" /p:CollectCoverage=true /p:Configuration=Release /p:CoverletOutput=./ /p:MergeWith="./coverage.json"
dotnet test --filter "TestCategory=UnitReleaseOnly" /p:CollectCoverage=true /p:Configuration=Release /p:CoverletOutput=./ /p:MergeWith="./coverage.json"
dotnet test --filter "TestCategory=UnitDebugOnly" /p:CollectCoverage=true /p:Configuration=Debug /p:CoverletOutput=./ /p:MergeWith="./coverage.json" /p:CoverletOutputFormat=\"cobertura\"
reportgenerator -reports:"coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html

# ******************************************************************
# PERCENTAGES OF COVERAGE REPORT ARE NOT CORRECT
# Reason: Several reports with different arguments are merged.
# Number of lines and branches are therefore counted several times.
# ******************************************************************
