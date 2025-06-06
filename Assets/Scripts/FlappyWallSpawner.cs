using UnityEngine;

public class FlappyWallSpawner : MonoBehaviour
{
    public GameObject wallPrefab;
    public void Spawn(float speed, bool left, float opening)
    {
        GameObject newWall = Instantiate(wallPrefab, left ? new Vector3(-transform.position.x, transform.position.y, transform.position.z) : transform.position, transform.rotation, GameManager.Instance.bigDaddy.transform);
        newWall.GetComponent<FlappyWall>().Setup(left ? -speed : speed, opening);
    }
}
