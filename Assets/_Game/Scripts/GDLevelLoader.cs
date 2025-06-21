#if UNITY_EDITOR

using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

// Lớp lưu trữ dữ liệu CollectableObject
[System.Serializable]
public class CollectableObjectData
{
    public string id;
    public int level;
    public float scaleFactor;
    public bool isObjective;  // Thêm trường này để lưu giá trị IsObjective
}

[System.Serializable]
public class CollectableObjectDataList
{
    public List<CollectableObjectData> collectables = new List<CollectableObjectData>();
}

public class GDLevelLoader : MonoBehaviour
{
    public string folderPath = "Assets/_Editor/EditorPrefab";

    [HideInInspector]
    public SerializedDictionary<string, GameObject> objectDictionary = new();

    // Đường dẫn mặc định cho file JSON của CollectableObject
    //public string collectableDataPath = "Assets/_Editor/CollectableData.json";

#if UNITY_EDITOR
    /// <summary>
    /// Làm mới dictionary bằng cách đọc toàn bộ prefab GameObject trong folderPath.
    /// Chỉ gọi được trong Editor.
    /// </summary>
    public void RefreshDictionary()
    {
        objectDictionary.Clear();

        // Tìm tất cả GUID của GameObject (prefab) trong folder
        string[] guids = AssetDatabase.FindAssets("t:GameObject", new[] { folderPath });
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            if (go != null)
            {
                if (!objectDictionary.ContainsKey(go.name))
                    objectDictionary.Add(go.name, go);
                else
                    Debug.LogWarning($"Đã có key trùng: {go.name}");
            }
        }

