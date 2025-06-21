using System.IO;
using UnityEngine;

public class TestPath : MonoBehaviour
{


    [SerializeField] private SerializedDirectory assetFolder;

    private void Start()
    {
        Debug.Log("Đường dẫn thư mục: " + assetFolder.Path);

#if UNITY_EDITOR
        if (!string.IsNullOrEmpty(assetFolder.Path))
        {
            string[] files = Directory.GetFiles(assetFolder.Path, "*.*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                Debug.Log("Asset trong thư mục: " + file);
            }
        }
#endif
    }
}


