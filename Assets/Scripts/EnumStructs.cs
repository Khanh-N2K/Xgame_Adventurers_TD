using System;
using UnityEngine;
[Serializable]
public struct Info
{
    public int id;
    public int heath;
    public int damage;
    public int speed;
    public int range;
}
[Serializable]
public enum Status
{
    Idle,
    Attack,
    Move,
}