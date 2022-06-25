using benchmark.Benchmarks;
using BenchmarkDotNet.Running;


System.Console.WriteLine("Seeding table");
var repo = new DapperRepositoryBenchmarks();
await repo.ReseedTable();
System.Console.WriteLine("Done seeding, benchmarking");

var summary = BenchmarkRunner.Run<DapperRepositoryBenchmarks>();
