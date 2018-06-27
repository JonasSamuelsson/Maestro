Use the following commands to pack & push nuget packages

```
dotnet pack -c release --include-symbols {project-dir}
dotnet nuget push -k {api-key} -s https://nuget.org {nupkg}
dotnet nuget push -k {api-key} -s https://nuget.smbsrc.net {symbols-nupkg}
```