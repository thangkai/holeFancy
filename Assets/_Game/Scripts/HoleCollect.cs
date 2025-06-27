using DG.Tweening;
using UnityEngine;

public class HoleCollect : MonoBehaviour
{
    [SerializeField] Hole hole;
    [SerializeField]private Camera uiCamera;
    [SerializeField] private CoinEffect m_Money_Effect;
    
    
    private void OnTriggerEnter(Collider other)
    {
        //switch (other.attachedRigidbody.mass)
        //{
        //    case 1:
        //        break;
        //    case 2:
        //        break;
        //    case 3:
        //        break;
        //    case 4:
        //        break;
        //}

        
        
        Vector3 start =    other.transform.position;
      

        RectTransform end = MainUI.Ins.ObjectiveUIs[Mathf.RoundToInt(other.attachedRigidbody.mass) - 1];
        // other.transform.DOMove( end  , 0.5f).OnComplete(() =>
        // {
        //
        //     MainUI.Ins.OnCollectTarget(Mathf.RoundToInt(other.attachedRigidbody.mass) - 1);
        //
        //     hole.OnCollectObject(5);
        //     other.gameObject.SetActive(false);
        // });
        
        m_Money_Effect.AddCoins(end, 0, 1, () =>
        {
            MainUI.Ins.OnCollectTarget(Mathf.RoundToInt(other.attachedRigidbody.mass) - 1);
            
            hole.OnCollectObject(5);
            other.gameObject.SetActive(false);
        });
        
    }
    
  

   
}
