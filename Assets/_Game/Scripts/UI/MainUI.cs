using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : Singleton<MainUI>
{
    public List<int> targets = new List<int>();
    public List<TMP_Text> textDisplay = new List<TMP_Text>();
    public List<LevelObjectiveUI> levelObjectiveUIs = new List<LevelObjectiveUI>();
    public TMP_Text txtTimer;
    public Image imgFillTimer;
    public Button playBtn;
    public Button bigButton;
    #region TMT code
    [Header("popup win")]
    [SerializeField] CanvasScaler canvasScaler;
    [SerializeField] GameObject popupWin;
    [SerializeField] GameObject popupLose;
    [SerializeField] Transform mainContentWin;
    [SerializeField] Transform mainContentLose;
    [SerializeField] List<Transform> stars;
    [SerializeField] List<GameObject> starsDisable;
    [SerializeField] float delayStep = 0.4f;
    [SerializeField] Transform btnNext;
    [SerializeField] GameObject arrowHandTut;
    [SerializeField] Transform handTut;
    [SerializeField] int posMove = 200;
    [SerializeField] float lastWidth;
    [SerializeField] float lastHeight;
    [SerializeField] Transform titleWin;
    #endregion TMT code
    private void Awake()
    {
        playBtn.onClick.AddListener(OnPlayButtonClick);
        bigButton.onClick.AddListener(OnPlayButtonClick);
    }
    void Update()
    {
        if (lastWidth == Screen.width && lastHeight == Screen.height) return;
        lastWidth = Screen.width;
        lastHeight = Screen.height;
        if (lastWidth / lastHeight >= 0.65f)
            canvasScaler.matchWidthOrHeight = 1;
        else
            canvasScaler.matchWidthOrHeight = 0;
    }
    public void OnCollectTarget(int index)
    {
        targets[index]--;
        if (targets[index] == 0)
        {
            AudioManager.Ins.PlaySFX(SFXType.Finish);
            textDisplay[index].text = "0";
            levelObjectiveUIs[index].Hide();
            foreach (var item in targets)
            {
                if (item > 0)
                {
                    return;
                }
            }
            //OnPlayButtonClick();
            RunAnimPopupWin();
            GameManager.Ins.PauseGameplay();
        }
        else if (targets[index] < 0)
        {
            return;
        }
        else
        {
            levelObjectiveUIs[index].OnClaimObjective();
            textDisplay[index].text = targets[index].ToString();
        }
    }

    public void UpdateTimerVisual(float currentTimer, float maxTimer)
    {
        txtTimer.text = MinuteSecondTimeFormat(currentTimer);

        imgFillTimer.fillAmount = currentTimer / maxTimer;
    }

    public string MinuteSecondTimeFormat(float timer)
    {
        int minute = Mathf.FloorToInt(timer / 60f);
        int second = Mathf.FloorToInt(timer % 60f);
        return $"{minute:00}:{second:00}";
    }

    public void OnPlayButtonClick()
    {
        //Luna.Unity.Playable.InstallFullGame();
    }

    public void ShowBigButton()
    {
        bigButton.gameObject.SetActive(true);
    }
    #region TMT code
    void RunAnimPopupWin()
    {
      //  Luna.Unity.LifeCycle.GameEnded();
        popupWin.SetActive(true);
        mainContentWin.localScale = Vector3.zero;
        popupWin.transform.DOScale(1, 0.2f).SetUpdate(true).OnComplete(() =>
        {
            AudioManager.Ins.CheckBGM();
            AudioManager.Ins.PlaySFX(SFXType.Win);
        });
        // Hiện main content
        mainContentWin.DOScale(1, 0.5f)
            .SetDelay(0.1f)
            .SetEase(Ease.InExpo)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                // hiệu ứng title
                RunAnimStar();
            });
    }
    void RunAnimStar()
    {
        // Hiệu ứng từng ngôi sao
        for (int i = 0; i < stars.Count; i++)
        {
            Transform star = stars[i];
            GameObject starDisable = starsDisable[i];
            star.gameObject.SetActive(true);
            star.localScale = Vector3.zero;
            float delay = i * delayStep;
            Sequence seq = DOTween.Sequence();
            seq.AppendInterval(delay)
               .AppendCallback(() => star.localScale = Vector3.zero)
               .Append(star.DOScale(1.5f, 0.1f).SetEase(Ease.OutBack))
               .Append(star.DOScale(1f, 0.15f).SetEase(Ease.InCubic))
               .AppendCallback(() =>
               {
                   starDisable.SetActive(false);
                   star.DORotate(new Vector3(0, 0, 360), 1f, RotateMode.FastBeyond360)
                       .SetEase(Ease.OutSine)
                       .SetLoops(1)
                       .SetUpdate(true);
               })
               .SetUpdate(true);
        }
        // Tính thời gian tổng cần chờ trước khi hiện nút
        float waitTime = stars.Count * delayStep + 0.5f; // 0.5 là thời gian tween 1 sao
        DOVirtual.DelayedCall(waitTime, () =>
        {
            ShowNextButtonEffect();
        }, ignoreTimeScale: true);
    }
    void ShowNextButtonEffect()
    {
        ShowBigButton();
        btnNext.gameObject.SetActive(true);
        // Nếu bạn dùng CanvasGroup thì có thể dùng cả fade
        CanvasGroup cg = btnNext.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = 0;
            cg.DOFade(1f, 0.2f).SetEase(Ease.InSine).SetUpdate(true);
        }
        // Scale hiệu ứng bật
        btnNext.localScale = Vector3.zero;
        Sequence btnSeq = DOTween.Sequence();
        btnSeq.Append(btnNext.DOScale(1.2f, 0.2f).SetEase(Ease.OutBack))
              .Append(btnNext.DOScale(1f, 0.1f).SetEase(Ease.InOutSine))
              .SetUpdate(true);
    }
    public void RunAnimLose()
    {
        //Luna.Unity.LifeCycle.GameEnded();
        popupLose.SetActive(true);
        popupLose.transform.DOScale(1, 0.2f).SetUpdate(true).OnComplete(() =>
        {
            AudioManager.Ins.CheckBGM();
            AudioManager.Ins.PlaySFX(SFXType.Fails);
        });
        mainContentLose.DOScale(1, 0.5f)
            .SetDelay(0.1f)
            .SetEase(Ease.InExpo)
            .SetUpdate(true).OnComplete(ShowBigButton);
    }
    public void SetTimeInit()
    {
        txtTimer.text = MinuteSecondTimeFormat(GameManager.Ins.currentTimer);
    }
    public void RunAnimHandTut()
    {
        handTut.DOMoveX(handTut.position.x + posMove, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
    }
    public void SetActiveTut(bool b)
    {
        arrowHandTut.SetActive(b);
    }
    #endregion TMT code
}
