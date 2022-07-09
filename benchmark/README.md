# Benchmark
This is a small project to measure the CPU time consumed by `Dapper.DDD.Repository` CRUD methods compared to a "raw" Dapper implementation.

If you're curious you can run the benchmarks yourself by spinning up the SqlServer pod (either by using podman or docker - I use podman myself)

Here are my results, when run on an AMD Ryzen 3800X with 32GB of memory:

|            Method |     Mean |     Error |    StdDev |   Median | Difference |
|------------------ |---------:|----------:|----------:|---------:|-----------:|
|        Raw_GetAll | 3.165 ms | 0.3776 ms | 1.1133 ms | 2.637 ms |            |
| Repository_GetAll | 3.025 ms | 0.3645 ms | 1.0690 ms | 2.502 ms |    -4.42 % |
|        Raw_Insert | 3.339 ms | 0.0636 ms | 0.0564 ms | 3.331 ms |            |
| Repository_Insert | 3.350 ms | 0.0633 ms | 0.0592 ms | 3.359 ms |     0.33 % |
|        Raw_Delete | 3.899 ms | 0.0772 ms | 0.2127 ms | 3.867 ms |            |
| Repository_Delete | 3.679 ms | 0.0662 ms | 0.1177 ms | 3.655 ms |    -5.64 % |
|        Raw_Update | 3.900 ms | 0.0610 ms | 0.0570 ms | 3.896 ms |            |
| Repository_Update | 4.037 ms | 0.0787 ms | 0.2017 ms | 4.082 ms |     3.51 % |

The GetAll is run for a 1,000 records.

As you can see this library is on occassions seemingly faster than raw Dapper. That seems somewhat implausible to me, so I'd put it down as within margin of error and call it a tie.