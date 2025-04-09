using UnityEngine;

public class IEdable : MonoBehaviour
{
    public void OnEat()
    {
        Destroy(gameObject);
    }
}
