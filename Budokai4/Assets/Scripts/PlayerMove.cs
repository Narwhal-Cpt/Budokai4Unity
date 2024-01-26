using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using System;

public class PlayerMove : MonoBehaviour
{
    [HideInInspector] public Vector3 velocity;
    [HideInInspector] public CharacterController controller;
    public PlayerMove target;
    Transform camT;
    Animator animator;
    AnimScript animScript;
    public float moveSpeed;
    public float walkSpeed;
    bool running;
    public float flyVerticalSpeed;
    public float currentSpeed;
    public float turnSmooth;
    float speedSmooth;
    float turnSmoothVelocity;
    float speedSmoothVelocity;
    float moveHorizontal;
    float moveVertical;
    [SerializeField] private int playerId = 0;
    [SerializeField] private Player player;
    float t;
    float time;
    float sideStepTime;
    public float dashTime = 5;
    bool firstPressedDirection;
    public float firstTap;
    public bool secondPressedDirection;

    public enum State
    {
        Normal,
        SideStepping,
        Attacking,
        Hurt,
    }
    public State state;

    // Start is called before the first frame update
    void Awake()
    {
        player = ReInput.players.GetPlayer(playerId);
        camT = Camera.main.transform;
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        animScript = GetComponentInChildren<AnimScript>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (state) //changes what will happen depending on state of character
        {
            case State.Normal:
                Move();
                LookAtEnemy();
                break;
            case State.SideStepping:
                SideStep();
                break;
        }
    }

    void Move()
    {
        moveHorizontal = player.GetAxisRaw("MoveHorizontal"); //left stick and D-Pad left and right assigned to a value
        moveVertical = player.GetAxisRaw("MoveVertical"); //same as previous line, but with up and down
        Vector3 direction = new Vector3(moveHorizontal, 0, moveVertical).normalized; //taking those two values and gives it an X and Z value (X for forward and backwards and Z for left and right according to where the character is facing)
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + camT.eulerAngles.y; //targets a certain angle depending on where the chamera is facing (used for movement)

        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, 0);
        t = Mathf.DeltaAngle(transform.eulerAngles.y, angle); //translates direction held into direction of the character (facing left and holding up on the left stick/D-Pad? Pointing towards the left)
        HandleTimers(direction);

        if (t <= 50 && t >= -50)
        {
            controller.Move(transform.forward * currentSpeed * Time.deltaTime); //holding in the character's forward direction? move forward
        }
        else if(t >= 130 || t <= -130)
        {
            controller.Move(-transform.forward * currentSpeed * Time.deltaTime); //holding in the character's back direction? move backward
        }
        HeightCorrection();
        VerticalFlyControl();
        controller.Move(velocity * Time.deltaTime); //Applying gravity
        float targetSpeed = (running ? moveSpeed : walkSpeed);
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmooth);

        if(player.GetButtonDown("Guard") && time <= 0.1f) //if direction and up/down pressed at about the same time, initiate side step
        {
            sideStepTime = 0.2f; //how long character will be side stepping
            state = State.SideStepping;
        }
    }

    void VerticalFlyControl()
    {
        if(!running || moveVertical <= 0.6f && moveVertical >= -0.6f) { velocity.y = 0; return; }

        if(t >= 130 || t <= -130)
        {
            if (moveVertical >= 0.7f)
            {
                velocity.y = flyVerticalSpeed;
            }
            else if (moveVertical <= -0.7f && !controller.isGrounded)
            {
                velocity.y = -flyVerticalSpeed;
            }
        }
    }
    void HeightCorrection()
    {
        float dist = Vector3.Distance(transform.position, target.transform.position);


    }
    void HandleTimers(Vector3 d)//if the player isn't holding a direction on the left stick or D-Pad, the timer stops. If they are, timer will add
    {
        if(d == Vector3.zero)
        {
            time = 0;
            secondPressedDirection = false;

            if(firstTap > 0)
            {
                dashTime += Time.deltaTime;
            }

            if(dashTime >= 0.1f)
            {
                firstPressedDirection = false;
                firstTap = 0;
                running = false;
                dashTime = 0;
            }
        }
        else
        {
            dashTime = 0;
            time += Time.deltaTime;
            if (!firstPressedDirection)
            {
                while(firstTap <= 0.5)
                {
                    firstTap += Time.deltaTime;
                }
                firstPressedDirection = true;
                secondPressedDirection = true;
            }
            else if(firstPressedDirection && !secondPressedDirection)
            {
                running = true;
            }
        }
    }

    void LookAtEnemy()
    {
        Vector3 LookT = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
        transform.LookAt(LookT);
    }

    void SideStep()
    {
        if(sideStepTime > 0) //makes character move as long as sideStepTime is above 0
        {
            if(t >= 80 && t <= 100)
            {
                controller.Move(transform.right * currentSpeed * 1.5f * Time.deltaTime);
            }
            else if(t <= -80 && t >= -100)
            {
                controller.Move(-transform.right * currentSpeed * 1.5f * Time.deltaTime);
            }
            sideStepTime -= Time.deltaTime;
        }
        else
        {
            state = State.Normal;
        }
    }

    public float Altitude()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, -transform.up, out hit);
        return hit.distance;
    }
}