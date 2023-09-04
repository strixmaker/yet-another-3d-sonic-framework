using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Framework.Global
{
    /// <summary>
    /// Various vector functions used throughout the framework
    /// </summary>
    internal static class VectorUtility
    {
        public static void Separate(this Vector3 input, Vector3 normal, out Vector3 lateral, out Vector3 vertical)
        {
            Vector3 lat = Vector3.ProjectOnPlane(input, normal);
            lateral = lat;
            vertical = input - lat;
        }

        public static Vector3 ComputeInput(Vector3 direction, Vector3 normal, Transform camera)
        {
            return Quaternion.FromToRotation(
                camera.up,
                normal) *
                (camera.rotation * new Vector3(direction.x, 0, direction.y));
        }
    }
}
