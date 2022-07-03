# Benchmark
This is a small project to measure the CPU time consumed by `Dapper.Repository` CRUD methods compared to a "raw" Dapper implementation.

If you're curious you can run the benchmarks yourself by spinning up the SqlServer pod (either by using podman or docker - I use podman myself)

Here are my results, when run on an AMD Ryzen 3800X with 32GB of memory:

|            Method |       Mean |    Error |    StdDev | Difference |
|------------------ |-----------:|---------:|----------:|-----------:|
|        Raw_GetAll | 1,298.5 us |  4.19 us |   3.50 us |            |
| Repository_GetAll | 1,302.2 us |  6.09 us |   5.40 us |     0.30 % |
|        Raw_Insert | 3,186.7 us | 58.80 us |  55.00 us |            |
| Repository_Insert | 3,239.9 us | 57.80 us |  51.24 us |     1.66 % |
|        Raw_Delete |   872.1 us |  8.56 us |   9.52 us |            |
| Repository_Delete |   887.5 us | 17.68 us |  37.67 us |     1,72 % |
|        Raw_Update |   829.0 us | 16.02 us |  18.45 us |            |
| Repository_Update |   865.6 us |  9.98 us |   9.33 us |     4,34 % |


The GetAll is run for a 1,000 records.

Also do note the numbers tend to change quite a bit from run to run, so take this with a grain of salt.