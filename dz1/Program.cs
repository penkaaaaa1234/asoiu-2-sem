static int DamerauLevenshteinDistance(string s1, string s2)
{
    if (s1 == null || s2 == null) return -1;

    int len1 = s1.Length, len2 = s2.Length;
    if (len1 == 0) return len2;
    if (len2 == 0) return len1;

    int[,] matrix = new int[len1 + 1, len2 + 1];

    for (int i = 0; i <= len1; i++) 
        matrix[i, 0] = i;
    for (int j = 0; j <= len2; j++) 
        matrix[0, j] = j;

    for (int i = 1; i <= len1; i++)
    {
        for (int j = 1; j <= len2; j++)
        {
            int cost = (s1[i - 1] == s2[j - 1]) ? 0 : 1;
            int insert = matrix[i, j - 1] + 1;
            int delete = matrix[i - 1, j] + 1;
            int substitute = matrix[i - 1, j - 1] + cost;

            matrix[i, j] = System.Math.Min(System.Math.Min(insert, delete), substitute);

            if (i > 1 && j > 1 &&
                s1[i - 1] == s2[j - 2] &&
                s1[i - 2] == s2[j - 1])
            {
                matrix[i, j] = System.Math.Min(matrix[i, j], matrix[i - 2, j - 2] + cost);
            }
        }
    }
    return matrix[len1, len2];
}

static void PrintDistance(string s1, string s2)
{
    int d = DamerauLevenshteinDistance(s1, s2);
    System.Console.WriteLine($"'{s1}' переходит в '{s2}' за {d}");
}


System.Console.WriteLine("Поиск с опечатками (введите 'exit' для выхода)\n");

while (true)
{
    System.Console.Write("Строка 1: ");
    string input1 = System.Console.ReadLine();

    if (input1 == "exit") break;

    System.Console.Write("Строка 2: ");
    string input2 = System.Console.ReadLine();

    PrintDistance(input1, input2);
    System.Console.WriteLine();
}