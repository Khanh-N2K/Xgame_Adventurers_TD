using System;
using UnityEngine;
[Serializable]
public struct Info
{
    public int id;
    public int heath;
    public int damage;
    public int speed;
    public float attackDelay;
    public float range;
}
[Serializable]
public enum Status
{
    Idle,
    Attack,
    Move,
    Die,
}