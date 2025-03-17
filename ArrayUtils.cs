internal static class ArrayUtils
{
    internal static void ShuffleArray(this Array array)
    {
        ShuffleArray(array, 0, array.Length - 1);
    }
    internal static void ShuffleArray(this Array array, int begin, int end)
    {

        System.Random random = new();
        for (int i = end - 1; i >= begin; i--)
        {
            int j = random.Next(begin, i);
            var t = array.GetValue(i);
            array.SetValue(array.GetValue(j), i);
            array.SetValue(t, j);
        }
    }
    internal static void RemoveAt<T>(this T[] array, int index)
    {
        if (index < 0 || index >= array.Length) return;
        for (int i = index; i < array.Length - 1; i++)
        {
            array.SetValue(array.GetValue(i + 1), i);
        }
        Array.Resize(ref array, array.Length - 1);
    }

}