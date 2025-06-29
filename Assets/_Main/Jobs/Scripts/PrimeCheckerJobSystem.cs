using System.Diagnostics;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class PrimeCheckerJobSystem : MonoBehaviour
{
    [Header("Prime Number Settings")]
    [SerializeField] private int numberToCheck = 982451653;
    [SerializeField] private bool useParallelJob = true;
    [SerializeField] private int innerLoopBatchCount = 1000;
    
    private void Start()
    {
        CheckPrime(numberToCheck);
    }

    [ContextMenu("Check Prime Number")]
    public void CheckPrimeFromInspector()
    {
        CheckPrime(numberToCheck);
    }

    public void CheckPrime(int number)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        
        if (useParallelJob)
        {
            CheckPrimeWithParallelFor(number, stopwatch);
        }
        else
        {
            CheckPrimeWithSingleJob(number, stopwatch);
        }
    }

    private void CheckPrimeWithSingleJob(int number, Stopwatch stopwatch)
    {
        var result = new NativeArray<bool>(1, Allocator.TempJob);
        
        var job = new PrimeCheckJob
        {
            number = number,
            result = result
        };
        
        JobHandle jobHandle = job.Schedule();
        jobHandle.Complete();
        
        bool isPrime = result[0];
        
        stopwatch.Stop();
        
        LogPrimeResult(number, isPrime, stopwatch.ElapsedMilliseconds, stopwatch.ElapsedTicks, "Single Job");
        
        result.Dispose();
    }

    private void CheckPrimeWithParallelFor(int number, Stopwatch stopwatch)
    {
        if (number <= 1)
        {
            stopwatch.Stop();
            LogPrimeResult(number, false, stopwatch.ElapsedMilliseconds, stopwatch.ElapsedTicks, "Parallel Job");
            return;
        }
        
        if (number == 2)
        {
            stopwatch.Stop();
            LogPrimeResult(number, true, stopwatch.ElapsedMilliseconds, stopwatch.ElapsedTicks, "Parallel Job");
            return;
        }
        
        if (number % 2 == 0)
        {
            stopwatch.Stop();
            LogPrimeResult(number, false, stopwatch.ElapsedMilliseconds, stopwatch.ElapsedTicks, "Parallel Job");
            return;
        }

        int sqrt = Mathf.FloorToInt(Mathf.Sqrt(number));
        int rangeSize = (sqrt - 3) / 2 + 1; // Only odd numbers from 3 to sqrt
        
        if (rangeSize <= 0)
        {
            stopwatch.Stop();
            LogPrimeResult(number, true, stopwatch.ElapsedMilliseconds, stopwatch.ElapsedTicks, "Parallel Job");
            return;
        }

        // Create array to store results for each divisor check
        var divisorResults = new NativeArray<bool>(rangeSize, Allocator.TempJob);
        
        var parallelJob = new ParallelPrimeCheckJob
        {
            number = number,
            divisorResults = divisorResults
        };
        
        JobHandle jobHandle = parallelJob.Schedule(rangeSize, innerLoopBatchCount);
        jobHandle.Complete();

        stopwatch.Stop(); // Stop timing here - only measures job execution

        // Result checking happens after timing
        bool isPrime = true;
        for (int i = 0; i < rangeSize; i++)
        {
            if (divisorResults[i])
            {
                isPrime = false;
                break;
            }
        }
        
        stopwatch.Stop();
        
        LogPrimeResult(number, isPrime, stopwatch.ElapsedMilliseconds, stopwatch.ElapsedTicks, "Parallel Job");
        
        divisorResults.Dispose();
    }

    private void LogPrimeResult(int number, bool isPrime, long milliseconds, long ticks, string method)
    {
        string result = isPrime ? "PRIME" : "NOT PRIME";
        string timeInMicroseconds = (ticks * 1000000.0 / Stopwatch.Frequency).ToString("F3");
        
        UnityEngine.Debug.Log($"<color=cyan>Prime Check Results ({method}):</color>\n" +
                             $"Number: <color=white>{number:N0}</color>\n" +
                             $"Result: <color={(isPrime ? "green" : "red")}>{result}</color>\n" +
                             $"Time: <color=orange>{milliseconds}ms ({timeInMicroseconds}μs)</color>\n" +
                             $"Ticks: <color=gray>{ticks:N0}</color>");
    }
}

public struct PrimeCheckJob : IJob
{
    public int number;
    public NativeArray<bool> result;

    public void Execute()
    {
        result[0] = IsPrime(number);
    }

    private bool IsPrime(int number)
    {
        if (number <= 1) return false;
        if (number == 2) return true;
        if (number % 2 == 0) return false;
        
        for (int i = 3; i * i <= number; i += 2)
        {
            if (number % i == 0)
                return false;
        }
        
        return true;
    }
}

public struct ParallelPrimeCheckJob : IJobParallelFor
{
    [ReadOnly] public int number;
    [WriteOnly] public NativeArray<bool> divisorResults;

    public void Execute(int index)
    {
        int divisor = 3 + index * 2; // Convert index to odd number starting from 3
        
        if (divisor * divisor > number)
        {
            divisorResults[index] = false; // No divisor found
            return;
        }
        
        // Check if this divisor divides the number
        divisorResults[index] = (number % divisor == 0);
    }
}