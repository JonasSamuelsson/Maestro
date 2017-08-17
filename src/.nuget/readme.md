Use the following commands to pack & push nuget packages

```
dotnet pack -c release --include-symbols
dotnet nuget push .\bin\release\{project}.nupkg -k {api-key} -s https://nuget.org
dotnet nuget push .\bin\release\{project}.symbols.nupkg -k {api-key} -s https://nuget.smbsrc.net
```