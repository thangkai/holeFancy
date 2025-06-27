using DG.Tweening;
using System;
using System.Collections;

using TMPro;
using UnityEngine;


public class CoinEffect : MonoBehaviour
{
    [Header("UI references")]
    [SerializeField] Transform m_Pos_Start;
    [SerializeField] Transform m_Parent_Holder;
    [SerializeField] private TMP_Text coinUIText;
    [SerializeField] private GameObject animatedCoinPrefab;
    [SerializeField] private Transform target;

    [Space]
    [Header("Available coins: ")]
    [SerializeField] private int maxCoins = 10;
    private System.Collections.Generic.Queue<GameObject> coinQueue = new System.Collections.Generic.Queue<GameObject>();

    [Space]
    [Header("Animation settings")]
    [SerializeField] [Range(0.5f, 100f)] private float minAnimationDuration = 0.5f;
    [SerializeField] [Range(0.9f, 100f)] private float maxAnimationDuration = 1f;
    //[SerializeField] [Range(0.5f, 0.9f)] private float minAnimationDuration;
    //[SerializeField] [Range(0.9f, 2f)] private float maxAnimationDuration;
    [SerializeField] private Ease easeType = Ease.InOutQuad;
    [SerializeField] private float spread = 100;
    [SerializeField] private float delayShowTime;

    [SerializeField] private Vector3 targetPosition;

    Action m_Anim_Done;

    private int coinNumber;
    public int CoinNumber
    {
        get => this.coinNumber;
        set
        {
            this.coinNumber = value;
            if (coinUIText != null)
            {
                this.coinUIText.text = this.coinNumber.ToString();
            }
        }
    }

