using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField]
    private SerializedScene sceneToLoad;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log($"Loading scene: {sceneToLoad.SceneName} (index: {sceneToLoad.BuildIndex})");
            SceneManager.LoadScene(sceneToLoad.BuildIndex);
        }
    }
}