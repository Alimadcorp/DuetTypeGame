using System.Collections;
using UnityEngine;

public class RodNFish : MonoBehaviour
{
    public static float fishY;
    public GameObject fish;
    public float growSpeed = 0.1f;
    public float fishGrowSpeed = 1f;
    public float fishSize = 1f;
    public float fishSpd = 2f;
    public float fishSrpd = 3f;
    public float moveSpeed = 1f;
    public float growLimit = 2.5f;
    public bool spawned = false;
    public static RodNFish instance;

    private Vector3 initialScale;
    private Vector3 startPos;

    private void Awake()
    {
        instance = this;
        initialScale = fish.transform.localScale;
    }

    public void Spawn(bool Exit)
    {
        StartCoroutine(Exit ? unspawn() : spawn());
    }

    private void Update()
    {
        if (!Player.Instance.initialStop && spawned)
        {
            float noise = Mathf.PerlinNoise1D(Time.timeSinceLevelLoad * fishSpd * moveSpeed);
            Vector3 move = Vector3.up * noise * Time.deltaTime;
            fish.transform.position += move;

            float clampedY = Mathf.Clamp(fish.transform.position.y, -fishSrpd, fishSrpd);
            fish.transform.position = new Vector3(fish.transform.position.x, clampedY, fish.transform.position.z);

            fishY = fish.transform.position.y;
        }
    }
    private IEnumerator spawn()
    {
        while(transform.localScale.y < growLimit)
        {
            transform.localScale += new Vector3(0, Time.deltaTime * growSpeed, 0);
            yield return null; 
        }
        while(fish.transform.localScale.x < fishSize)
        {
            fish.transform.localScale += new Vector3(1, 1, 0) * Time.deltaTime * fishGrowSpeed;
            yield return null;
        }
        spawned = true;

    }
    private IEnumerator unspawn()
    {
        spawned = false;
        while (transform.localScale.y > 0)
        {
            transform.localScale -= new Vector3(0, Time.deltaTime * growSpeed, 0);
            yield return null;
        }
        transform.localScale = new Vector3(transform.localScale.x, 0, transform.localScale.z);
        while (fish.transform.localScale.x > 0)
        {
            fish.transform.localScale -= new Vector3(1, 1, 0) * Time.deltaTime * fishGrowSpeed;
            yield return null;
        }
        fish.transform.localScale = Vector3.zero;
    }
}
