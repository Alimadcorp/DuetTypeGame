using System.Collections;
using UnityEngine;

public class Powerups : MonoBehaviour
{
    public Transform powerupSpawn;
    public Vector3 powerupOff;
    public Transform powerupParent;
    public static Powerups instance;
    public int powerups;
    void Awake()
    {
        instance = this;
    }
    public void AddPowerup(string name, float duration)
    {
        PowerupDisplay display = Instantiate(Resources.Load("Prefabs/Powerup") as GameObject).GetComponent<PowerupDisplay>();
        display.transform.position = powerupSpawn.transform.position;
        display.transform.SetParent(powerupParent, true);
        display.Initiate(duration, name);
        powerupSpawn.transform.position += powerupOff;
        powerups++;
    }
    public void Shift()
    {
        powerups--;
        powerupSpawn.transform.position -= powerupOff;
        StartCoroutine(shift());
    }
    private IEnumerator shift() {
        float t = 0;
        Vector3 target = powerupParent.transform.position - powerupOff;
        while(t < 1)
        {
            t += Time.deltaTime * 2;
            powerupParent.transform.position = Vector3.Lerp(powerupParent.transform.position, target, 0.4f);
            yield return null;
        }
        powerupParent.transform.position = target;
    }
}