        // Đánh dấu scene hoặc asset này đã thay đổi để lưu lại
        EditorUtility.SetDirty(this);
        Debug.Log($"Đã làm mới dictionary với {objectDictionary.Count} mục từ '{folderPath}'");
    }

    /// <summary>
    /// Kiểm tra và export dữ liệu CollectableObject từ objectDictionary
    /// </summary>
    //public void ExportCollectableData()
    //{
    //    CollectableObjectDataList dataList = new CollectableObjectDataList();

    //    foreach (var kvp in objectDictionary)
    //    {
    //        // Kiểm tra xem GameObject có component CollectableObject không
    //        CollectableObject collectableObj = kvp.Value.GetComponent<CollectableObject>();
    //        if (collectableObj != null)
    //        {
    //            CollectableObjectData data = new CollectableObjectData
    //            {
    //                id = collectableObj.Id,
    //                level = collectableObj.level,
    //                scaleFactor = collectableObj.scaleFactor,
    //                isObjective = collectableObj.IsObjective
    //            };
    //            dataList.collectables.Add(data);
    //            Debug.Log($"Đã thêm collectable: {data.id}, Level: {data.level}, Scale: {data.scaleFactor}, IsObjective: {data.isObjective}");
    //        }
    //    }

    //    // Chuyển đổi thành JSON và lưu vào file
    //    string json = JsonUtility.ToJson(dataList, true); // true để định dạng đẹp
    //    File.WriteAllText(collectableDataPath, json);
    //    AssetDatabase.Refresh();

    //    Debug.Log($"Đã export {dataList.collectables.Count} collectable objects vào {collectableDataPath}");
    //}

    /// <summary>
    /// Import dữ liệu CollectableObject từ file JSON
    /// </summary>
    /// <returns>Dictionary chứa dữ liệu collectable theo id</returns>
    //public Dictionary<string, CollectableObjectData> ImportCollectableData()
    //{
    //    Dictionary<string, CollectableObjectData> result = new Dictionary<string, CollectableObjectData>();

    //    if (File.Exists(collectableDataPath))
    //    {
    //        string json = File.ReadAllText(collectableDataPath);
    //        CollectableObjectDataList dataList = JsonUtility.FromJson<CollectableObjectDataList>(json);

    //        foreach (var data in dataList.collectables)
    //        {
    //            result[data.id] = data;
    //        }

    //        Debug.Log($"Đã import {result.Count} collectable objects từ {collectableDataPath}");
    //    }
    //    else
    //    {
    //        Debug.LogWarning($"Không tìm thấy file JSON tại đường dẫn {collectableDataPath}");
    //    }

    //    return result;
    //}

    /// <summary>
    /// Áp dụng dữ liệu collectable vào objectDictionary
    /// </summary>
    //public void ImportCollectableDataToDictionary()
    //{
    //    Dictionary<string, CollectableObjectData> dataDict = ImportCollectableData();
    //    if (dataDict.Count == 0) return;

    //    int modifiedCount = 0;
    //    List<string> modifiedObjects = new List<string>();

    //    // Lặp qua tất cả các GameObject trong objectDictionary
    //    foreach (var kvp in objectDictionary)
    //    {
    //        GameObject prefab = kvp.Value;
    //        CollectableObject collectableObj = prefab.GetComponent<CollectableObject>();

    //        if (collectableObj != null)
    //        {
    //            // Kiểm tra xem có dữ liệu phù hợp trong file import không
    //            if (dataDict.TryGetValue(collectableObj.Id, out CollectableObjectData data))
    //            {
    //                // Mở prefab để chỉnh sửa
    //                bool success = PrefabUtility.IsPartOfPrefabAsset(prefab);

    //                if (success)
    //                {
    //                    string prefabPath = AssetDatabase.GetAssetPath(prefab);
    //                    GameObject prefabRoot = PrefabUtility.LoadPrefabContents(prefabPath);
    //                    CollectableObject targetCollectable = prefabRoot.GetComponent<CollectableObject>();

    //                    if (targetCollectable != null)
    //                    {
    //                        // Áp dụng dữ liệu từ file import
    //                        Undo.RecordObject(targetCollectable, "Update Collectable Data");
    //                        targetCollectable.Id = data.id;
    //                        targetCollectable.level = data.level;
    //                        targetCollectable.scaleFactor = data.scaleFactor;
    //                        targetCollectable.IsObjective = data.isObjective;

    //                        // Lưu prefab đã sửa đổi
    //                        PrefabUtility.SaveAsPrefabAsset(prefabRoot, prefabPath);
    //                        PrefabUtility.UnloadPrefabContents(prefabRoot);

    //                        modifiedCount++;
    //                        modifiedObjects.Add(data.id);
    //                    }
    //                }
    //                else
    //                {
    //                    Debug.LogWarning($"Không thể chỉnh sửa {prefab.name} vì nó không phải là prefab asset");
    //                }
    //            }
    //        }
    //    }

    //    if (modifiedCount > 0)
    //    {
    //        AssetDatabase.Refresh();
    //        Debug.Log($"Đã import và áp dụng dữ liệu cho {modifiedCount} collectable objects trong dictionary: {string.Join(", ", modifiedObjects)}");
    //    }
    //    else
    //    {
    //        Debug.Log("Không có collectable objects nào được cập nhật");
    //    }
    //}

    /// <summary>
    /// Áp dụng dữ liệu collectable lên các instance trong scene
    /// </summary>
    //public void ApplyCollectableDataToScene()
    //{
    //    Dictionary<string, CollectableObjectData> dataDict = ImportCollectableData();
    //    if (dataDict.Count == 0) return;

    //    // Tìm tất cả CollectableObject trong scene
    //    CollectableObject[] collectables = FindObjectsOfType<CollectableObject>();
    //    int count = 0;

    //    foreach (var collectable in collectables)
    //    {
    //        if (dataDict.TryGetValue(collectable.Id, out CollectableObjectData data))
    //        {
    //            // Áp dụng dữ liệu từ file lên object
    //            Undo.RecordObject(collectable, "Apply Collectable Data");
    //            collectable.level = data.level;
    //            collectable.scaleFactor = data.scaleFactor;
    //            collectable.IsObjective = data.isObjective;
    //            EditorUtility.SetDirty(collectable);
    //            count++;
    //        }
    //    }

    //    Debug.Log($"Đã áp dụng dữ liệu cho {count}/{collectables.Length} collectable objects trong scene");
    //}
