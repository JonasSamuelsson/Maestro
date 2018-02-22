``` ini

BenchmarkDotNet=v0.10.12, OS=Windows 10 Redstone 3 [1709, Fall Creators Update] (10.0.16299.248)
Intel Core i5-4300U CPU 1.90GHz (Haswell), 1 CPU, 4 logical cores and 2 physical cores
Frequency=2435765 Hz, Resolution=410.5486 ns, Timer=TSC
.NET Core SDK=2.1.4
  [Host]     : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT
  DefaultJob : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT


```
|    Method |     Mean |    Error |   StdDev |
|---------- |---------:|---------:|---------:|
| Transient | 195.6 ns | 1.717 ns | 1.522 ns |
| Singleton | 287.8 ns | 1.716 ns | 1.605 ns |
