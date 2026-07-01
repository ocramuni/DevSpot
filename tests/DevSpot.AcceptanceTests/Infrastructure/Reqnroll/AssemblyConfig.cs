using Xunit;

// Disable parallel execution of acceptance tests because they share the in-memory SQLite connection.
[assembly: CollectionBehavior(DisableTestParallelization = true)]