#endif

    public TextAsset levelDataTextAsset;

    //public TextAsset mapDataTextAsset;

    public Transform parent;

    //public GameObject SPrefab;
    //public GameObject PPrefab;
    //public GameObject CPrefab;
    //public GameObject ICPrefab;

    [HideInInspector]
    public Text countText;
    private Dictionary<string, int> countById = new();


    [ContextMenu("Load Object")]
    public void LoadObject()
    {
        MapData data = JsonUtility.FromJson<MapData>(levelDataTextAsset.text);

        countById.Clear();

        foreach (LevelData level in data.levelData)
        {
            if (!objectDictionary.TryGetValue(level.id, out GameObject prefab))
            {
                Debug.LogError($"Không tìm thấy prefab với ID: {level.id}");
                continue;
            }

            foreach (ItemData item in level.itemData)
            {
                Vector3 pos = new(item.position.x, item.position.y, item.position.z);
                Vector3 scale = new(item.scale.x, item.scale.y, item.scale.z);
                Vector3 rot = new(item.rotation.x, item.rotation.y, item.rotation.z);

                GameObject obj = Instantiate(prefab, pos, Quaternion.Euler(rot), parent);
                obj.transform.localScale = scale;

                if (obj.TryGetComponent<Rigidbody>(out var rb))
                {
                    rb.isKinematic = true;
                }

                // Áp dụng thông tin CollectableObject nếu có
//                CollectableObject collectable = obj.GetComponent<CollectableObject>();
//                if (collectable != null)
//                {
//#if UNITY_EDITOR
//                    // Nếu có dữ liệu collectable được import, áp dụng chúng
//                    Dictionary<string, CollectableObjectData> dataDict = ImportCollectableData();
//                    if (dataDict.TryGetValue(collectable.Id, out CollectableObjectData cdata))
//                    {
//                        collectable.level = cdata.level;
//                        collectable.scaleFactor = cdata.scaleFactor;
//                        collectable.IsObjective = cdata.isObjective;
//                    }
//#endif
//                }

                if (countById.ContainsKey(level.id))
                    countById[level.id]++;
                else
                    countById[level.id] = 1;
            }
        }

        UpdateCountUI();
    }

    private void UpdateCountUI()
    {
        var sb = new System.Text.StringBuilder();
        foreach (var kvp in countById)
        {
            sb.AppendLine($"ID {kvp.Key}: {kvp.Value}");
        }
        countText.text = sb.ToString();
    }

    //[ContextMenu("Load ground")]
    //public void LoadGround()
    //{
    //    GroundDataRoot groundRoot = JsonUtility.FromJson<GroundDataRoot>(mapDataTextAsset.text);

    //    GroundDataParent parent = groundRoot.Data;

    //    GroundDataGroup dataGroup = parent.MapData;

    //    if (dataGroup.S != null)
    //        foreach (GroundData ground in dataGroup.S)
    //        {
    //            SpawnGround(ground, SPrefab);
    //        }
    //    if (dataGroup.P != null)
    //        foreach (GroundData ground in dataGroup.P)
    //        {
    //            SpawnGround(ground, PPrefab);
    //        }
    //    if (dataGroup.C != null)
    //        foreach (GroundData ground in dataGroup.C)
    //        {
    //            SpawnGround(ground, CPrefab);
    //        }
    //    if (dataGroup.IC != null)
    //        foreach (GroundData ground in dataGroup.IC)
    //        {
    //            SpawnGround(ground, ICPrefab);
    //        }
    //}

    public void SpawnGround(GroundData ground, GameObject prefab)
    {
        Vector3 pos = new(ground.position.x, 0, ground.position.z);
        Vector3 rot = new(0, ground.rotation, 0);
        Vector3 scale = new(ground.scale.x, 1, ground.scale.z);

        GameObject obj = Instantiate(prefab, pos, Quaternion.Euler(rot), parent);
        obj.transform.localScale = scale;
    }

    public void DestroyAllChildren()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(GDLevelLoader))]
