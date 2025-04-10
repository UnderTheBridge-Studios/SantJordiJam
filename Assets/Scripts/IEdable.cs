using UnityEngine;

public class IEdable : MonoBehaviour
{
    public void OnEat()
    {
        GameManager.Instance.AnimalEaten();
        Destroy(gameObject);
    }
}
