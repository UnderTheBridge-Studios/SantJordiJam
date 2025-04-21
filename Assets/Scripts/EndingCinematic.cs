using System.Collections;
using UnityEngine;

public class EndingCinematic : MonoBehaviour
{
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Drac" && GameManager.Instance.isLastDay)
        {
            Debug.Log("StartCinematic");
            GameManager.Instance.StopRoseLoop();
            
        }
    }
}
