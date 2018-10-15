using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dw
{
    public interface INoiseFilter
    {

        float Evaluate(Vector3 point);
    }
}