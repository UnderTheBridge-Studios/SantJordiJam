using UnityEngine;

public class TriggerEatTutorial : MonoBehaviour
{
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Drac")
        {
            GameManager.Instance.ShowTuto(Tutorial.espai);
            GameManager.Instance.dracReference.EnableEat(true);
            Destroy(gameObject);
        }
    }
}
