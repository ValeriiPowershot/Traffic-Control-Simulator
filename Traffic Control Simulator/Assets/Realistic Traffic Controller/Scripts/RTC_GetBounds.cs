//----------------------------------------------
//        Realistic Traffic Controller
//
// Copyright © 2014 - 2024 BoneCracker Games
// http://www.bonecrackergames.com
//
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Calculates the total bound size of a gameobject.
/// </summary>
public class RTC_GetBounds {

    /// <summary>
    /// Gets the center bounds extent of object, including all child renderers,
    /// but excluding particles and trails, for FOV zooming effect.
    /// </summary>
    /// <returns>The bounds center.</returns>
    /// <param name="obj">Object.</param>
    public static Vector3 GetBoundsCenter(Transform obj) {

        var renderers = obj.GetComponentsInChildren<Renderer>();

        Bounds bounds = new Bounds();
        bool initBounds = false;

        foreach (Renderer r in renderers) {

            if (!((r is TrailRenderer) || (r is ParticleSystemRenderer))) {

                if (!initBounds) {

                    initBounds = true;
                    bounds = r.bounds;

                } else {

                    bounds.Encapsulate(r.bounds);

                }

            }

        }

        Vector3 center = bounds.center;
        return center;

    }

    public static Bounds GetBounds(GameObject obj) {

        Bounds bounds = new Bounds();

        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();

        if (renderers.Length > 0) {

            //Find first enabled renderer to start encapsulate from it

            foreach (Renderer renderer in renderers) {

                if (renderer.enabled) {

                    bounds = renderer.bounds;
                    break;

                }

            }

            //Encapsulate for all renderers

            foreach (Renderer renderer in renderers) {

                if (renderer.enabled)
                    bounds.Encapsulate(renderer.bounds);

            }

        }

        return bounds;

    }

    /// <summary>
    /// Gets the maximum bounds extent of object, including all child renderers,
    /// but excluding particles and trails, for FOV zooming effect.
    /// </summary>
    /// <returns>The bounds extent.</returns>
    /// <param name="obj">Object.</param>
    public static float MaxBoundsExtent(Transform obj) {

        var renderers = obj.GetComponentsInChildren<Renderer>();

        Bounds bounds = new Bounds();
        bool initBounds = false;

        foreach (Renderer r in renderers) {

            if (!((r is TrailRenderer) || (r is ParticleSystemRenderer))) {

                if (!initBounds) {

                    initBounds = true;
                    bounds = r.bounds;

                } else {

                    bounds.Encapsulate(r.bounds);

                }

            }

        }

        float max = Mathf.Max(bounds.extents.x, bounds.extents.y, bounds.extents.z);
        return max;

    }

    public static MeshFilter GetBiggestMesh(Transform obj) {

        MeshFilter[] mfs = obj.GetComponentsInChildren<MeshFilter>();
        MeshFilter biggestMesh = mfs[0];

        for (int i = 0; i < mfs.Length; i++) {

            if (mfs[i].mesh.bounds.size.magnitude > biggestMesh.mesh.bounds.size.magnitude)
                biggestMesh = mfs[i];

        }

        return biggestMesh;

    }

    public static Bounds TransformBounds(Transform _transform, Bounds _localBounds) {

        var center = _transform.TransformPoint(_localBounds.center);

        // transform the local extents' axes
        var extents = _localBounds.extents;
        var axisX = _transform.TransformVector(extents.x, 0, 0);
        var axisY = _transform.TransformVector(0, extents.y, 0);
        var axisZ = _transform.TransformVector(0, 0, extents.z);

        // sum their absolute value to get the world extents
        extents.x = Mathf.Abs(axisX.x) + Mathf.Abs(axisY.x) + Mathf.Abs(axisZ.x);
        extents.y = Mathf.Abs(axisX.y) + Mathf.Abs(axisY.y) + Mathf.Abs(axisZ.y);
        extents.z = Mathf.Abs(axisX.z) + Mathf.Abs(axisY.z) + Mathf.Abs(axisZ.z);

        return new Bounds { center = center, extents = extents };

    }

}
