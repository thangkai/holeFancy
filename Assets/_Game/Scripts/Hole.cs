using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Hole : MonoBehaviour
{
    [Header("-----REFERENCES-----")]
    [SerializeField] HoleMoving holeMoving;
    [SerializeField] HoleUI holeUI;
    [SerializeField] HoleMagnet holeMagnet;
    [SerializeField] CameraFollow selfCamera;
    [SerializeField] ParticleSystem holeVFX;

    [SerializeField] List<HoleLevelStat> holeStatByLevel;

    private int xpRequired;
    private int currentXp;
    private int currentLevel;

    public int Level => currentLevel;

    private void Start()
    {
        InitHole(1);
    }

    private void FixedUpdate()
    {
        holeMoving.MoveOnJoystick();
    }
    public void OnCollectObject(int xp)
    {
        AddXp(xp);

        AudioManager.Ins.PlaySFX(SFXType.Collect, delayBeforeCanPlayAgain: 0.1f);
    }

    public void AddXp(int xp)
    {
        currentXp += xp;

        if (currentXp >= xpRequired)
        {
            currentXp %= xpRequired;

            InitHole(currentLevel + 1);
            holeVFX.Play();
            AudioManager.Ins.PlaySFX(SFXType.LevelUp);
            ShowCompliment();
        }

        holeUI.UpdateXPFill((float)currentXp / xpRequired);
    }

    private void InitHole(int level)
    {
        HoleLevelStat holeLevelState = holeStatByLevel[level - 1];

        transform.DOScale(Vector3.one * (holeLevelState.HoleScale), 0.4f).SetEase(Ease.OutBack);
        holeMoving.ChangeSpeedReferToScale(holeLevelState.CameraScale);

        xpRequired = holeLevelState.XpRequired;
        selfCamera.ScaleUpSmooth(holeLevelState.CameraScale);
        currentLevel = level;
        var gradientColor = new VertexGradient(
            colorTop,  // top left
            colorTop,  // top right
            colorBot,  // bottom left
            colorBot   // bottom right
        );
        complimentText.colorGradient = gradientColor;
    }
    
    
    
    public void LevelUpHoleByBoster(int level)
    {
    Debug .LogError("tang size");

    
    int levelTemp = currentLevel + level;
            
    HoleLevelStat holeLevelState = holeStatByLevel[levelTemp - 1];
            
    transform.DOScale(Vector3.one * (holeLevelState.HoleScale), 0.4f).SetEase(Ease.OutBack);
    holeMoving.ChangeSpeedReferToScale(holeLevelState.CameraScale);
            
    selfCamera.ScaleUpSmooth(holeLevelState.CameraScale);
        DOVirtual.DelayedCall(5f, () =>
        { 
            HoleLevelStat holeLevelState = holeStatByLevel[currentLevel - 1];
            
            transform.DOScale(Vector3.one * (holeLevelState.HoleScale), 0.4f).SetEase(Ease.OutBack);
            holeMoving.ChangeSpeedReferToScale(holeLevelState.CameraScale);
            
            selfCamera.ScaleUpSmooth(holeLevelState.CameraScale);
        }).SetAutoKill(true);








        // var gradientColor = new VertexGradient(
        //     colorTop,  // top left
        //     colorTop,  // top right
        //     colorBot,  // bottom left
        //     colorBot   // bottom right
        // );
        // complimentText.colorGradient = gradientColor;
    }
    
    
    #region TMT code
    [SerializeField] TMP_Text complimentText;
  //  [LunaPlaygroundField("Color Top Compliment Text:", 1, "Game Settings")]
    [SerializeField] Color colorTop;
    //[LunaPlaygroundField("Color Bot Compliment Text:", 1, "Game Settings")]
    [SerializeField] Color colorBot;
    [SerializeField]
    readonly string[] compliments = new string[] {
        "So cute!",
        "Yummy!",
        "Delicious!",
        "Sweet move!",
        "Snack time!",
        "Fruit-tastic!",
        "Aww, adorable!",
        "You're glowing!",
        "Tasty treat!",
        "So smooth!"
    };
    List<string> textTmp = new List<string>();
    public void ShowCompliment()
    {
        if (textTmp.Count <= 0)
            textTmp = compliments.ToList();
        int intRandom = Random.Range(0, textTmp.Count);
        string randomText = textTmp[intRandom];
        complimentText.text = randomText;
        complimentText.gameObject.SetActive(true);

        // Reset to original state
        complimentText.transform.localPosition = new Vector3(0, 0.35f, 0);
        complimentText.transform.localScale = Vector3.zero;
        complimentText.color = new Color(1, 1, 1, 1); // full opacity
        complimentText.DOKill();

        // Pop scale animation
        complimentText.transform.DOScale(1.2f, 0.2f)
            .SetEase(Ease.OutBack)
            .OnComplete(() => complimentText.transform.DOScale(1f, 0.1f));

        // Move up animation
        complimentText.transform
            .DOMoveY(complimentText.transform.position.y + 3f, 1f)
            .SetEase(Ease.OutCubic);

        // Fade out animation
        complimentText.DOFade(0f, 1f)
            .SetDelay(0.5f)
            .OnComplete(() => complimentText.gameObject.SetActive(false));

        textTmp.RemoveAt(intRandom);
    }
    #endregion TMT code
}

[System.Serializable]
public class HoleLevelStat
{
    public int XpRequired;
    public float HoleScale;
    public float CameraScale;
}
