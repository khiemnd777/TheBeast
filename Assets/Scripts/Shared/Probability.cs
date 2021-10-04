using System.Linq;
using UnityEngine;

public class Probability<T>
{
    T[] items;

    public void Initialize (T[] values, float[] percents)
    {
        var random = new System.Random ();
        // init array with 100 elements;
        var capacity = Mathf.FloorToInt (percents.Sum ());
        var arr = new T[capacity];
        for (var x = 0; x < arr.Length; x++)
        {
            var t = 0f;
            T v = default (T);
            for (var y = 0; y < percents.Length; y++)
            {
                t += percents[y];
                if (x == t)
                {
                    continue;
                }
                else if (x < t)
                {
                    v = values[y];
                    break;
                }
            }
            arr[x] = v;
        }
        arr = arr.OrderBy (x => random.Next ()).ToArray ();
        items = arr;
    }

    public T GetValueInProbability ()
    {
        if (items == null || items.Length == 0)
            return default (T);
        var index = Random.Range (0, items.Length - 1);
        return items[index];
    }
}
