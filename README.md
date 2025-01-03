# Unity-Data-Structures-Analysis

This Unity component provides comprehensive performance testing and profiling capabilities for common C# data structures: Array, List, Dictionary, and HashSet. It offers both standard performance measurements and Unity Profiler integration for detailed memory analysis.

### Features

Performance testing for multiple data structures
Memory usage profiling
Configurable test sizes and iterations
Automated test reporting
File-based logging
Unity Editor integration

### Configuration

```csharp
[Header("Test Settings")]
[SerializeField] private int testSize = 100000;
[SerializeField] private TestTypes activeTests = (TestTypes)(-1);
[Flags]
private enum TestTypes { Array = 1, List = 2, Dictionary = 4, HashSet = 8 }
```

testSize: Number of elements to use in tests (default: 100,000)

activeTests: Flag enum to select which data structures to test

## Running Tests

- Via Keyboard:

Press numeric keys (0-9) to run standard tests
Hold Shift + numeric keys for profiler tests
0 defaults to 10 test sets

- Via Context Menu:

Run Test (100 Set)
Run Test (1000 Set)
Run Test (10000 Set)
Run Profiler Test (10 Set)

Test Operations
Each data structure is tested for three operations:

Add: Insert elements
Iterate: Traverse all elements
Remove: Delete elements

Implementation Details
Test Initialization
```csharp
private void InitializeTest()
{
    arrayData = new int[testSize];
    listData = new List<int>(testSize);
    dictionaryData = new Dictionary<int, int>(testSize);
    hashSetData = new HashSet<int>();
    // Initialize logging systems
}
```
Performance Measurement
The system uses two measurement approaches:

Stopwatch-based Measurement:

```csharp
stopwatch.Reset();
stopwatch.Start();
operation();
stopwatch.Stop();
```

Unity Profiler Integration:

```csharp
Profiler.BeginSample(sampleName);
operations.ForEach(operation => operation());
Profiler.EndSample();
```

Memory Analysis
The profiler test tracks three memory metrics:

Managed Memory (GC)
Total Allocated Memory
Graphics Driver Memory

# Performance Analysis Results

## Test Environment
- Unity Version: 2022.3.50f1 LTS
- Platform: Ubuntu 22.04
- Test Size: 100,000 elements
- Test Sets: 10 iterations

## Standard Test Results

### Array Performance
- **Add**: 27.115ms
- **Iterate**: 0.098ms
- **Remove**: 0.068ms
- **Total Execution**: Fast (≈27.281ms total)
- **Memory Usage**: Lowest (≈4-6MB)
- **CPU Time**: 22-26ms per operation set

### List Performance
- **Add**: 39.084ms
- **Iterate**: 22.957ms
- **Remove**: 0.014ms
- **Total Execution**: Moderate (≈62.055ms total)
- **Memory Usage**: Moderate (≈15-19MB)
- **CPU Time**: 52-74ms per operation set

### Dictionary Performance
- **Add**: 63.818ms
- **Iterate**: 46.993ms
- **Remove**: 0.006ms
- **Total Execution**: Slow (≈110.817ms total)
- **Memory Usage**: High (≈27-30MB)
- **CPU Time**: 94-122ms per operation set

### HashSet Performance
- **Add**: 108.177ms
- **Iterate**: 16.168ms
- **Remove**: 0.006ms
- **Total Execution**: Slowest (≈124.351ms total)
- **Memory Usage**: Highest (≈27-53MB)
- **CPU Time**: 96-144ms per operation set

## Profiler Analysis

### Memory Management
1. **Managed Memory**
   - Most operations show 0.00KB delta
   - Occasional spikes in HashSet (2292KB)
   - Efficient garbage collection

2. **Total Memory Usage**
   - Array: 4-6MB consistent
   - List: 15-52MB variable
   - Dictionary: 15-37MB variable
   - HashSet: 0.5-53MB highly variable

3. **Graphics Memory**
   - Consistently 0.00KB for all structures
   - No graphics driver impact

### CPU Performance
1. **Array Operations**
   - Most efficient overall
   - Consistent performance
   - Best for iteration

2. **List Operations**
   - Good for dynamic sizing
   - Moderate memory overhead
   - Efficient removal

3. **Dictionary Operations**
   - Higher CPU usage
   - Consistent performance
   - Excellent for lookups

4. **HashSet Operations**
   - Highest CPU usage
   - Variable performance
   - Good for unique sets

## Key Findings

1. **Performance Hierarchy**
   ```
   Speed: Array > List > Dictionary > HashSet
   Memory: Array > List > Dictionary ≈ HashSet
   ```

2. **Operation Costs**
   ```
   Add: Array < List < Dictionary < HashSet
   Iterate: Array < HashSet < List < Dictionary
   Remove: Dictionary ≈ HashSet < List < Array
   ```

3. **Memory Stability**
   - Arrays show most stable memory usage
   - Lists and HashSets show highest variability
   - Dictionaries maintain consistent but high usage

4. **Thread Behavior**
   - All operations run on main thread
   - No thread pool utilization observed
   - Consistent threading model

## Recommendations

### Use Arrays When:
- Size is fixed
- Performance is critical
- Memory is limited
- Sequential access is common

### Use Lists When:
- Size varies frequently
- Quick insertion/removal at end
- Moderate memory usage is acceptable
- Index access is needed

### Use Dictionaries When:
- Key-value pairs are needed
- Fast lookup is critical
- Memory isn't constrained
- Random access is frequent

### Use HashSets When:
- Unique values are required
- Memory can be sacrificed
- Fast lookup is needed
- Order isn't important

## Conclusions

The choice of data structure significantly impacts both performance and memory usage:

1. Arrays provide best performance but least flexibility
2. Lists offer good balance of performance and functionality
3. Dictionaries excel at lookups but consume more resources
4. HashSets are specialized for unique collections but resource intensive
