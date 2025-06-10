using System.Collections;
using UnityEngine;

public class RodNFish : MonoBehaviour
{
    public static float fishY;
    public GameObject fish;
    public float growSpeed;
    public float fishGrowSpeed;
    public float fishSize;
    public float fishSpd;
    public float fishSrpd;
    public float growLimit;
    public bool spawned = false;
    private void Start()
    {
        Spawn();
    }
    public void Spawn()
    {
        Debug.Log("rr6u7y");
        StartCoroutine(spawn());
    }
    private void Update()
    {
        if(!Player.Instance.initialStop && spawned)
        {
            fish.transform.position += Time.deltaTime * Vector3.up * Mathf.PerlinNoise1D(Time.timeSinceLevelLoad * fishSpd);
            fish.transform.position = new Vector3(fish.transform.position.x, Mathf.Clamp(fish.transform.position.y, -fishSrpd, fishSrpd), fish.transform.position.z);
        }
    }
    private IEnumerator spawn()
    {
        Debug.Log("rr6uy");
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
}
