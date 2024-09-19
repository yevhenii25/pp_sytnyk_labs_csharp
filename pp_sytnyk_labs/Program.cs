using System;
using System.Collections.Generic;
using System.Threading.Tasks;

class ParallelArraySum
{
    private const int ARRAY_SIZE = 500000;

    static void Main(string[] args)
    {
        int[] array = GenerateArray(ARRAY_SIZE);

        Console.Write("Input number of chunks: ");
        int numberOfThreads = int.Parse(Console.ReadLine());

        if (numberOfThreads <= 0)
        {
            return;
        }

        try
        {
            long totalSum = ParallelSum(array, numberOfThreads);
            Console.WriteLine("Sum: " + totalSum);
        }
        catch (AggregateException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public static int[] GenerateArray(int size)
    {
        int[] array = new int[size];
        for (int i = 0; i < size; i++)
        {
            array[i] = i + 1;
        }
        return array;
    }

    public static long ParallelSum(int[] array, int numberOfThreads)
    {
        List<Task<long>> tasks = new List<Task<long>>();

        int chunkSize = array.Length / numberOfThreads;
        int remainder = array.Length % numberOfThreads;

        int start = 0;
        for (int i = 0; i < numberOfThreads; i++)
        {
            int end = start + chunkSize;

            if (i == numberOfThreads - 1)
            {
                end += remainder;
            }

            int[] chunk = new int[end - start];
            Array.Copy(array, start, chunk, 0, chunk.Length);

            tasks.Add(Task.Run(() => ArraySumTask(chunk)));

            start = end;
        }

        Task.WaitAll(tasks.ToArray());
        long totalSum = 0;
        foreach (var task in tasks)
        {
            totalSum += task.Result;
        }

        return totalSum;
    }

    public static long ArraySumTask(int[] chunk)
    {
        long sum = 0;
        foreach (int value in chunk)
        {
            sum += value;
        }
        return sum;
    }
}
