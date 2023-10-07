using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack")]
public class BaseAttack : ScriptableObject
{
    public string stateName;
    public List<BaseAttack> nextOptions;
    /* 0 Neutral Punch
     * 1 Forward Punch
     * 2 Back Punch
     * 3 Neutral Kick
     * 4 Forward Kick
     * 5 Backward Kick
     */
}
