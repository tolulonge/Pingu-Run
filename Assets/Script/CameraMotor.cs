using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotor : MonoBehaviour
{
    public Transform lookAt;
    public Vector3 offset = new Vector3(0, 5.0f, -10.0f);
    public Vector3 rotation = new Vector3(35, 0, 0);
    private PlayerMotor playerMotor;

    public bool IsMoving { set; get; }

    private void Awake()
    {
       
        playerMotor = FindObjectOfType<PlayerMotor>();
    }

    private void LateUpdate()
    {
        if (!IsMoving)
            return;
        
        if (playerMotor.isInAir)
        {
            offset = new Vector3(0, 5.0f, 12.0f);
        }else
            offset = new Vector3(0, 5.0f, -5.0f);
        
        Vector3 desiredPosition = lookAt.position + offset;
        desiredPosition.x = 0;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.Euler(rotation),0.1f);
    }

    public void BringCameraForward()
    {
        if (IsMoving)
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 5.8f);
    }
}
