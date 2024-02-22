using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.Pool;

public class EnemySpawner : MonoBehaviour
{
    private IObjectPool<Enemy> _enemyPool;
    public Enemy EnemyPrefab;
    private const float _totalCooldown = 2f;
    [SerializeField] private bool collectionCheck = true;
    [SerializeField] private int defaultCapacity = 20;
    [SerializeField] private int maxSize = 100;
    
    private float _currentCooldown;

    private void Awake()
    {
        _enemyPool = new ObjectPool<Enemy>(CreateFunc, ActionOnGet, ActionOnRelease, ActionOnDestroy,
            collectionCheck, defaultCapacity, maxSize);
        for (int i = 0; i < 20; i++)
        {
            _enemyPool.Release(CreateFunc());
        }
    }

    private void ActionOnDestroy(Enemy obj)
    {
        Destroy(obj.gameObject);
    }

    private void ActionOnRelease(Enemy obj)
    {
        obj.gameObject.SetActive(false);
    }

    private void ActionOnGet(Enemy obj)
    {
        obj.gameObject.SetActive(true);
    }

    private Enemy CreateFunc()
    {
        Enemy fiende = Instantiate(EnemyPrefab);
        fiende.gameObject.SetActive(false);
        fiende.ObjectPool = _enemyPool;
        return fiende;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        this._currentCooldown -= Time.deltaTime;
        if (this._currentCooldown <= 0f)
        {
            this._currentCooldown += _totalCooldown;
            SpawnEnemies();
        }
    }


    void SpawnEnemies()
    {
        var maxAmount = Mathf.CeilToInt(Time.timeSinceLevelLoad / 7);
        int amount = Random.Range(maxAmount, maxAmount + 3);
        for (var i = 0; i < amount; i++)
        {
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        var randomPositionX = Random.Range(-6f, 6f);
        var randomPositionY = Random.Range(-6f, 6f);
        Instantiate(this.EnemyPrefab, new Vector2(randomPositionX, randomPositionY), Quaternion.identity);
    }
}
