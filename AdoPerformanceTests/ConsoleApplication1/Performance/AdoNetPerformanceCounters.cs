namespace ConsoleApplication1.Performance
{
	public enum AdoNetPerformanceCounters
	{
		NumberOfActiveConnectionPools,
		NumberOfReclaimedConnections,
		HardConnectsPerSecond,
		HardDisconnectsPerSecond,
		NumberOfActiveConnectionPoolGroups,
		NumberOfInactiveConnectionPoolGroups,
		NumberOfInactiveConnectionPools,
		NumberOfNonPooledConnections,
		NumberOfPooledConnections,
		NumberOfStasisConnections,
		// The following performance counters are more expensive to track.
		// Enable ConnectionPoolPerformanceCounterDetail in your config file.
		SoftConnectsPerSecond,
		SoftDisconnectsPerSecond,
		NumberOfActiveConnections,
		NumberOfFreeConnections,
	}
}