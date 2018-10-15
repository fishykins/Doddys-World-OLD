using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dw
{
    public class MinMax
    {

        public float Min { get; private set; }
        public float Max { get; private set; }
        public float Average { get; private set; }

        private float sum = 0f;
        private int count = 0;

        public MinMax()
        {
            Min = float.MaxValue;
            Max = float.MinValue;
        }

        public void AddValue(float v)
        {
            sum += v;
            count++;
            Average = sum / count;

            if (v > Max) {
                Max = v;
            }
            if (v < Min) {
                Min = v;
            }
        }
    }
}