using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpellBind
{
    public class ObjectPooler : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private int poolCount;

        private void Awake()
        {
            for (int i = 0; i < poolCount; i++)
            {
                GameObject _obj = Instantiate(prefab, transform);
                _obj.SetActive(false);
            }
        }

        public GameObject GetPooledObject()
        {
            foreach(Transform _tr in transform)
            {
                if(!_tr.gameObject.activeSelf)
                    return _tr.gameObject;
            }

            GameObject _obj = Instantiate(prefab, transform);
            return _obj;
        }
    }
}
