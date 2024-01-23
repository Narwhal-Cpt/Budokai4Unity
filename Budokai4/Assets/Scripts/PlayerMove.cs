using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerMove : MonoBehaviour
{
    [HideInInspector] public Vector3 velocity;
    [HideInInspector] public CharacterController controller;
    public Transform target;
    Transform camT;
    Animator animator;
    AnimScript animScript;
    public float moveSpeed;
    public float walkSpeed;
    bool running;
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

        if (controller.isGrounded) //using the CharacterController's built-in ground check method and setting movement on the y axis (up and down) to -1 (probably will change considering flying)
        {
            velocity.y = -1;
        }
        controller.Move(velocity * Time.deltaTime); //Applying gravity
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, 0);
        t = Mathf.DeltaAngle(transform.eulerAngles.y, angle); //translates direction held into direction of the character (facing left and holding up on the left stick/D-Pad? Pointing towards the left)
        HandleTimers(direction);
        if(t <= 25 && t >= -25)
        {
            controller.Move(transform.forward * currentSpeed * Time.deltaTime); //holding in the character's forward direction? move forward
        }
        else if(t >= 155 || t <= -155)
        {
            controller.Move(-transform.forward * currentSpeed * Time.deltaTime); //holding in the character's back direction? move backward
        }
        float targetSpeed = (running ? moveSpeed : walkSpeed);
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmooth);

        if(player.GetButtonDown("Guard") && time <= 0.1f) //if direction and up/down pressed at about the same time, initiate side step
        {
            sideStepTime = 0.2f; //how long character will be side stepping
            state = State.SideStepping;
        }
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
            //StopCoroutine(ResetDashTime());
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
                Debug.Log("Double tap");
                running = true;
            }
        }
    }
    IEnumerator ResetDashTime()
    {
        yield return new WaitForSeconds(0.3f);
        firstPressedDirection = false;
        firstTap = 0;
    }

    void LookAtEnemy()
    {
        Vector3 LookT = new Vector3(target.position.x, transform.position.y, target.position.z);
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
}
