using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CollectableObject : MonoBehaviour
{
    [Header("Remove this serialize later, id will be set on init" +
        "\nThis serialize will be used to generate map data text asset")]
    [SerializeField] private string id;

    private bool isObjective = false;

    public string Id
    {
        get => id;
        set
        {
            id = value;
        }
    }

    public bool IsObjective
    {
        get => isObjective;
        set => isObjective = value;
    }

#if UNITY_EDITOR
    [ContextMenu("Config ID")]
    public void ConfigID()
    {
        id = gameObject.name;
    }

    [Tooltip("Map of hole level to target radius. Define your own values here.")]
    private readonly Dictionary<int, float> levelScaleMap = new Dictionary<int, float>
    {
        { 1, 0.85f * 1.414f },
        { 2, 0.85f * 1.5f * 1.414f },
        { 3, 0.85f * 2.25f * 1.414f },
        { 4, 0.85f * 2.8125f * 1.414f },
        { 5, 0.85f * 3.375f * 1.414f },
        { 6, 0.85f * 3.375f * 1.414f },
        { 7, 0.85f * 4.05f * 1.414f },
        { 8, 0.85f * 4.86f * 1.414f },
        { 9, 0.85f * 5.832f * 1.414f },
        { 10, 0.85f * 6.9984f * 1.414f },
        { 11, 0.85f * 8.39808f * 1.414f },
    };

    [Tooltip("Hole level to use for operations.")]
    public int level = 1;

    [Tooltip("Last computed scale factor based on current level and mesh geometry.")]
    public float scaleFactor = 1f;

    // Lưu trữ tỷ lệ ban đầu của mỗi object con
    private Dictionary<Transform, Vector3> originalScaleRatios = new Dictionary<Transform, Vector3>();

    /// <summary>
    /// Scales and adds a SphereCollider to each active child with a MeshRenderer so it matches the hole radius.
    /// </summary>
    public void FitObjects(int lvl)
    {
        if (lvl < 1 || !levelScaleMap.TryGetValue(lvl, out float targetRadius))
        {
            Debug.LogWarning($"Level {lvl} không có trong levelScaleMap");
            return;
        }

        originalScaleRatios.Clear();
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>(true);
        foreach (MeshRenderer mr in renderers)
        {
            GameObject go = mr.gameObject;
            if (!go.activeSelf) continue;

            MeshFilter mf = go.GetComponent<MeshFilter>();
            if (mf == null || mf.sharedMesh == null) continue;

            // Lưu lại tỷ lệ ban đầu
            Transform t = go.transform;
            Vector3 originalScale = t.localScale;
            float maxDimension = Mathf.Max(originalScale.x, originalScale.y, originalScale.z);
            Vector3 scaleRatio = new Vector3(
                originalScale.x / maxDimension,
                originalScale.y / maxDimension,
                originalScale.z / maxDimension
            );
            originalScaleRatios[t] = scaleRatio;

            Mesh mesh = mf.sharedMesh;
            Vector3 extents = mesh.bounds.extents;
            float localRadius = Mathf.Max(extents.x, extents.y, extents.z);

            SphereCollider sc = go.AddComponent<SphereCollider>();
            sc.center = mesh.bounds.center;
            sc.radius = localRadius;

            Vector3 lossy = go.transform.lossyScale;
            float maxScale = Mathf.Max(lossy.x, lossy.y, lossy.z);
            float worldRadius = localRadius * maxScale;

            float factor = targetRadius / worldRadius;
            factor /= 2f;

            go.transform.localScale *= factor;
            scaleFactor = factor;
        }
    }

    /// <summary>
    /// Applies the last computed scaleFactor while maintaining original scale ratios.
    /// </summary>
    [ContextMenu("Apply")]
    public void ApplyScaleFactor()
    {
        if (scaleFactor == 1)
            return;

        Debug.Log("apply scale " + name);

        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>(true);
        foreach (MeshRenderer mr in renderers)
        {
            if (!mr.gameObject.activeSelf) continue;

            Transform t = mr.transform;
            // Kiểm tra xem đã lưu tỷ lệ ban đầu chưa
            if (originalScaleRatios.TryGetValue(t, out Vector3 ratio))
            {
                // Áp dụng scaleFactor nhưng giữ nguyên tỷ lệ ban đầu
                float baseScale = scaleFactor;
                t.localScale = new Vector3(
                    ratio.x * baseScale,
                    ratio.y * baseScale,
                    ratio.z * baseScale
                );
            }
            else
            {
                // Nếu chưa có thông tin tỷ lệ ban đầu, sử dụng tỷ lệ hiện tại
                Vector3 currentScale = t.localScale;
                float maxDimension = Mathf.Max(currentScale.x, currentScale.y, currentScale.z);
                Vector3 currentRatio = new Vector3(
                    currentScale.x / maxDimension,
                    currentScale.y / maxDimension,
                    currentScale.z / maxDimension
                );

                float baseScale = scaleFactor;
                t.localScale = new Vector3(
                    currentRatio.x * baseScale,
                    currentRatio.y * baseScale,
                    currentRatio.z * baseScale
                );
            }
        }
    }

    [ContextMenu("Remove Receive Shadows")]
    public void RemoveReceiveShadow()
    {
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>(true);
        foreach (MeshRenderer mr in renderers)
        {
            Debug.Log("remove receive shadow " + name);
            mr.receiveShadows = false;
        }
    }
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(CollectableObject))]
public class CollectableObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        CollectableObject script = (CollectableObject)target;

        GUILayout.Space(8);
        if (GUILayout.Button("Rescale In Editor & Remove Colliders"))
        {
            Undo.RecordObject(script, "Rescale Objects");
            script.FitObjects(script.level);

            MeshRenderer[] renderers = script.GetComponentsInChildren<MeshRenderer>(true);
            foreach (var mr in renderers)
            {
                if (!mr.gameObject.activeSelf) continue;
                SphereCollider sc = mr.gameObject.GetComponent<SphereCollider>();
                if (sc != null)
                    DestroyImmediate(sc, true);
            }

            EditorUtility.SetDirty(script);
        }

        GUILayout.Space(8);

        if (GUILayout.Button("Apply Scale Factor"))
        {
            Undo.RecordObject(script, "Apply Scale Factor");
            script.ApplyScaleFactor();
            EditorUtility.SetDirty(script);
        }
    }
}
#endif