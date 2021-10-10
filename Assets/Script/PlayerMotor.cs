using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    private const float LANE_DISTANCE = 2.5f;
    private const float TURN_SPEED = 0.05f;

    private Rigidbody playerRb;

    public bool isRunning = false;
    // Animation
    private Animator anim;


    //Movement
    private CharacterController controller;
    private float jumpForce = 4.0f;
    private float gravity = 12.0f;
    private float verticalVelocity;
    public int desiredLane = 1; // 0 = left, 1 = Middle, 2 = Right

    // Speed Modifier
    private float originalSpeed = 7.0f;
    private float speed;
    private float speedIncreaseLastTick;
    private float speedIncreaseTime = 2.5f;
    private float speedIncreaseAmount = 0.1f;
    public bool isPowerUpActive;
    public bool isInAir;

    private bool shouldMoveForward;

    public GameObject shieldPowerUpIndidicator;
    public GameObject jetpackPowerUpIndicator;

    public bool hasShieldPowerup;
    private GameObject gameObjj;

    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        speed = originalSpeed;
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }

    private bool FlyPowerUpActive()
    {
        return true;
    }

    // Update is called once per frame
    void Update()
    {
        shieldPowerUpIndidicator.transform.position = transform.position + new Vector3(0, 0.66f, 0);
        jetpackPowerUpIndicator.transform.position = transform.position + new Vector3(0, 2.32f, 0.07f);


        if (!isRunning)
        {
            return;
        }

        if(Time.time - speedIncreaseLastTick > speedIncreaseTime)
        {
            speedIncreaseLastTick = Time.time;
            speed += speedIncreaseAmount;
            GameManager.Instance.UpdateModifier(speed - originalSpeed);
        }


        // Gather inputs on which lane we should be
        if (MobileInput.Instance.SwipeLeft)
            MoveLane(false);
        if (MobileInput.Instance.SwipeRight)
            MoveLane(true);


        // Calculate where we should be in the future
        Vector3 targetPosition = transform.position.z * Vector3.forward;

        if (desiredLane == 0)
            targetPosition += Vector3.left * LANE_DISTANCE;
        if (desiredLane == 2)
            targetPosition += Vector3.right * LANE_DISTANCE;

        // Move delta
        Vector3 moveVector = Vector3.zero;
        moveVector.x = (targetPosition - transform.position).normalized.x * speed;

        bool isGrounded = IsGrounded();
        anim.SetBool("Grounded", isGrounded);

        // Calculate Y
        if (isGrounded)
        {
            verticalVelocity = -0.1f;
            

            if (MobileInput.Instance.SwipeUp)
            {
                // Jump
                anim.SetTrigger("Jump");
                verticalVelocity = jumpForce;
            }
            else if (MobileInput.Instance.SwipeDown)
            {
                // Slide
                StartSliding();
                Invoke("StopSliding", 1.0f);
            }
        }
        else
        {
            verticalVelocity -= (gravity * Time.deltaTime);

            // Fast falling mechanic
            if (MobileInput.Instance.SwipeDown)
            {
                verticalVelocity = -jumpForce;
            }
        }


        if (isPowerUpActive)
        {
            if (transform.position.y > 12f)
            {
                verticalVelocity = 0.5f;
                speed = 15;
                isInAir = true;
            }else
           
            verticalVelocity = jumpForce;
            StartCoroutine(PowerupCountdownRoutine());
            
        }
    




        moveVector.y = verticalVelocity;
        moveVector.z = speed;

        // Move the Pengu
        controller.Move(moveVector * Time.deltaTime);

        // Rotate the Pengu to where he is going
        Vector3 dir = controller.velocity;

        if(dir != Vector3.zero)
        {
            dir.y = 0;
            transform.forward = Vector3.Lerp(transform.forward, dir, TURN_SPEED);
        }
        
    }

    IEnumerator PowerupCountdownRoutine()
    {
        yield return new WaitForSeconds(15);
        isPowerUpActive = false;
        isInAir = false;
       // transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 4f);
        speed = originalSpeed;
        jetpackPowerUpIndicator.gameObject.SetActive(isPowerUpActive);
    }


    private void MoveLane(bool goingRight) {
        desiredLane += (goingRight) ? 1 : -1;
        desiredLane = Mathf.Clamp(desiredLane, 0, 2);
    }

    private bool IsGrounded()
    {
        Ray groundRay =new Ray(new Vector3(
            controller.bounds.center.x,(controller.bounds.center.y - controller.bounds.extents.y)
            + 0.2f, controller.bounds.center.z), Vector3.down);
        Debug.DrawRay(groundRay.origin, groundRay.direction, Color.cyan, 1.0f);

        return Physics.Raycast(groundRay, 0.2f + 0.1f);
        
            
    }

    public void StartRunning()
    {
        isRunning = true;
        anim.SetTrigger("StartRunning");
    }

    private void StartSliding()
    {
        anim.SetBool("Sliding", true);
        controller.height /= 2;
        controller.center = new Vector3(controller.center.x, controller.center.y / 2, controller.center.z);

    }

    public void handleTutorialSlide()
    {
        StartSliding();
        Invoke("StopSliding", 1.0f);
    }

    public void handleTutorialJump()
    {
        anim.SetTrigger("Jump");
        verticalVelocity = jumpForce;
    }

    private void StopSliding()
    {
        anim.SetBool("Sliding", false);
        controller.height *= 2;
        controller.center = new Vector3(controller.center.x, controller.center.y * 2, controller.center.z);
    }

    private void Crash()
    {
        anim.SetTrigger("Death");
        isRunning = false;
        GameManager.Instance.OnDeath();
    }

    public void ShieldReset()
    {

        transform.position = new Vector3(transform.position.x, transform.position.y + 4f, transform.position.z + 4f);
        hasShieldPowerup = false;
        shieldPowerUpIndidicator.gameObject.SetActive(hasShieldPowerup);
    }

    public void ResetPosition()
    {
        StartRunning();
        anim.SetTrigger("Runn");
        gameObjj.SetActive(false);
    }


    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        switch (hit.gameObject.tag)
        {
            case "Obstacle":
                if (hasShieldPowerup)
                {
                    ShieldReset();
                    
                }
                else
                {
                    Crash();
                    gameObjj = hit.gameObject.transform.parent.gameObject;
                }
              
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ShieldPowerup"))
        {
            hasShieldPowerup = true;
            shieldPowerUpIndidicator.gameObject.SetActive(hasShieldPowerup);
            Destroy(other.gameObject);
            StartCoroutine(ShieldPowerupCountdownRoutine());
        }

        if (other.CompareTag("JetpackPowerup"))
        {
            isPowerUpActive = true;
            jetpackPowerUpIndicator.gameObject.SetActive(isPowerUpActive);
            Destroy(other.gameObject);
            StartCoroutine(ShieldPowerupCountdownRoutine());
        }

    }

    IEnumerator ShieldPowerupCountdownRoutine()
    {
        yield return new WaitForSeconds(20);
        hasShieldPowerup = false;
        shieldPowerUpIndidicator.gameObject.SetActive(hasShieldPowerup);
    }


}
