using UnityEngine;

public class HoleCollect : MonoBehaviour
{
    [SerializeField] Hole hole;

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
        MainUI.Ins.OnCollectTarget(Mathf.RoundToInt(other.attachedRigidbody.mass) - 1);

        hole.OnCollectObject(5);
        other.gameObject.SetActive(false);
    }
}
