Sample project copied from https://github.com/dotnet/EntityFramework.Docs/tree/master/samples/core/Miscellaneous/Testing/ItemsWebApi on 02-JAN-2021 and then the class SqlServerDockerItemsControllerTest.cs was added.

This was done to add to the possible testing solutions outlined by https://docs.microsoft.com/en-us/ef/core/testing/testing-sample


## Why?
Although this sample already has a SQL Server example, it uses `(localdb)` which is only support on Windows or if you install SQL Server Express. Some users may prefer (or already have) Docker available instead.
