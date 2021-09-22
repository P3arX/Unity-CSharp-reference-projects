using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Player
{
    [SerializeField]
    private CharacterController controller;

    [SerializeField]
    private Transform camHandle, groundCheck, attackPosition, directionIndicator;
    
    [SerializeField]
    private float cameraSpeed;

    [SerializeField]
    private Animator animationController;

    public float gravity = -9.81f;
    public float groundDistance = 0.2f;
    public LayerMask groundMask;
    public float jumpHeight;

    private Vector3 direction;
    private Vector3 velocityForGravity;

    private float forwardInput, sideInput;
    private bool pressedJumpButton, pressedAttackButton, pressedSpell1Button, pressedSpell2Button, pressedSpell3Button, pressingRotateButton;
    private bool isGrounded = true;
    private bool shouldSlide = false;
    private Vector3 hitNormal;

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        animationController.SetFloat("movementSpeed", movementSpeed / 5);

        if (isGrounded && velocityForGravity.y < 0)
        {
            animationController.SetBool("isJumping", false);
            velocityForGravity.y = -2;
        }

        GetInputs();
        direction = (camHandle.forward * forwardInput + camHandle.right * sideInput).normalized;
        animationController.SetBool("isRunning", direction != Vector3.zero);

        if (pressedJumpButton && isGrounded)
        {
            animationController.SetBool("isJumping", true);
            velocityForGravity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }

        if(pressedAttackButton)
            Attack();

        if(pressedSpell1Button && knownSpells[0].Cost <= curEnergy)
        {
            knownSpells[0].Cast(attackPosition);
            PaySpell(knownSpells[0].Cost);
        }
    }

    private void FixedUpdate()
    {
        if(transform.rotation != Quaternion.LookRotation(direction, Vector3.up) && direction != Vector3.zero)
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction, Vector3.up), 10 + (movementSpeed / 2));

        //Creaing a starting and an ending point for the checker capsule
        Vector3 objectCheckerCapsuleStartingPoint = transform.position + (transform.right * -0.15f) + (transform.forward) + new Vector3(0, directionIndicator.localPosition.y, 0);
        Vector3 objectCheckerCapsuleEndingPoint = transform.position + (transform.right * 0.15f) + (transform.forward) + new Vector3(0, directionIndicator.localPosition.y, 0);
        //Check if an object is in front of the player
        if(Physics.OverlapCapsule(objectCheckerCapsuleStartingPoint, objectCheckerCapsuleEndingPoint, 0.2f).Length > 0)
        {
            //Always normalize with this funciton, so it returns between -1 and 1. This function checks if the two given directions are the same.
            //1 = same direction, 0 = perpendicular, -1 = opposite direction
            float difference = Vector3.Dot(direction, transform.forward);
            
            if(difference > 0.5f)
            {
                direction = (camHandle.right * sideInput).normalized;
                direction.x += (1f - hitNormal.y) * hitNormal.x * 0.2f;
                direction.z += (1f - hitNormal.y) * hitNormal.z * 0.2f;
            }
        }

        if(shouldSlide)
        {
            direction.x += (1f - hitNormal.y) * hitNormal.x * 0.2f;
            direction.z += (1f - hitNormal.y) * hitNormal.z * 0.2f;
        }

        if (direction != Vector3.zero)
        {
            controller.Move(direction * movementSpeed * Time.deltaTime);
            camHandle.GetComponent<CameraController>().Move(controller.velocity * Time.deltaTime);
        }

        //Apply gravity
        velocityForGravity.y += gravity * Time.deltaTime;
        controller.Move(velocityForGravity * Time.deltaTime);

        //Check if the object we are against is above the slope limit and is less than 90 degress.
        shouldSlide = (Vector3.Angle (Vector3.up, hitNormal) >= controller.slopeLimit && Vector3.Angle (Vector3.up, hitNormal) < 90);
    }

    private void LateUpdate()
    {
        if(pressingRotateButton)
            camHandle.RotateAround(transform.position, Vector3.up, cameraSpeed  * Input.GetAxis("Mouse X"));
    }

    private void Attack()
    {
        Collider[] hitTargets = Physics.OverlapSphere(attackPosition.position, 0.5f);

        for(int i = 0; i < hitTargets.Length; i++)
            if(hitTargets[i].GetComponent<IDamagable>() != null)
                hitTargets[i].GetComponent<IDamagable>().TakeDamage(2);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(attackPosition.position, 0.5f);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(directionIndicator.position, 0.2f);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        hitNormal = hit.normal;
    }

    private void GetInputs()
    {
        forwardInput = Input.GetAxisRaw("Vertical");
        sideInput = Input.GetAxisRaw("Horizontal");
        pressedJumpButton = Input.GetKeyDown(KeyCode.Space);
        pressedAttackButton = Input.GetMouseButtonDown(0);
        pressedSpell1Button = Input.GetKeyDown(KeyCode.Alpha1);
        pressedSpell2Button = Input.GetKeyDown(KeyCode.Alpha2);
        pressedSpell3Button = Input.GetKeyDown(KeyCode.Alpha3);
        pressingRotateButton = Input.GetMouseButton(1);
    }
}
