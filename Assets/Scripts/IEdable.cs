using UnityEngine;

public class IEdable : MonoBehaviour
{
    public virtual void OnEat()
    {
        GameManager.Instance.AnimalEaten();
        Destroy(gameObject);
    }
}
