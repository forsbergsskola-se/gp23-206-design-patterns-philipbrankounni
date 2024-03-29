using System;
using UnityEngine;
using UnityEngine.Pool;

public class Castle : MonoBehaviour
{
    public Projectile Projectile;
    private Transform _target;
    private int _enemyLayerMask;
    private float _currentCooldown;
    private IObjectPool<Projectile> _objectPool;
    
    [SerializeField] private bool collectionCheck = true;
    [SerializeField] private int defaultCapacity = 20;
    [SerializeField] private int maxSize = 100;
    
    const float _maxCooldown = 0.8f;

    private void Awake()
    {
        _objectPool = new ObjectPool<Projectile>(CreateFunc, ActionOnGet, ActionOnRelease, ActionOnDestroy,
            collectionCheck, defaultCapacity, maxSize);
        for (int i = 0; i < 20; i++)
        {
            _objectPool.Release(CreateFunc());
        }
    }

    private void ActionOnDestroy(Projectile obj)
    {
        Destroy(obj.gameObject);
    }

    private void ActionOnRelease(Projectile obj)
    {
        obj.gameObject.SetActive(false);
        obj._totalTime = 0f;
    }

    private void ActionOnGet(Projectile obj)
    {
        obj.gameObject.SetActive(true);
    }

    private Projectile CreateFunc()
    {
        Projectile pewpew = Instantiate(Projectile);
        pewpew.gameObject.SetActive(false);
        pewpew.ObjectPool = _objectPool;
        return pewpew;
    }

    void Start()
    {
        this._enemyLayerMask = LayerMask.GetMask("Enemy");
    }

    // Update is called once per frame
    void Update()
    {
        AcquireTargetIfNecessary();
        TryAttack();
    }

    void AcquireTargetIfNecessary()
    {
        if (this._target == null || !this._target.gameObject.activeInHierarchy);
        {
            this._target = Physics2D.OverlapCircle(this.transform.position, 5f, this._enemyLayerMask)?.transform;
        }
    }

    void TryAttack()
    {
        _currentCooldown -= Time.deltaTime;
        if (this._target == null || !(_currentCooldown <= 0f)) return;
        this._currentCooldown = _maxCooldown;
        Attack();
    }

    void Attack()
    {
        var projectile = _objectPool.Get();
        projectile.transform.SetLocalPositionAndRotation(transform.position, GetTargetDirection());
    }

    Quaternion GetTargetDirection()
    {
        var dir = this._target.transform.position - this.transform.position;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        return Quaternion.AngleAxis(angle, Vector3.forward);
    }
}