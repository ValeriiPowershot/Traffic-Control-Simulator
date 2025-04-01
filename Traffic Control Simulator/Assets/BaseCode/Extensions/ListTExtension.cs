using System.Collections.Generic;
using BaseCode.Logic.Vehicles.Vehicles;
using DG.Tweening;
using UnityEngine;

namespace BaseCode.Extensions
{
    public static class ListTExtension
    {
        public static void SetLocalScales(this List<Transform> target, Vector3 localScale)
        {
            foreach (var star in target)
                star.localScale = localScale;
        }
    }
    
}