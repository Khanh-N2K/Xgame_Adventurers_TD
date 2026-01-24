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
    private float[] PosZ = new float[11] { -0.125f, -0.1f, -0.075f, -0.05f, -0.025f, 0, 0.025f, 0.05f, 0.075f, 0.1f, 0.125f };

    private void Start()
    {
        StartCoroutine(SpawnEnemyWave(0));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            SpawnCharacter(0);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            SpawnCharacter(1);
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            SpawnCharacter(2);
        }
    }

    public Base GetClosestEnemy(Vector3 root)
    {
        if (listEnemys.Count == 0)
        {
            return null;
        }

        Base closestCharacter = listEnemys[0];
        float minDistance = Vector3.SqrMagnitude(closestCharacter.transform.position - root);
        for (int i = 1; i < listEnemys.Count; i++)
        {
            if (listEnemys[i].status != Status.Die
                && Vector3.SqrMagnitude(listEnemys[i].transform.position - root) < minDistance)
            {
                minDistance = Vector3.SqrMagnitude(listEnemys[i].transform.position - root);
                closestCharacter = listEnemys[i];
            }
        }
        return closestCharacter;
    }

    public Base GetClosestCharacter(Vector3 root)
    {
        if (listCharacters.Count == 0)
        {
            return null;
        }

        Base closestCharacter = listCharacters[0];
        float minDistance = Vector3.SqrMagnitude(closestCharacter.transform.position - root);
        for (int i = 1; i < listCharacters.Count; i++)
        {
            if (listCharacters[i].status != Status.Die
                && Vector3.SqrMagnitude(listCharacters[i].transform.position - root) < minDistance)
            {
                minDistance = Vector3.SqrMagnitude(listCharacters[i].transform.position - root);
                closestCharacter = listCharacters[i];
            }
        }
        return closestCharacter;
    }

    public void SpawnEnemy()
    {
        StartCoroutine(SpawnEnemyWave(currentWave));
    }
    public bool CheckEndWave()
    {
        bool resuit = true;
        for (int i = 0; i < listEnemys.Count; i++)
        {
            if (listEnemys[i].status != Status.Die)
            {
                return false;
            }
        }

        for (int i = 0; i < listCharacters.Count; i++)
        {
            if (listCharacters[i].status != Status.Die)
            {
                return false;
            }
        }

        // tăng wave lên
        currentWave++;

        return resuit;
    }
    public void SpawnCharacter(int id)
    {
        Base Character = CreateCharacterObject(id, characterTransform);
        Character.SwitchStatus(Status.Attack);
        if (Character == null) return;
        listCharacters.Add(Character);
    }

    private Base GetEnemyById(int id)
    {
        for (int i = 0; i < enemyPrefab.Length; i++)
        {
            if (i == id)
            {
                return enemyPrefab[i];
            }
        }
        return null;
    }

    private Base GetCharacterById(int id)
    {
        for (int i = 0; i < characterPrefab.Length; i++)
        {
            if (i == id)
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
        for (int i = 0; i < infoWave.infoTurns.Length; i++)
        {
            InfoTurn infoTurn = infoWave.infoTurns[i];
            for (int j = 0; j < infoTurn.enemy.Length; j++)
            {
                Base enemy = CreateEnemyObject(infoTurn.enemy[j], enemyTransform);
                listEnemys.Add(enemy);
            }
            yield return new WaitForSeconds(infoTurn.timedelay);
        }
    }

    public Base CreateEnemyObject(int id, Transform parent)
    {
        Base enemy = GetEnemyById(id);
        if (enemy == null) return null;
        int idx = Random.Range(0, PosZ.Length);
        Vector3 pos = new Vector3(1f, 0.03f, PosZ[idx]);
        Base newEnemy = Instantiate(enemy, parent);
        newEnemy.transform.position = pos;
        newEnemy.transform.rotation = Quaternion.identity;
        return newEnemy;
    }

    public Base CreateCharacterObject(int id, Transform parent)
    {
        Base Character = GetCharacterById(id);
        if (Character == null) return null;
        int idx = Random.Range(0, PosZ.Length);
        Vector3 pos = new Vector3(-0.65f, 0.03f, PosZ[idx]);
        Base newCharacter = Instantiate(Character, parent);
        newCharacter.transform.position = pos;
        newCharacter.transform.rotation = Quaternion.identity;
        return newCharacter;
    }

    #region ___ CONTEXT MENU ___

    [ContextMenu("Test All Attack")]
    private void TestAllAttack()
    {
        foreach (var enemy in listEnemys)
        {
            enemy.SwitchStatus(Status.Attack);
        }
        foreach (var character in listCharacters)
        {
            character.SwitchStatus(Status.Attack);
        }
    }

    #endregion ___
}