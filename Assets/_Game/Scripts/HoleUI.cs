using UnityEngine;
using UnityEngine.UI;

public class HoleUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Image xpFill;

    public void UpdateXPFill(float xpNormalized)
    {
        xpFill.fillAmount = xpNormalized;
    }
}
