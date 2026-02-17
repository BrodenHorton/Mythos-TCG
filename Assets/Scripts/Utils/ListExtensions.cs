using System;
using System.Collections.Generic;

public static class ListExtensions {
    
    public static void Shuffle<T>(this List<T> list) {
        Random rand = new Random();
        List<T> copy = new List<T>();
        for(int i = 0; i < list.Count; i++)
            copy.Add(list[i]);
        for (int i = 0; i < list.Count; i++) {
            int randIndex = rand.Next(copy.Count);
            list[i] = copy[randIndex];
            copy.RemoveAt(randIndex);
        }
    }
}
