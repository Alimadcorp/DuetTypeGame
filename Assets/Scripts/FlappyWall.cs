using UnityEngine;

public class FlappyWall : MonoBehaviour
{
    public GameObject Up, Down;
    public void Setup(float speed, float opening)
    {
        Up.GetComponent<Rigidbody2D>().linearVelocity = new Vector3(speed, 0, 0);
        Down.GetComponent<Rigidbody2D>().linearVelocity = new Vector3(speed, 0, 0);
        Up.transform.position += new Vector3(0, opening, 0);
        Down.transform.position += new Vector3(0, -opening, 0);
        transform.position += new Vector3(0, Random.Range(-opening, opening), 0);
    }
    private void Start()
    {
        Invoke("DestroyWall", 7f);
    }
    private void DestroyWall()
    {
        Destroy(gameObject);
    }
}
