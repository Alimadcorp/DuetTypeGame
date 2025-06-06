using UnityEngine;

public class DestroyAfter : MonoBehaviour
{
    public float Time = 1f;
    void Start()
    {
        Invoke("End", Time);
    }
    void End()
    {
        Destroy(gameObject);
    }
}
