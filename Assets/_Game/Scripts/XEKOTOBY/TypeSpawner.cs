using UnityEngine;

public class TypeSpawner : MonoBehaviour
{ 
    [SerializeField]
    private SerializedType componentType; // Chọn class Type trong Inspector

    private void Start()
    {
        if (componentType.Type != null)
        {
            GameObject go = new GameObject("SpawnedObject");

            // Thêm component theo kiểu được lưu
            var comp = go.AddComponent(componentType.Type) as Component;

            Debug.Log($"Đã gắn component: {componentType.Type.FullName}");
            
        }
        else
        {
            Debug.LogWarning("Chưa chọn component type.");
        }
    }
}