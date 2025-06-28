using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameManager : Singleton<GameManager>
{
    #region TMT code
    [Header("field")]
  
    public float currentTimer;
    bool isRedLine;
    [SerializeField] List<Image> redTimeLeftList;

    [SerializeField] private Hole hole;
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

    public void UseBooster(ItemBoster itemBooster )
    {
        switch (itemBooster)
        {
            case ItemBoster.Magnet:
                // tang ban kinh len 
                break;
            case ItemBoster.Compass :
                break;
            case ItemBoster.SizeUp:
                SizeUp(1);
                break;
            case ItemBoster.BlockBomb:
              
                break;
            case ItemBoster.AddTime:
                AddTime(10);
                break;
            default: break;
        }
    }

    public void UseHoleUp()
    {
        SizeUp(1);
    }

    public void UseTime()
    {
        AddTime(10);
    }

    private void AddTime(int time)
    {
        currentTimer += time;
    }
    private void SizeUp(int level)
    {
       hole.LevelUpHoleByBoster(level);
    }
    
    
    



  
    public void PauseGameplay()
    {
        Time.timeScale = 0;
    }
}
