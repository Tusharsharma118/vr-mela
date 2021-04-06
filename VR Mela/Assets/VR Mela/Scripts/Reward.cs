using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Reward",menuName ="Reward")]
public class Reward : ScriptableObject
{
    public GameObject reward;
    public double cost;
    public new string name;
    public bool isBought;

}
