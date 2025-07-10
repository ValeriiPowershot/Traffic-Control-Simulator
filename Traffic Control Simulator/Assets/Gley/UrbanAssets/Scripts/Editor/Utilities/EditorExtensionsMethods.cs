using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Gley.UrbanAssets.Editor
{
    public static class EditorExtensionsMethods
    {
        public static T AddComponent<T>(this GameObject go, T toAdd) where T : Component
        {
            return go.AddComponent<T>().GetCopyOf(toAdd) as T;
        }


        public static int EnumToInt<TValue>(this TValue value) where TValue : struct, IConvertible
        {
            if (!typeof(TValue).IsEnum)
            {
                throw new ArgumentException(nameof(value));
            }

            return (int)(object)value;
        }


        static T GetCopyOf<T>(this Component comp, T other) where T : Component
        {
            Type type = comp.GetType();
            if (type != other.GetType()) return null; // type mis-match
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
            PropertyInfo[] pinfos = type.GetProperties(flags);
            foreach (var pinfo in pinfos)
            {
                if (pinfo.CanWrite)
                {
                    try
                    {
                        pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
                    }
                    catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
                }
            }
            FieldInfo[] finfos = type.GetFields(flags);
            foreach (var finfo in finfos)
            {
                finfo.SetValue(comp, finfo.GetValue(other));
            }
            return comp as T;
        }


        public static Transform FindDeepChild(this Transform aParent, string aName)
        {
            Queue<Transform> queue = new Queue<Transform>();
            queue.Enqueue(aParent);
            while (queue.Count > 0)
            {
                var c = queue.Dequeue();
                if (c.name == aName)
                    return c;
                foreach (Transform t in c)
                    queue.Enqueue(t);
            }
            return null;
        }


        public static void SetTag(this GameObject parent, string tag, bool includeChildren = true)
        {
            parent.tag = tag;
            if (includeChildren)
            {
                foreach (Transform trans in parent.transform.GetComponentsInChildren<Transform>(true))
                {
                    trans.gameObject.tag = tag;
                }
            }
        }


        public static void SetLayer(this GameObject parent, int layer, bool includeChildren = true)
        {
            parent.layer = layer;
            if (includeChildren)
            {
                foreach (Transform trans in parent.transform.GetComponentsInChildren<Transform>(true))
                {
                    trans.gameObject.layer = layer;
                }
            }
        }


        public static Vector3 GetLocalPositionRelativeToTopParent(this Transform childTransform, Transform topParent)
        {
            // Initialize the local position to the child's own local position
            Vector3 localPosition = childTransform.localPosition;

            // Traverse up the hierarchy to find the topmost parent
            Transform currentParent = childTransform.parent;
            while (currentParent != topParent)
            {
                // Add the local position of the current child to the overall local position
                localPosition += currentParent.localPosition;

                // Move up the hierarchy
                currentParent = currentParent.parent;
            }

            // The 'localPosition' variable now contains the local position of the child
            // relative to the topmost parent in the hierarchy.
            return localPosition;
        }
    }
}
