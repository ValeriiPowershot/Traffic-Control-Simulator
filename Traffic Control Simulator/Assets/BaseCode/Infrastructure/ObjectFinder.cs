using UnityEngine;

namespace BaseCode.Infrastructure
{
    public static class ObjectFinder
    {
        /// <summary>
        /// Finds an object with the specified name in the parent's GameObject hierarchy.
        /// Recursively searches through all child objects.
        /// </summary>
        /// <param name="parent">The parent GameObject to search under.</param>
        /// <param name="objectName">The name of the object to find.</param>
        /// <returns>The found GameObject or null if the object is not found.</returns>
        /// 
        public static GameObject FindObjectInParent(GameObject parent, string objectName)
        {
            if (parent == null)
                return null;

            if (parent.name == objectName)
                return parent;

            foreach (Transform child in parent.transform)
            {
                GameObject foundObject = FindObjectInParent(child.gameObject, objectName);
                if (foundObject != null)
                {
                    return foundObject;
                }
            }

            return null;
        }
    }
}
