using UnityEngine;

public class FlappyWall : MonoBehaviour
{
    public GameObject Up, Down;
    private bool chk = false;
    private float spd = 1;
    public void Setup(float speed, float opening)
    {
        spd = speed;
        Up.GetComponent<Rigidbody2D>().linearVelocity = new Vector3(speed * GameManager.Instance.Speed, 0, 0);
        Down.GetComponent<Rigidbody2D>().linearVelocity = new Vector3(speed * GameManager.Instance.Speed, 0, 0);
        Up.transform.position += new Vector3(0, opening, 0);
        Down.transform.position += new Vector3(0, -opening, 0);
        transform.position += new Vector3(0, Random.Range(-opening, opening) * Random.Range(1f, 1.5f), 0);
    }
    private void Start()
    {
        InvokeRepeating("Check", 1f, 0.1f);
        Invoke("DestroyWall", 7f / GameManager.Instance.Speed);
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
        Up.GetComponent<Rigidbody2D>().linearVelocity = new Vector3(spd * GameManager.Instance.Speed, 0, 0);
        Down.GetComponent<Rigidbody2D>().linearVelocity = new Vector3(spd * GameManager.Instance.Speed, 0, 0);
    }
    private void DestroyWall()
    {
        Destroy(gameObject);
    }
}
