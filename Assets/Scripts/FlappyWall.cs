using UnityEngine;

public class FlappyWall : MonoBehaviour
{
    public GameObject Up, Down;
    private bool chk = false;
    public void Setup(float speed, float opening)
    {
        Up.GetComponent<Rigidbody2D>().linearVelocity = new Vector3(speed, 0, 0);
        Down.GetComponent<Rigidbody2D>().linearVelocity = new Vector3(speed, 0, 0);
        Up.transform.position += new Vector3(0, opening, 0);
        Down.transform.position += new Vector3(0, -opening, 0);
        transform.position += new Vector3(0, Random.Range(-opening, opening) * Random.Range(1f, 1.5f), 0);
    }
    private void Start()
    {
        InvokeRepeating("Check", 1f, 0.1f);
        Invoke("DestroyWall", 7f);
    }
    private void Check()
    {
        if (!chk) { 
            if(Player.Instance.transform.position.x > Up.transform.position.x)
            {
                chk = true;
                GameManager.Instance.AddScore(100, "Went Through Wall");
            }
        }
    }
    private void DestroyWall()
    {
        Destroy(gameObject);
    }
}
