using System.Diagnostics;
using UnityEngine;

public class PrimeChecker : MonoBehaviour
{
    [Header("Prime Number Settings")]
    [SerializeField] private int numberToCheck = 982451653;

    private void Start()
    {
        CheckPrime(numberToCheck);
    }

    [ContextMenu("Check Prime Number")]
    public void CheckPrimeFromInspector()
    {
        CheckPrime(numberToCheck);
    }

    public bool CheckPrime(int number)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        
        bool isPrime = IsPrime(number);
        
        stopwatch.Stop();
        
        LogPrimeResult(number, isPrime, stopwatch.ElapsedMilliseconds, stopwatch.ElapsedTicks);
        
        return isPrime;
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

    private void LogPrimeResult(int number, bool isPrime, long milliseconds, long ticks)
    {
        string result = isPrime ? "PRIME" : "NOT PRIME";
        string timeInMicroseconds = (ticks * 1000000.0 / Stopwatch.Frequency).ToString("F3");
        
        UnityEngine.Debug.Log($"<color=cyan>Prime Check Results:</color>\n" +
                             $"Number: <color=white>{number:N0}</color>\n" +
                             $"Result: <color={(isPrime ? "green" : "red")}>{result}</color>\n" +
                             $"Time: <color=orange>{milliseconds}ms ({timeInMicroseconds}μs)</color>\n" +
                             $"Ticks: <color=gray>{ticks:N0}</color>");
    }
}