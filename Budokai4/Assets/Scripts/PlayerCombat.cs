using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
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
    public int noOfPresses;
    private int playerId = 0;
    private Player player;

    // Start is called before the first frame update
    void Awake()
    {
        player = ReInput.players.GetPlayer(playerId);
        playerMove = GetComponent<PlayerMove>();
        animator = GetComponentInChildren<Animator>();
        animScript = GetComponentInChildren<AnimScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.GetButtonDown("Punch"))
        {
            Attack(0);
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
        }
    }
}
