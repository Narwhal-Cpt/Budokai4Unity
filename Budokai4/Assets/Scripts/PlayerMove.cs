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
        switch (state)
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
        moveHorizontal = player.GetAxisRaw("MoveHorizontal");
        moveVertical = player.GetAxisRaw("MoveVertical");
        Vector3 direction = new Vector3(moveHorizontal, 0, moveVertical).normalized;
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + camT.eulerAngles.y;
        Vector2 m = new Vector2(moveHorizontal, moveVertical) * Time.deltaTime;
        Vector2 mDir = m.normalized;

        if (controller.isGrounded)
        {
            velocity.y = -1;
        }
        controller.Move(velocity * Time.deltaTime);
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, 0);
        t = Mathf.DeltaAngle(transform.eulerAngles.y, angle);
        if(direction == Vector3.zero) { time = 0; return; } else { time += Time.deltaTime; }
        if(t <= 25 && t >= -25)
        {
            controller.Move(transform.forward * currentSpeed * Time.deltaTime);
        }
        else if(t >= 155 || t <= -155)
        {
            controller.Move(-transform.forward * currentSpeed * Time.deltaTime);
        }
        float targetSpeed = (running ? moveSpeed : walkSpeed) * mDir.magnitude;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmooth);
        Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
        //controller.Move(moveDir.normalized * currentSpeed * Time.deltaTime);

        if (direction.x <= -0.5f && direction.x >= 0.5f) { return; }

        if(player.GetButtonDown("Guard") && time <= 0.1f)
        {
            sideStepTime = 0.3f;
            state = State.SideStepping;
        }
    }

    void LookAtEnemy()
    {
        Vector3 LookT = new Vector3(target.position.x, transform.position.y, target.position.z);
        transform.LookAt(LookT);
    }

    void SideStep()
    {
        if(sideStepTime > 0)
        {
            if(t >= 80 && t <= 100)
            {
                controller.Move(transform.right * currentSpeed * Time.deltaTime);
            }
            else if(t <= -80 && t >= -100)
            {
                controller.Move(-transform.right * currentSpeed * Time.deltaTime);
            }
            sideStepTime -= Time.deltaTime;
        }
        else
        {
            state = State.Normal;
        }
    }
}
