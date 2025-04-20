using UnityEngine;

public class Cova : MonoBehaviour
{
    [SerializeField] private Transform m_InteriorCovaRef;
    [SerializeField] private Transform m_ExteriorCovaRef;

    private Vector3 m_InteriorCova;
    private Vector3 m_ExteriorCova;

    public Vector3 interiorCova => m_InteriorCova;
    public Vector3 exteriorCova => m_ExteriorCova;

    private void Start()
    {
        GameManager.Instance.SetCaveReference(this);

        m_InteriorCova = m_InteriorCovaRef.position;
        m_ExteriorCova = m_ExteriorCovaRef.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Drac")
            return;

        GameManager.Instance.EnterCave();
    }
}
