using System;
using UnityEngine;

[Serializable]
public struct InfoSpawn
{
    public InfoWave[] infoWave;
}
[Serializable]
public struct InfoWave
{
    public float timeDelay;
    public InfoTurn[] infoTurns;
}
[Serializable]
public struct InfoTurn
{
    public float timedelay;
    public int[] enemy;
}