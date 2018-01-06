using UnityEngine;

public class DoNotDestroyOnLoad : MonoBehaviour
{
    void Start ()
    {
        DontDestroyOnLoad(gameObject);
    }
}
