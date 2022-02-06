namespace Totem.Map.Builder;

internal static class TypoDetector
{
    // https://github.com/NoelKennedy/MinimumEditDistance/blob/master/MinimumEditDistance/Levenshtein.cs

    internal static bool IsPossibleTypo(string expected, string actual)
    {
        if(string.IsNullOrEmpty(expected) || string.IsNullOrEmpty(actual))
        {
            // Short-circuit

            return IsWithinTwoEdits(expected, actual, new RectangularArray(0, 0));
        }
        else if(expected.Length * actual.Length > _maxStringProductLength)
        {
            // Large strings, use large data structure

            return IsWithinTwoEdits(expected, actual, new JaggedArray(expected.Length + 1, actual.Length + 1));
        }
        else
        {
            // Small strings, can use rectangular array

            return IsWithinTwoEdits(expected, actual, new RectangularArray(expected.Length + 1, actual.Length + 1));
        }
    }

    static bool IsWithinTwoEdits(string x, string y, MemoryStructure memory) =>
        CalculateEditDistance(x, y, memory) <= 2;

    static int CalculateEditDistance(string x, string y, MemoryStructure memory)
    {
        const int substitutionCost = 1;

        if(string.IsNullOrEmpty(x))
        {
            return string.IsNullOrEmpty(y) ? 0 : y.Length;
        }

        var m = x.Length + 1;
        var n = y.Length + 1;

        // Map empties to each other

        for(int i = 0; i < m; i++)
        {
            memory[i, 0] = i;
        }

        for(int i = 0; i < n; i++)
        {
            memory[0, i] = i;
        }

        for(int i = 1; i < m; i++)
        {
            for(int j = 1; j < n; j++)
            {
                if(x[i - 1] == y[j - 1])
                {
                    // No cost, letters are the same

                    memory[i, j] = memory[i - 1, j - 1];
                }
                else
                {
                    var delete = memory[i - 1, j] + 1;
                    var insert = memory[i, j - 1] + 1;
                    var substitution = memory[i - 1, j - 1] + substitutionCost;

                    memory[i, j] = Math.Min(delete, Math.Min(insert, substitution));
                }
            }
        }

        return memory[m - 1, n - 1];
    }

    const int _maxStringProductLength = 536848900;

    abstract class MemoryStructure
    {
        internal abstract int this[int i, int j] { get; set; }
    }

    sealed class RectangularArray : MemoryStructure
    {
        readonly int[,] _value;

        internal RectangularArray(int m, int n)
        {
            _value = new int[m, n];
        }

        internal override int this[int i, int j]
        {
            get { return _value[i, j]; }
            set { _value[i, j] = value; }
        }
    }

    sealed class JaggedArray : MemoryStructure
    {
        int[][] _value;

        internal JaggedArray(int m, int n)
        {
            _value = new int[m][];

            for(int i = 0; i < m; i++)
            {
                _value[i] = new int[n];
            }
        }

        internal override int this[int i, int j]
        {
            get { return _value[i][j]; }
            set { _value[i][j] = value; }
        }
    }
}
