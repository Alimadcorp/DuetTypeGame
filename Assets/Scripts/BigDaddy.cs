using UnityEngine;

public class BigDaddy : MonoBehaviour
{
    public static Vector3 moveDirection = Vector3.zero;
    void FixedUpdate()
    {
        if(moveDirection.sqrMagnitude > 0)
        {
            transform.position += moveDirection * Time.fixedDeltaTime * GameManager.Instance.Speed * GameManager.Instance.speedMultiplier;
        }
    }
}