    private void Awake()
    {
        if(target != null)
        {
            this.targetPosition = this.target.position;
        }
        //Prepare pool
        PrepareCoins();
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        Vector3 startPosition = Camera.main.WorldToScreenPoint(Vector3.zero);
    //        AddCoins(startPosition, 0, 100);
    //    }
    //}

    
    
    private void PrepareCoins()
    {
        GameObject coin;
        for (int i = 0; i < this.maxCoins; i++)
        {
            //coin = Instantiate(this.animatedCoinPrefab, transform);
            coin = Instantiate(this.animatedCoinPrefab, m_Parent_Holder == null ? transform : m_Parent_Holder);
            //coin.transform.SetParent(transform);
            coin.SetActive(false);
            this.coinQueue.Enqueue(coin);
        }
    }

   
    [SerializeField] private float timeDown = 0.6f;
    [SerializeField] [Range(0.25f, 100f)] private float minAnimation1 = 0.25f;
    [SerializeField] [Range(0.35f, 100f)] private float maxAnimation2 = 0.35f;

    private IEnumerator Animate(Vector3 collectedCoinPosition, int startCoin, int finalCoin)
    {
        yield return new WaitForSecondsRealtime(this.delayShowTime);
        this.spread = 1;
        int countDownCoin = Mathf.Abs(startCoin - finalCoin) > maxCoins ? maxCoins : Mathf.Abs(startCoin - finalCoin);
        if (countDownCoin <= 0)
        {
            countDownCoin = maxCoins;
        }

        if (startCoin == finalCoin)
        {
            countDownCoin = 1;
        }

        int stepCoinValue = Mathf.Abs(startCoin - finalCoin) / countDownCoin;
        CoinNumber = startCoin;

        for (int i = 0; i < countDownCoin; i++)
        {
            if (this.coinQueue.Count > 0)
            {
                GameObject coin = this.coinQueue.Dequeue();


                //Move coin
                coin.SetActive(true);
                coin.transform.position = collectedCoinPosition +
                                          new Vector3(UnityEngine.Random.Range(-this.spread / 2, this.spread / 2), 0f,
                                              0f);
                // Hiệu ứng shake
                coin.transform.DOMoveY(coin.transform.position.y - 1f, timeDown)
                    .SetUpdate(UpdateType.Normal) // Sử dụng thời gian dựa trên Time.timeScale
                    .SetEase(Ease.InOutQuad).OnComplete(() =>
                    {

                        float duration = UnityEngine.Random.Range(minAnimation1, maxAnimation2);

                        coin.transform.DOMove(this.targetPosition, duration)
                            .SetEase(this.easeType)
                            .SetUpdate(UpdateType.Normal) // Sử dụng thời gian dựa trên Time.timeScale
                            .OnComplete(
                                () =>
                                {
                                    coin.SetActive(false);
                                    this.coinQueue.Enqueue(coin);
                                    countDownCoin -= 1;
                                    if (countDownCoin == 0)
                                    {
                                        CoinNumber = finalCoin;
                                        if (m_Anim_Done != null)
                                        {
                                            m_Anim_Done.Invoke();
                                        }
                                    }
                                    else
                                    {
                                        CoinNumber += stepCoinValue;
                                    }
                                }
                            ) ;

                    } );
                yield return new WaitForSeconds(0.05f);

            }
        }
    }
    
    
    
    
     private IEnumerator AnimatePos(Transform posTarget, Vector3 collectedCoinPosition, int startCoin, int finalCoin)
    {
        yield return new WaitForSecondsRealtime(this.delayShowTime);
        this.spread = 1;
        int countDownCoin = Mathf.Abs(startCoin - finalCoin) > maxCoins ? maxCoins : Mathf.Abs(startCoin - finalCoin);
        if (countDownCoin <= 0)
        {
            countDownCoin = maxCoins;
        }

        if (startCoin == finalCoin)
        {
            countDownCoin = 1;
        }

        int stepCoinValue = Mathf.Abs(startCoin - finalCoin) / countDownCoin;
        CoinNumber = startCoin;

        for (int i = 0; i < countDownCoin; i++)
        {
            if (this.coinQueue.Count > 0)
            {
                GameObject coin = this.coinQueue.Dequeue();


                //Move coin
                coin.SetActive(true);
                coin.transform.position = collectedCoinPosition +
                                          new Vector3(UnityEngine.Random.Range(-this.spread / 2, this.spread / 2), 0f,
                                              0f);
                // Hiệu ứng shake
                // coin.transform.DOMoveY(coin.transform.position.y - 1f, timeDown)
                //     .SetUpdate(UpdateType.Normal) // Sử dụng thời gian dựa trên Time.timeScale
                //     .SetEase(Ease.InOutQuad).OnComplete(() =>
                //     {

                        float duration = UnityEngine.Random.Range(minAnimation1, maxAnimation2);

                        coin.transform.DOMove(posTarget.position, duration)
                            .SetEase(this.easeType)
                            .SetUpdate(UpdateType.Normal) // Sử dụng thời gian dựa trên Time.timeScale
                            .OnComplete(
                                () =>
                                {
                                    coin.SetActive(false);
                                    this.coinQueue.Enqueue(coin);
                                    countDownCoin -= 1;
                                    if (countDownCoin == 0)
                                    {
                                        CoinNumber = finalCoin;
                                        if (m_Anim_Done != null)
                                        {
                                            m_Anim_Done.Invoke();
                                        }
                                    }
                                    else
                                    {
                                        CoinNumber += stepCoinValue;
                                    }
                                }
                            ) ;

             //       } );
                yield return new WaitForSeconds(0f);

            }
        }
    }

    public void AddCoins(Vector3 collectedCoinPosition, int startCoin, int finalCoin, Action anim_Done = null)
    {
        m_Anim_Done = anim_Done;
        //this.coinNumber += amount;
        StartCoroutine(Animate(collectedCoinPosition, startCoin, finalCoin));
    }
    


    public void AddCoins(TMP_Text _coinUIText, Vector3 collectedCoinPosition, int startCoin, int finalCoin, Action anim_Done = null)
    {
        coinUIText = _coinUIText;
        m_Anim_Done = anim_Done;
        //this.coinNumber += amount;
        StartCoroutine(Animate(collectedCoinPosition, startCoin, finalCoin));
    }

 
    public void AddCoins(int startCoin, int finalCoin, Action anim_Done = null)
    {
        m_Anim_Done = anim_Done;
        //this.coinNumber += amount;
        StartCoroutine(Animate(m_Pos_Start.position, startCoin, finalCoin));
    }
    
    
        
    public void AddCoins(Transform target, int startCoin, int finalCoin, Action anim_Done = null)
    {
        m_Anim_Done = anim_Done;
        //this.coinNumber += amount;
        StartCoroutine(AnimatePos(target,m_Pos_Start.position, startCoin, finalCoin));
    }

    public void AddCoins(bool is_UI_In_Camera, TMP_Text _coinUIText, Transform target, int startCoin, int finalCoin, Action anim_Done = null)
    {
        targetPosition = is_UI_In_Camera ? Camera.main.WorldToScreenPoint(target.position) : target.position;
        if(_coinUIText != null)
        {
            coinUIText = _coinUIText;
        }
        m_Anim_Done = anim_Done;



        //this.coinNumber += amount;
        StartCoroutine(Animate(m_Pos_Start.position, startCoin, finalCoin));
    }
}