public class GDLevelLoaderEditor : Editor
{
    private SerializedProperty collectableDataPathProp;

    private void OnEnable()
    {
        collectableDataPathProp = serializedObject.FindProperty("collectableDataPath");
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GDLevelLoader editorLevelLoader = (GDLevelLoader)target;

        GUILayout.Space(10);
        if (GUILayout.Button("Refresh Dictionary"))
        {
            editorLevelLoader.RefreshDictionary();
        }

        if (editorLevelLoader.levelDataTextAsset != null)
        {
            if (GUILayout.Button("Load Object"))
            {
                CreateUICanvasIfNeeded(editorLevelLoader);
                editorLevelLoader.LoadObject();
            }
        }

        //if (editorLevelLoader.mapDataTextAsset != null)
        //{
        //    if (GUILayout.Button("Load Ground"))
        //    {
        //        editorLevelLoader.LoadGround();
        //    }
        //}

        if (GUILayout.Button("Clear"))
        {
            editorLevelLoader.DestroyAllChildren();
        }

       // // Phần quản lý CollectableObject
       // GUILayout.Space(15);
       // GUILayout.Label("Collectable Object Data Management", EditorStyles.boldLabel);

       // EditorGUILayout.PropertyField(collectableDataPathProp, new GUIContent("JSON Path"));
       // serializedObject.ApplyModifiedProperties();

       // GUILayout.BeginHorizontal();
       //{
       //     editorLevelLoader.ExportCollectableData();
       // }

       // if (GUILayout.Button("Import To Dictionary"))
       // {
       //     editorLevelLoader.ImportCollectableDataToDictionary();
       // }
       // GUILayout.EndHorizontal();

       // if (GUILayout.Button("Import & Apply to Scene"))
       // {
       //     editorLevelLoader.ApplyCollectableDataToScene();
       // }        if (GUILayout.Button("Export Collectable Data"))
 

       // // Thêm nút Browse để chọn file
       // GUILayout.BeginHorizontal();
       // GUILayout.FlexibleSpace();
       // if (GUILayout.Button("Browse JSON File...", GUILayout.Width(150)))
       // {
       //     string currentPath = editorLevelLoader.collectableDataPath;
       //     string directory = Path.GetDirectoryName(currentPath);
       //     string fileName = Path.GetFileName(currentPath);

       //     string path = EditorUtility.SaveFilePanel("Select Collectable Data JSON", directory, fileName, "json");
       //     if (!string.IsNullOrEmpty(path))
       //     {
       //         // Chuyển đổi path tuyệt đối thành path tương đối với project
       //         if (path.StartsWith(Application.dataPath))
       //         {
       //             path = "Assets" + path.Substring(Application.dataPath.Length);
       //         }

       //         collectableDataPathProp.stringValue = path;
       //         serializedObject.ApplyModifiedProperties();
       //     }
       // }
       // GUILayout.EndHorizontal();
    }

    private void CreateUICanvasIfNeeded(GDLevelLoader loader)
    {
        // Nếu chưa có Text để hiển thị số lượng
        if (loader.countText == null)
        {
            // 1) Tạo Canvas
            var canvasGO = new GameObject("AutoCanvas");
            Undo.RegisterCreatedObjectUndo(canvasGO, "Create Canvas");
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();

            // 2) Tạo Text con của Canvas
            var textGO = new GameObject("CountText");
            Undo.RegisterCreatedObjectUndo(textGO, "Create CountText");
            textGO.transform.SetParent(canvasGO.transform, false);

            var text = textGO.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = 50;
            text.alignment = TextAnchor.UpperLeft;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.text = "Loading...";

            // Đặt RectTransform về góc trên bên trái
            var rt = text.rectTransform;
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0, 1);
            rt.anchoredPosition = new Vector2(10, -10);
            rt.sizeDelta = new Vector2(300, 200);

            // 3) Gán reference cho loader và đánh dấu dirty để lưu lại
            loader.countText = text;
            EditorUtility.SetDirty(loader);
        }
    }
}
#endif
#endif