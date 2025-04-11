using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : Singleton<PoolManager>
{
    #region Pool

    class Pool
    {
        public GameObject OriginalPrefab { get; private set; }
        public Transform Root { get; set; }

        private ObjectPool<Poolable> _pool;

        public void Init(GameObject original, int maxCapacity = 20)
        {
            OriginalPrefab = original;
            Root = new GameObject().transform;
            Root.name = $"{original.name}_Pool";

            _pool = new ObjectPool<Poolable>(
                createFunc: Create
                , actionOnGet: OnGet
                , actionOnRelease: OnRelease
                , actionOnDestroy: OnDestroy
                , defaultCapacity : maxCapacity / 2
                , maxSize: maxCapacity);
        }

        Poolable Create()
        {
            GameObject go = GameObject.Instantiate(OriginalPrefab);
            go.name = OriginalPrefab.name;
            return go.GetOrAddComponent<Poolable>();
        }

        void OnGet(Poolable poolable)
        {
            poolable.gameObject.SetActive(true);
        }

        void OnRelease(Poolable poolable)
        {
            poolable.gameObject.SetActive(false);
            poolable.transform.SetParent(Root);
            //poolable.isUsing = false;
        }

        void OnDestroy(Poolable poolable)
        {
            GameObject.Destroy(poolable.gameObject);
        }

        public Poolable Pop(Transform parent)
        {
            Poolable poolable = _pool.Get();
            if (parent == null) poolable.transform.SetParent(Root);
            else poolable.transform.SetParent(parent);
            //poolable.isUsing = true;
            return poolable;
        }

        public void Push(Poolable poolable)
        {
            Debug.Log("릴리즈호출");
            _pool.Release(poolable);
        }
    }

    #endregion Pool

    private Dictionary<string, Pool> _poolDict = new Dictionary<string, Pool>();
    private Transform _root;

    protected override void Awake()
    {
        base.Awake();

        if (_root == null)
        {
            _root = new GameObject { name = "@Pool_Root" }.transform;
        }
    }

    public void CreatePool(GameObject original, int count = 10)
    {
        Pool pool = new();
        pool.Init(original, count);
        pool.Root.SetParent(_root);
        _poolDict.Add(original.name, pool);
    }

    public Poolable Get(GameObject original, int count = 10, Transform parent = null)
    {//Instantiate 대용
        if (_poolDict.ContainsKey(original.name) == false) CreatePool(original, count);

        return _poolDict[original.name].Pop(parent);
    }

    public void Release(Poolable poolable)
    {//Destroy 대용
        string name = poolable.gameObject.name;

        if (_poolDict.ContainsKey(name) == true) _poolDict[name].Push(poolable);
        else CreatePool(poolable.gameObject);
    }

    public void Clear()
    {
        foreach(Transform child in _root)
        {
            GameObject.Destroy(child.gameObject);
        }

        _poolDict.Clear();
    }
}
