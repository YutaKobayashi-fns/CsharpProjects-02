
int[,] aaa = new int[,] { { 1, 50, 100 }, { 5, 20, 40 }, { 10, 10, 20 }, { 20, 5, 10 } };

for (int i = 0; i < aaa.GetLength(0); i++)
{
    for (int j = 0; j < aaa.GetLength(1); j++)
    {
        Console.WriteLine($"[{i},{j}] => {aaa[i,j]}");
    }
}
