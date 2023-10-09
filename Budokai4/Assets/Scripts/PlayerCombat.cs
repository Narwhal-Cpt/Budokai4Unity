using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Rewired.Platforms.Custom.CustomInputSource;

public class PlayerCombat : MonoBehaviour
{
    Transform camT;
    PlayerMove playerMove;
    Animator animator;
    AnimScript animScript;
    public List<BaseAttack> initialComboAttacks;
    public BaseAttack currentAttack;
    public bool canPress;
    public bool isGuarding;
    public bool punchHolding;
    public int punchPresses;
    public bool kickHolding;
    public int kickPresses;
    private int playerId = 0;
    private Player player;
    float time;
    float turnSmoothVelocity;

    // Start is called before the first frame update
    void Awake()
    {
        player = ReInput.players.GetPlayer(playerId);
        playerMove = GetComponent<PlayerMove>();
        animator = GetComponentInChildren<Animator>();
        animScript = GetComponentInChildren<AnimScript>();
        camT = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        float moveHorizontal = player.GetAxisRaw("MoveHorizontal");
        float moveVertical = player.GetAxisRaw("MoveVertical");
        Vector2 direction = new Vector2(moveHorizontal, moveVertical).normalized;
        float targetAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg + camT.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, 0);
        float t = Mathf.DeltaAngle(transform.eulerAngles.y, angle);

        if (direction != Vector2.zero)
        {
            time += Time.deltaTime;
        }
        else
        {
            time = 0;
        }

        if (player.GetButtonDown("Punch"))
        {
            if (time < 0.2f)
            {
                if (t <= 25 && t >= -25)
                {
                    Debug.Log("Forward Punch");
                }
                else if (t >= 155 || t <= -155)
                {
                    Debug.Log("Back Punch");
                }
            }
            else
            {
                Attack(0);
            }
        }
        if (player.GetButtonDown("Kick"))
        {
            if (time < 0.2f)
            {
                if (t <= 25 && t >= -25)
                {
                    Debug.Log("Forward Kick");
                }
                else if (t >= 155 || t <= -155)
                {
                    Debug.Log("Back Kick");
                }
            }
            else
            {
                Attack(3);
            }
        }
    }

    void Attack(int type)
    {
        switch (type)
        {
            case 0:
                if (canPress) { punchPresses++; }
                if (playerMove.state == PlayerMove.State.Attacking) { return; }
                animScript.ChangeAnimationState("PunchJab");
                currentAttack = initialComboAttacks[0];
                break;
            case 3:
                if (canPress) { kickPresses++; }
                break;
        }
    }
}
