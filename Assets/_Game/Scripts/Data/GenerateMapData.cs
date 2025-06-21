#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class GenerateMapData : MonoBehaviour
{
    // Biến nhập level từ Inspector
    public int level = 1;
    // Đường dẫn thư mục xuất file (ví dụ: "Assets/Output/")
    public string outputFolderPath = "Assets/_Game/MapData/NewGenerate";

    /// <summary>
    /// Thu thập dữ liệu từ các đối tượng con có CollectableObject,
    /// nhóm theo id và chuyển thành chuỗi JSON (có định dạng đẹp).
    /// </summary>
    public string GetJsonData()
    {
        // Sử dụng Dictionary để nhóm các ItemData theo id
        Dictionary<string, List<ItemData>> groupedData = new Dictionary<string, List<ItemData>>();
        CollectableObject[] collectableObjects = GetComponentsInChildren<CollectableObject>();

        foreach (CollectableObject obj in collectableObjects)
        {
            if (obj == null)
                continue;

            string id = obj.Id;
            if (!groupedData.ContainsKey(id))
                groupedData.Add(id, new List<ItemData>());

            Transform t = obj.transform;
            Rigidbody rb = obj.GetComponent<Rigidbody>();

            bool isKinematic = false;
            if (rb != null)
                isKinematic = rb.isKinematic;
            else
                Debug.LogWarning("GameObject " + obj.gameObject.name + " không có Rigidbody!");

            ItemData item = new ItemData
            {
                position = t.position,
                rotation = t.eulerAngles, // Sử dụng EulerAngles cho rotation kiểu Vector3
                scale = t.localScale,
                isKinematic = isKinematic
            };

            groupedData[id].Add(item);
        }

        // Xây dựng MapData với mỗi LevelData chứa id và danh sách itemData
        MapData mapData = new MapData();
        List<LevelData> levelDataList = new List<LevelData>();

        foreach (var kvp in groupedData)
        {
            LevelData ld = new LevelData
            {
                id = kvp.Key,
                itemData = kvp.Value.ToArray()
            };
            levelDataList.Add(ld);
        }

        mapData.levelData = levelDataList.ToArray();

        return JsonUtility.ToJson(mapData, true);
    }

    /// <summary>
    /// Xây dựng đường dẫn file theo outputFolderPath và biến level.
    /// Tên file: "Level{level}MapData.json"
    /// </summary>
    public string GetFilePath()
    {
        string fileName = "Level" + level + "MapData.json";
        return Path.Combine(outputFolderPath, fileName);
    }

    /// <summary>
    /// Ghi dữ liệu JSON ra file. Nếu thư mục chưa tồn tại, tạo mới.
    /// </summary>
    public void SaveData()
    {
        string json = GetJsonData();
        string filePath = GetFilePath();

        if (!Directory.Exists(outputFolderPath))
        {
            Directory.CreateDirectory(outputFolderPath);
        }

        File.WriteAllText(filePath, json);
        Debug.Log("Đã xuất file JSON tại: " + filePath);
    }
}

[CustomEditor(typeof(GenerateMapData))]
public class CollectableDataCollectorEditor : Editor
{
    // Cờ để hiển thị các tùy chọn ghi đè (override)
    private bool showOverrideOptions = false;

    public override void OnInspectorGUI()
    {
        // Vẽ các trường mặc định của Inspector
        DrawDefaultInspector();

        // Lấy đối tượng target
        GenerateMapData collector = (GenerateMapData)target;
        // Tính toán đường dẫn file dựa trên level
        string filePath = collector.GetFilePath();

        // Nút Save chính
        if (GUILayout.Button("Save Level Data"))
        {
            if (File.Exists(filePath))
            {
                // Nếu file đã tồn tại, hiển thị các nút override và cancel
                showOverrideOptions = true;
            }
            else
            {
                // Nếu file chưa tồn tại, thực hiện lưu file
                collector.SaveData();
            }
        }

        // Nếu file đã tồn tại, hiển thị thông báo cảnh báo màu vàng và các nút Override/Cancel
        if (showOverrideOptions)
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.normal.textColor = Color.yellow;
            EditorGUILayout.LabelField("File đã tồn tại tại: " + filePath, style);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Override"))
            {
                // Ghi đè file cũ
                collector.SaveData();
                showOverrideOptions = false;
            }
            if (GUILayout.Button("Cancel"))
            {
                // Hủy thao tác ghi đè
                showOverrideOptions = false;
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
#endif