// </> by Halil Emre Yildiz (Github: @JahnStar)
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class DataStructureTest : MonoBehaviour 
{
    [Header("Test Settings")]
    [SerializeField] private int testSize = 100000;
    [Flags] 
    private enum TestTypes { Array = 1, List = 2, Dictionary = 4, HashSet = 8 }
    [SerializeField] private TestTypes activeTests = (TestTypes)(-1);
    
    // We don't serialize fields to prevent effecting the performance of the tests.
    private int[] arrayData;
    private List<int> listData;
    private Dictionary<int, int> dictionaryData;
    private HashSet<int> hashSetData;
    
    private StringBuilder report;
    private Stopwatch stopwatch;

    // This is the analysis report that stores the operation times for each data structure.
    private Dictionary<Tuple<string, string>, List<long>> analysisReport; // <Data Structure, Operation> -> List of operation times

    private void Awake()
    {
        stopwatch = new();
        analysisReport = new();
        Debug.Log($"Press 0-9 to run tests. Or press Shift + 0-9 to run profiler tests.");
    }

    private void Update()
    {
        if (Input.anyKeyDown) for (KeyCode key = KeyCode.Alpha0; key <= KeyCode.Alpha9; key++)
        {
            if (Input.GetKeyDown(key))
            {
                int sets = key - KeyCode.Alpha0;
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) RunProfilerTests(sets == 0 ? 10 : sets);
                else RunTests(sets == 0 ? 10 : sets);
                break;
            }
        }
    }

    private void InitializeTest()
    {
        arrayData = new int[testSize];
        listData = new List<int>(testSize);
        dictionaryData = new Dictionary<int, int>(testSize);
        hashSetData = new HashSet<int>();
        // log system
        stopwatch ??= new();
        report = new StringBuilder("==== Data Structure Performance Test ====\n");
        analysisReport = new();
    }
    
    #region Accurate Test
    [ContextMenu("Run Test (100 Set)")]
    private void RunTest100Set() => RunTests(100);
    [ContextMenu("Run Test (1000 Set)")]
    private void RunTest1000Set() => RunTests(1000);
    [ContextMenu("Run Test (10000 Set)")]
    private void RunTest10000Set() => RunTests(10000);
    
    private void RunTests(int sets)
    {
        InitializeTest();
        var testTime = DateTime.Now;
        var startTime = Time.realtimeSinceStartup;

        report.AppendLine($"\n{testTime}");
        report.AppendLine($"Running {sets} sets of tests...");

        for (int i = 0; i < sets; i++)
        {
            if (activeTests.HasFlag(TestTypes.Array)) RunTestSet("Array", new() { AddToArray, IterateArray, RemoveFromArray });
            if (activeTests.HasFlag(TestTypes.List)) RunTestSet("List", new() { AddToList, IterateList, RemoveFromList });
            if (activeTests.HasFlag(TestTypes.Dictionary)) RunTestSet("Dictionary", new() { AddToDictionary, IterateDictionary, RemoveFromDictionary });
            if (activeTests.HasFlag(TestTypes.HashSet)) RunTestSet("HashSet", new() { AddToHashSet, IterateHashSet, RemoveFromHashSet });
        }

        report.AppendLine($"\nExecution Time: {(Time.realtimeSinceStartup - startTime) * 1000:F2}ms");

        PrintResults(sets);
    }

    private void RunTestSet(string structure, List<Action> operations)
    {
        operations.ForEach(operation => 
        {
            var operationKey = new Tuple<string, string>(structure, operation.Method.Name);
            if (!analysisReport.ContainsKey(operationKey)) analysisReport[operationKey] = new List<long>();

            stopwatch.Reset();
            stopwatch.Start();
            operation();
            stopwatch.Stop();

            analysisReport[operationKey].Add(stopwatch.ElapsedTicks);
        });
    }

    private void PrintResults(int sets)
    {
        report.AppendLine($"\n==== Test Results ({sets} sets) ====");
        
        var structures = new HashSet<string>();
        
        foreach (var kvp in analysisReport)
        {
            string structure = kvp.Key.Item1;
            string operation = kvp.Key.Item2;
            //
            if (!structures.Contains(structure))
            {
                report.AppendLine($"\n{structure}:");
                structures.Add(structure);
            }
            double averageMs = kvp.Value.Average() / (Stopwatch.Frequency / 1000.0); // Convert frequency (ticks) to milliseconds
            #if UNITY_EDITOR
            report.AppendLine($"{UnityEditor.ObjectNames.NicifyVariableName(operation)}: {averageMs:F3}ms");
            #else
            report.AppendLine($"{operation}: {averageMs:F3}ms");
            #endif
        }
        
        // Debug log
        Debug.Log(report.ToString());
        
        // Save to file
        string logPath = Application.dataPath + "/DataStructureTestLog_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm") + ".txt";
        System.IO.File.WriteAllText(logPath, report.ToString());
        Debug.Log($"Test results are saved to {logPath}");
        
        #if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
        #endif
    }
    #endregion

    #region Profiler Test
    [ContextMenu("Run Profiler Test (10 Set)")]
    private void RunProfilerTest10Set() => RunProfilerTests(10);

    private void RunProfilerTests(int sets)
    {
        #if UNITY_EDITOR
        InitializeTest();
        var testTime = DateTime.Now;
        var startTime = Time.realtimeSinceStartup;
        
        report.AppendLine($"\n---- Profiler Test ({sets} sets) ----");
        report.AppendLine($"{testTime}");

        for (int i = 0; i < sets; i++)
        {
            if (activeTests.HasFlag(TestTypes.Array)) ProfilingTest("Array", new() { AddToArray, IterateArray, RemoveFromArray });
            if (activeTests.HasFlag(TestTypes.List)) ProfilingTest("List", new() { AddToList, IterateList, RemoveFromList });
            if (activeTests.HasFlag(TestTypes.Dictionary)) ProfilingTest("Dictionary", new() { AddToDictionary, IterateDictionary, RemoveFromDictionary });
            if (activeTests.HasFlag(TestTypes.HashSet)) ProfilingTest("HashSet", new() { AddToHashSet, IterateHashSet, RemoveFromHashSet });
        }
        
        report.AppendLine($"Execution Time: {(Time.realtimeSinceStartup - startTime) * 1000:F2}ms");

        // Save report to file
        var reportPath = $"{Application.dataPath}/ProfilerReport_{testTime:yyyy-MM-dd_HH-mm}.txt";
        System.IO.File.WriteAllText(reportPath, report.ToString());
        Debug.Log($"Profiler report saved to: {reportPath}");
        
        UnityEditor.AssetDatabase.Refresh();
        #else
        Debug.LogError("Profiler test can only be run in Unity Editor.");
        #endif
    }

   private void ProfilingTest(string structure, List<Action> operations)
    {
        #if UNITY_EDITOR
        string sampleName = $"{string.Join(", ", operations.Select(op => UnityEditor.ObjectNames.NicifyVariableName(op.Method.Name)))}";
        report.AppendLine($"\n# {structure} Test\nOperations: {sampleName}");
        
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        
        var beforeTotalMemory = GC.GetTotalMemory(true);
        var beforeAllocatedMemory = Profiler.GetTotalAllocatedMemoryLong();
        var beforeGfxMemory = Profiler.GetAllocatedMemoryForGraphicsDriver();
        var beforeCPUTime = Time.realtimeSinceStartup;
        
        Profiler.BeginSample(sampleName.Replace(" ", "_"));
        
        operations.ForEach(operation => 
        {
            string opName = UnityEditor.ObjectNames.NicifyVariableName(operation.Method.Name);
            Profiler.BeginSample(opName);
            operation();
            Profiler.EndSample();
        });
        
        Profiler.EndSample();
        
        var afterTotalMemory = GC.GetTotalMemory(false);
        var afterAllocatedMemory = Profiler.GetTotalAllocatedMemoryLong();
        var afterGfxMemory = Profiler.GetAllocatedMemoryForGraphicsDriver();
        var cpuTime = (Time.realtimeSinceStartup - beforeCPUTime) * 1000;
        
        var managedMemoryDelta = (afterTotalMemory - beforeTotalMemory) / 1024f;
        var totalMemoryDelta = (afterAllocatedMemory - beforeAllocatedMemory) / 1024f;
        var gfxMemoryDelta = (afterGfxMemory - beforeGfxMemory) / 1024f;
        
        report.AppendLine($"CPU Time: {cpuTime:F2}ms");
        report.AppendLine($"Managed Memory Delta: {managedMemoryDelta:F2}KB");
        report.AppendLine($"Total Memory Delta: {totalMemoryDelta:F2}KB");
        report.AppendLine($"Graphics Memory Delta: {gfxMemoryDelta:F2}KB");
        
        if (System.Threading.Thread.CurrentThread.IsThreadPoolThread) report.AppendLine("Running on Thread Pool");
        #endif
    }
    #endregion

    // Array Operations
    private void AddToArray()
    {
        for (int i = 0; i < testSize; i++) arrayData[i] = Random.Range(0, testSize);
    }

    private void IterateArray()
    {
        int sum = 0;
        for (int i = 0; i < arrayData.Length; i++) sum += arrayData[i];
    }

    private void RemoveFromArray()
    {
        int removeIndex = Random.Range(0, testSize - 1);
        for (int i = removeIndex; i < testSize - 1; i++) arrayData[i] = arrayData[i + 1];
    }

    // List Operations
    private void AddToList()
    {
        listData.Clear();
        for (int i = 0; i < testSize; i++) listData.Add(Random.Range(0, testSize));
    }

    private void IterateList()
    {
        int sum = 0;
        for (int i = 0; i < listData.Count; i++) sum += listData[i];
    }

    private void RemoveFromList()
    {
        if (listData.Count > 0)
        {
            int removeIndex = Random.Range(0, listData.Count);
            listData.RemoveAt(removeIndex);
        }
    }

    // Dictionary Operations
    private void AddToDictionary()
    {
        dictionaryData.Clear();
        for (int i = 0; i < testSize; i++) dictionaryData[i] = Random.Range(0, testSize);
    }

    private void IterateDictionary()
    {
        int sum = 0;
        foreach (var kvp in dictionaryData) sum += kvp.Value;
    }

    private void RemoveFromDictionary()
    {
        if (dictionaryData.Count > 0)
        {
            int removeKey = Random.Range(0, testSize);
            dictionaryData.Remove(removeKey);
        }
    }

    // HashSet Operations
    private void AddToHashSet()
    {
        hashSetData.Clear();
        for (int i = 0; i < testSize; i++) hashSetData.Add(Random.Range(0, testSize));
    }

    private void IterateHashSet()
    {
        int sum = 0;
        foreach (var item in hashSetData) sum += item;
    }

    private void RemoveFromHashSet()
    {
        if (hashSetData.Count > 0)
        {
            int removeValue = Random.Range(0, testSize);
            hashSetData.Remove(removeValue);
        }
    }
}