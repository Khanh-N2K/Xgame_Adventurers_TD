using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MapCtr : Singleton<MapCtr>
{
    public Base[] enemyPrefab;
    public Base[] characterPrefab;
    public List<Base> listEnemys = new();
    public List<Base> listCharacters = new();
    public Transform characterTransform;
    public Transform enemyTransform;
    private int currentWave = 0;
    public SpawnSO spawnSOData;

    private void Start() {
        StartCoroutine(SpawnEnemyWave(0));
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            SpawnCharacter(0);
        }

        if(Input.GetKeyDown(KeyCode.D))
        {
            SpawnCharacter(1);
        }

        if(Input.GetKeyDown(KeyCode.W))
        {
            SpawnCharacter(2);
        }
    }

    public void SpawnEnemy()
    {
        StartCoroutine(SpawnEnemyWave(currentWave));
    }
    public bool CheckEndWave()
    {
        bool resuit = true;
        for(int i = 0 ; i < listEnemys.Count ; i++)
        {
            if(listEnemys[i].status != Status.Die)
            {
                return false;
            }
        }

        for(int i = 0 ; i < listCharacters.Count ; i++)
        {
            if(listCharacters[i].status != Status.Die)
            {
                return false;
            }
        }

        // tăng wave lên
        currentWave ++;

        return resuit;
    }
    public void SpawnCharacter(int id)
    {
        Base Character = CreateCharacterObject(id, characterTransform);
        listCharacters.Add(Character);
    }


    private Base GetEnemyById(int id)
    {
        for(int i = 0 ; i < enemyPrefab.Length ; i++)
        {
            if(enemyPrefab[i].info.id == id)
            {
                return enemyPrefab[i];
            }
        }
        return null;
    }

    private Base GetCharacterById(int id)
    {
        for(int i = 0 ; i < characterPrefab.Length ; i++)
        {
            if(characterPrefab[i].info.id == id)
            {
                return characterPrefab[i];
            }
        }
        return null;
    }

    public IEnumerator SpawnEnemyWave(int index)
    {
        InfoWave infoWave = spawnSOData.infoSpawn.infoWave[index];
        yield return new WaitForSeconds(infoWave.timeDelay - 13f);
        for(int i = 0 ; i < infoWave.infoTurns.Length ; i++)
        {
            InfoTurn infoTurn = infoWave.infoTurns[i];
            for(int j = 0 ; j < infoTurn.enemy.Length ; j++)
            {
                Base enemy = CreateEnemyObject(infoTurn.enemy[j] , enemyTransform);
                listEnemys.Add(enemy);
            }
            yield return new WaitForSeconds(infoTurn.timedelay);
        }
    }
    

    public Base CreateEnemyObject(int id, Transform parent)
    {
        Base enemy = GetEnemyById(id);
        Instantiate(enemy, parent.transform.position + new Vector3(UnityEngine.Random.Range(-0.05f,0.05f),0.03f,UnityEngine.Random.Range(-0.05f,0.05f)), Quaternion.identity, parent);
        return enemy;
    }

    public Base CreateCharacterObject(int id, Transform parent)
    {
        Base Character = GetCharacterById(id);
        Instantiate(Character, parent.transform.position + new Vector3(UnityEngine.Random.Range(-0.05f,0.05f),0.03f,UnityEngine.Random.Range(-0.05f,0.05f)), Quaternion.identity, parent);
        return Character;
    }

}