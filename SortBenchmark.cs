using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;

public class SortBenchmark : MonoBehaviour
{
    //Method 1
    [DllImport("SortPlugin", EntryPoint = "TestSort")]
    static extern void NativeSort(int[] a, int length);

    [Tooltip("Text file from Random.org with one integer per line")]
    public TextAsset numbersFile;

    int[] original;

    void Start()
    {
        if (numbersFile == null)
        {
            UnityEngine.Debug.LogError("Assign the numbers TextAsset in the Inspector.");
            return;

        }

        original = ParseNumbers(numbersFile.text);
        UnityEngine.Debug.Log($"Loaded {original.Length} integers.");

      
        NativeSort((int[])original.Clone(), original.Length);
        ManagedSort.Sorter.Sort((int[])original.Clone());
        MonoBehaviourSort((int[])original.Clone());

   
        Measure("Method 1 - C++ native plugin", a => NativeSort(a, a.Length));
        Measure("Method 2 - C# managed plugin", a => ManagedSort.Sorter.Sort(a));
        Measure("Method 3 - C# MonoBehaviour", MonoBehaviourSort);
    }

    //Method 3
    void MonoBehaviourSort(int[] a)
    {
        Array.Sort(a);

    }

    void Measure(string label, Action<int[]> sortMethod)
    {

        int[] copy = (int[])original.Clone();

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        sortMethod(copy);
        stopwatch.Stop();

        bool sorted = IsSorted(copy);
        UnityEngine.Debug.Log(
            $"{label} took {stopwatch.Elapsed.TotalMilliseconds:F4} ms (sorted correctly: {sorted})");


    }

    static int[] ParseNumbers(string text)
    {

        string[] parts = text.Split(new[] { '\n', '\r', ' ', '\t', ',' },
                                    StringSplitOptions.RemoveEmptyEntries);
        int[] result = new int[parts.Length];
        for (int i = 0; i < parts.Length; i++)
            result[i] = int.Parse(parts[i]);
        return result;

    }

    static bool IsSorted(int[] a)
    {

        for (int i = 1; i < a.Length; i++)
            if (a[i - 1] > a[i]) return false;
        return true;


    }
}