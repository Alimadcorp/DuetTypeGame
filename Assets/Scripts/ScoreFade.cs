using UnityEngine;

public class ScoreFade : MonoBehaviour
{
    public float speed;
    void Start()
    {
        Invoke("Delete", 1f);
    }
    void Update()
    {
        transform.position += new Vector3(0, speed * Time.deltaTime, 0);
    }
    void Delete()
    {
        Destroy(gameObject);
    }
}
