// Reqnroll acceptance tests share one in-memory SQLite database.
// Parallel execution would cause concurrent resets and constraint violations.
[assembly: Xunit.CollectionBehavior(DisableTestParallelization = true)]
