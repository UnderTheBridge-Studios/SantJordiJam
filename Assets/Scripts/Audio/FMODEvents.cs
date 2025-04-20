using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [field: Header("Ambience")]
    [field: SerializeField] public EventReference ambience { get; private set; }

    [field: Header("Music")]
    [field: SerializeField] public EventReference music { get; private set; }

    [field: Header("Caja SFX")]
    [field: SerializeField] public EventReference cajaAbrir { get; private set; }
    [field: Header("Quejas SFX")]
    [field: SerializeField] public EventReference quejas { get; private set; }
    [field: Header("Ovejas SFX")]
    [field: SerializeField] public EventReference ovejas { get; private set; }


    public static FMODEvents instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one FMOD Events instance in the scene.");
        }
        instance = this;
    }
}
