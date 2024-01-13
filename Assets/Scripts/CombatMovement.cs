using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class CombatMovement : NetworkBehaviour 
{
   [SerializeField] private CharacterController characterController;
    public PlayerController playerController;
    public Animator animator;
    
    public Transform cameraPosition;
    public float walkSpeed = 3f;
    public float sprintSpeed = 8f;
    private float speed;
    public Rigidbody sphere_rb;
    public float gravity = 9.8f;
    private PlayerFighting playerFighting;

    bool comboSwitch= true;
    private void Awake()
    {
        playerFighting = GetComponent<PlayerFighting>();

        playerController = new PlayerController();
        playerController.PlayerCombat.Enable();
        playerController.PlayerCombat.Jump.performed += Jump;
        playerController.PlayerCombat.Sprint.performed += Sprint;
        playerController.PlayerCombat.Attack.performed += Attack;
        speed = walkSpeed;

        cameraPosition = Camera.main.transform;

    }

    NetworkVariable<int> randomNumber = new NetworkVariable<int>(3, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private void Attack(InputAction.CallbackContext obj)
    {
        //playerFighting.Attack();

        if (!IsOwner) { return; }   
        randomNumber.Value = Random.Range(1, 10);
    }

    private void Sprint(InputAction.CallbackContext context)
    {
        if (speed == sprintSpeed) speed = walkSpeed;
        else if(speed == walkSpeed) speed = sprintSpeed;

    //    Debug.Log("sprint");

    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
                //implement!!!!
        }
    }

    void Update()
    {
     // Debug.Log( OwnerClientId + " ->" + randomNumber.Value);


        if (!IsOwner) { return; }

        if (!characterController.isGrounded)
        {
            characterController.Move(Vector3.down * gravity *Time.deltaTime);
        }

        Vector3 currentDirection = PlayerMove();
        PlayerLook(currentDirection);
    }

    void PlayerLook(Vector3 currentDirection)
    {
        if (currentDirection != Vector3.zero) // check that the player is moving
        {
            Quaternion toRotation = Quaternion.LookRotation(currentDirection, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, 10f * Time.deltaTime);
        }

    }

    Vector3 PlayerMove()
    {

        Vector2 playerInput = playerController.PlayerCombat.Move.ReadValue<Vector2>();


        Vector3 movementDirection = AdjustVectorToCamera(playerInput);

        characterController.Move(movementDirection * speed * Time.deltaTime);


        animator.SetFloat("inputY", movementDirection.magnitude*speed);



        return movementDirection;
    }


    Vector3 AdjustVectorToCamera(Vector2 playerInput)
    {
        Vector3 input = new Vector3(playerInput.x, 0f, playerInput.y);

        Vector3 cameraForward = cameraPosition.forward;

        cameraForward.y = 0f;
        cameraForward.Normalize();

        Vector3 cameraRight = Vector3.Cross(cameraForward, Vector3.up);

        Vector3 inputRelativeToCamera = cameraPosition.TransformDirection(input);

        Vector3 movementDirection = Vector3.Project(inputRelativeToCamera, cameraRight) +
           Vector3.Project(inputRelativeToCamera, cameraForward);

        return movementDirection;
    }


    public void MoveEnabDesab(bool state)
    {
        if (state)
        {
            playerController.PlayerCombat.Move.Disable();
        }else playerController.PlayerCombat.Move.Enable();
    }

}
