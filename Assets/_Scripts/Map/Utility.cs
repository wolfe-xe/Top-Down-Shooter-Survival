using System.Collections;
using System.Collections.Generic;

public static class Utility
{
    public static T[] ShuffleArray<T>(T[] array, int seed)
    {
        System.Random r = new System.Random(seed);

        for(int i = 0; i < array.Length - 1;  i++)
        {
            int randomIndex = r.Next(i, array.Length);
            T tempItem = array[randomIndex];
            array[randomIndex] = array[i];
            array[i] = tempItem;

        }
        return array;
    }
}
