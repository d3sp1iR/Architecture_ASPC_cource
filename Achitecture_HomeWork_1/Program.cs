/// <summary>
/// Вычисление расстояния Дамерау-Левенштейна между двумя строками.
/// </summary>
static int Distance(string str1Param, string str2Param)
{
    if (str1Param == null || str2Param == null)
        return -1;

    string str1 = str1Param.ToUpper();
    string str2 = str2Param.ToUpper();

    int lenStr1 = str1.Length;
    int lenStr2 = str2.Length;

    if (lenStr1 == 0)
        return lenStr2;
    if (lenStr2 == 0)
        return lenStr1;

    int[,] matrix = new int[lenStr1 + 1, lenStr2 + 1];

    for (int i = 0; i <= lenStr1; i++)
        matrix[i, 0] = i;
    for (int j = 0; j <= lenStr2; j++)
        matrix[0, j] = j;

    // Заполнение матрицы
    for (int i = 1; i <= lenStr1; i++)
    {
        for (int j = 1; j <= lenStr2; j++)
        {
            int cost = (str1[i - 1] == str2[j - 1]) ? 0 : 1;

            matrix[i, j] = Math.Min(
                Math.Min(matrix[i - 1, j] + 1,         // удаление
                        matrix[i, j - 1] + 1),         // вставка
                matrix[i - 1, j - 1] + cost            // замена
            );

            // Транспозиция
            if (i > 1 && j > 1 &&
                str1[i - 1] == str2[j - 2] &&
                str1[i - 2] == str2[j - 1])
            {
                matrix[i, j] = Math.Min(matrix[i, j], matrix[i - 2, j - 2] + 1);

            }
        }
    }

    return matrix[lenStr1, lenStr2];
}

while (true)
{
    Console.Write("Введите первую строку: ");
    string? s1 = Console.ReadLine();

    if (s1?.ToLower() == "exit")
        break;

    Console.Write("Введите вторую строку: ");
    string? s2 = Console.ReadLine();

    if (s1 != null && s2 != null)
    {
        int result = Distance(s1, s2);
        Console.WriteLine($"'{s1}', '{s2}' -> {result}");
    }
}