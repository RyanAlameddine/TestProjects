using System;
using System.Diagnostics.CodeAnalysis;

namespace LevenshteinDistance
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Input word 1");
            string start = " " + Console.ReadLine();
            Console.WriteLine("Input word 2");
            string end = " " + Console.ReadLine();


            Edit[,] levenshtein = new Edit[start.Length, end.Length];
            levenshtein[0, 0] = new Edit(null, "Initial State", 0);
            for (int i = 1; i < levenshtein.GetLength(0); i++)
            {
                levenshtein[i, 0] = new Edit(levenshtein[i - 1, 0], $"Removed {start[i]}", i);
            }
            for (int j = 1; j < levenshtein.GetLength(1); j++)
            {
                levenshtein[0, j] = new Edit(levenshtein[0, j - 1], $"Added {end[j]}", j);
            }

            for (int i = 1; i < levenshtein.GetLength(0); i++)
            {
                for (int j = 1; j < levenshtein.GetLength(1); j++)
                {
                    if (i == 1 && j == 2)
                        Console.WriteLine();
                    if (start[i] == end[j])
                    {
                        levenshtein[i, j] = levenshtein[i - 1, j - 1];
                        continue;
                    }
                    var insert = levenshtein[i, j - 1];
                    var delete = levenshtein[i - 1, j];
                    var replace = levenshtein[i - 1, j - 1];
                    levenshtein[i, j] = Min(insert, delete, replace, start[i], end[j]);
                }
            }

            Console.WriteLine();
            var finalEdit = levenshtein[levenshtein.GetLength(0) - 1, levenshtein.GetLength(1) - 1];
            while(finalEdit != null)
            {
                Console.WriteLine(finalEdit.Message);
                finalEdit = finalEdit.Previous;
            }
        }

        static Edit Min(Edit insert, Edit delete, Edit replace, char startChar, char endChar)
        {
            if (replace.Count <= insert.Count && replace.Count <= delete.Count)
            {
                return new Edit(replace, $"Replaced {startChar} with {endChar}", replace.Count + 1);
            }
            else if (insert.Count < delete.Count)
            {
                return new Edit(insert, $"Inserted {endChar}", insert.Count + 1);
            }
            else
            {
                return new Edit(delete, $"Deleted {startChar}", delete.Count + 1);
            }
        }
    }
}

class Edit
{
    public readonly Edit Previous;
    public readonly string Message;
    public readonly int Count;

    public Edit(Edit previous, string message, int count)
    {
        Previous = previous;
        Message = message;
        Count = count;
    }

    public override string ToString()
    {
        return $"C: {Count}, M: {Message}";
    }
}
