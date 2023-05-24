using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _GAME.Common
{
    /// <summary> Класс с полезными (не только в этой игре) часто используемыми методами, свойствами </summary>
    public static class RandomHelper
    {
        /// <summary> Метод возвращает случайный элемент с равномерным распределением </summary>
        public static T RandomElement<T>(params T[] elements)
        {
            return elements[UnityEngine.Random.Range(0, elements.Length)];
        }

        public static T RandomElement<T>(this List<T> list)
        {
            return list[UnityEngine.Random.Range(0, list.Count)];
        }

        public static T Random<T>(T[] array, float[] weights)
        {
            if (array.Length == 0)
            {
                return default(T);
            }
            else if (array.Length != weights.Length)
            {
                Debug.LogErrorFormat("Utils:random(list, weights):list.count != weights.count!!! list.count = {0}, weights.count = {1}", array.Length, weights.Length);
                return default(T);
            }
            else
            {
                var sum = weights.Sum();
                float curr = 0;
                var random = UnityEngine.Random.Range(0, sum);
                for (int i = 0; i < array.Length; i++)
                {
                    curr += weights[i];
                    if (random <= curr)
                        return array[i];
                }

                return default(T);
            }
        }

        /// <summary> Метод возвращает случайное число от 0 (включительно) до count (исключительно) </summary>
        public static int Random(int count)
        {
            return UnityEngine.Random.Range(0, count);
        }

        /// <summary> Метод возвращает случайное число от min (включительно) до max (исключительно) </summary>
        public static int Random(int min, int max)
        {
            return UnityEngine.Random.Range(min, max);
        }

        /// <summary> Метод возвращает случайное число от min до max</summary>
        public static float Random(float min, float max)
        {
            return UnityEngine.Random.Range(min, max);
        }

        /// <summary> Метод возвращает true, если сгенерированное число меньше chance, иначе false. chance -> [0, 1] </summary>
        public static bool Random(float chance)
        {
            return UnityEngine.Random.Range(0f, 1f) <= chance;
        }

        /// <summary> Метод возвращает случайное число от 0 (включительно) до weights.Lenght (исключительно) с заданными весами  </summary>
        public static int Random(params float[] weights)
        {
            if (weights.Length == 0)
                return -1;

            var sum = weights.Sum();
            float curr = 0;
            var random = UnityEngine.Random.Range(0, sum);
            for (int i = 0; i < weights.Length; i++)
            {
                curr += weights[i];
                if (random <= curr)
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Метод возвращает случайную точку внутри заданного RectTransform на заданном расстроянии от lastCenter и centers
        /// </summary>
        public static Vector3 RandomPointFromZone(RectTransform rect)
        {
            var corners = new Vector3[4];
            rect.GetWorldCorners(corners);

            return new Vector3(Random(corners[0].x, corners[3].x), Random(corners[0].y, corners[1].y), (corners[0].z + corners[3].z) / 2);
        }

        public static Vector3 RandomEuler()
        {
            //return new Vector3(UnityEngine.Random.Range(0f, 360f),
            //                   UnityEngine.Random.Range(0f, 360f),
            //                   UnityEngine.Random.Range(0f, 360f));

            return UnityEngine.Random.rotation.eulerAngles;
        }

        public static bool RandomBool()
        {
            return UnityEngine.Random.Range(0, 2) == 0;
        }
    }
}
