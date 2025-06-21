using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameManager : Singleton<GameManager>
{
    #region TMT code
    [Header("LunaPlaygroundField")]
   // [LunaPlaygroundField("Time Play:", 30, "Game Settings")]
    public float currentTimer;
    bool isRedLine;
    [SerializeField] List<Image> redTimeLeftList;
    bool isPlayBGM;
    #endregion TMT code

    public bool isPlaying = false;
    [SerializeField] bool isLose = false;

    private void Start()
    {
        Application.targetFrameRate = 60;
        MainUI.Ins.SetTimeInit();
        MainUI.Ins.RunAnimHandTut();
    }

    private void Update()
    {
        if (isLose) return;
        if (isPlaying)
        {
            if (currentTimer > 0)
            {
                if (currentTimer <= 10)
                {
                    if (!isRedLine)
                    {
                        isRedLine = true;
                        foreach (var item in redTimeLeftList)
                        {
                            item.gameObject.SetActive(true);
                            item.DOColor(new Color(1, 1, 1, 0), .5f)
                                .SetLoops(-1, LoopType.Yoyo)
                                .SetEase(Ease.Linear);
                        }
                    }
                }
                currentTimer -= Time.deltaTime;
            }
            else
            {
                MainUI.Ins.RunAnimLose();
                currentTimer = 0;
                //MainUI.Ins.OnPlayButtonClick();
                PauseGameplay();
                isLose = true;
            }

            MainUI.Ins.UpdateTimerVisual(currentTimer, 45f);
            if (Input.GetMouseButtonDown(0) && AudioManager.Ins.bgmAudioSource == null)
            {
                AudioManager.Ins.PlaySFX(SFXType.bgm, isBGM: true);
            }
        }
    }

    private void OnEnable()
    {
        // Luna.Unity.LifeCycle.OnPause += PauseGameplay;
        // Luna.Unity.LifeCycle.OnResume -= ResumeGameplay;
    }
    private void OnDisable()
    {
        // Luna.Unity.LifeCycle.OnPause -= PauseGameplay;
        // Luna.Unity.LifeCycle.OnResume -= ResumeGameplay;
    }

    private void ResumeGameplay()
    {
        Time.timeScale = 1f;
    }
    public void PauseGameplay()
    {
        Time.timeScale = 0;
    }
}
