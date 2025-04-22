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
    [field: Header("Start SFX")]
    [field: SerializeField] public EventReference start { get; private set; }
    [field: Header("Barullo SFX")]
    [field: SerializeField] public EventReference barullo { get; private set; }
    [field: Header("Eat SFX")]
    [field: SerializeField] public EventReference eat { get; private set; }
    [field: Header("Hearts SFX")]
    [field: SerializeField] public EventReference hearts { get; private set; }
    [field: Header("Outro SFX")]
    [field: SerializeField] public EventReference outro { get; private set; }
    [field: Header("Yawn SFX")]
    [field: SerializeField] public EventReference yawn { get; private set; }

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
