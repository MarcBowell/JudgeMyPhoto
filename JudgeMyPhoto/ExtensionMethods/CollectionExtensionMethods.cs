using System;
using System.Collections.Generic;
using System.Linq;

namespace Marcware.JudgeMyPhoto.ExtensionMethods
{
    internal static class CollectionExtensionMethods
    {
        /// <summary>
        /// Return a collection in a random sequence
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>        
        /// <returns></returns>
        public static List<T> InRandomSequence<T>(this List<T> collection)
        {
            Random randomizer = new Random(Guid.NewGuid().GetHashCode());

            List<KeyValuePair<int, double>> itemsToSort = new List<KeyValuePair<int, double>>();
            for (int item = 0; item < collection.Count; item++)
            {
                itemsToSort.Add(new KeyValuePair<int, double>(item, randomizer.NextDouble()));
            }
            itemsToSort = itemsToSort
                .OrderBy(i => i.Value)
                .ToList();

            List<T> result = new List<T>();
            foreach (KeyValuePair<int, double> item in itemsToSort)
            {
                result.Add(collection[item.Key]);
            }

            return result;
        }
    }
}
