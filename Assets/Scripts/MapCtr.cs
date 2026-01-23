using System.Collections.Generic;
using UnityEngine;

public class MapCtr : Singleton<MapCtr>
{
    public Base[] enemyPrefab;
    public Base characterPrefab;
    public List<Base> listEnemys = new();
    public List<Base> listCharacters = new();

}