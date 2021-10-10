using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowMotor : MonoBehaviour
{
    public Transform lookAt;
    public Vector3 offset = new Vector3(0, 5.0f, -10.0f);
    private PlayerMotor playerMotor;

    public bool IsMoving { set; get; }

    private void Awake()
    {
        if (IsMoving)
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 5.8f);
        playerMotor = FindObjectOfType<PlayerMotor>();
    }
    private void LateUpdate()
    {
        if (!IsMoving)
            return;
       
        if (playerMotor.isInAir)
        {
            offset = new Vector3(0, 5.0f, 12.0f);
        }
        else
            offset = new Vector3(0, 5.0f, -2.0f);

        Vector3 desiredPosition = lookAt.position + offset;
        desiredPosition.x = 0;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime);
      
    }

    public void BringSnowForward()
    {
        if (IsMoving)
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 5.8f);
    }
}
