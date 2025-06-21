using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelObjectiveUI : MonoBehaviour
{
    private Tween tween;
    public void OnClaimObjective()
    {
        tween = transform.DOScale(1.2f, 0.2f).OnComplete(() =>
        {
            transform.DOScale(1f, 0.1f);
        });
    }

    public void Hide()
    {
        tween.Kill();
        transform.DOScale(0, 1f).SetEase(Ease.InBack).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }
}
