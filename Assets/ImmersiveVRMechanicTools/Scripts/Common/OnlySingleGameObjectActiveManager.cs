using System;
using System.Collections.Generic;
using System.Linq;
using ImmersiveVRTools.Runtime.Common.Extensions;
using UnityEngine;
using UnityEngine.Serialization;

public class OnlySingleGameObjectActiveManager: MonoBehaviour
{
    [SerializeField] [FormerlySerializedAs("_allGameObjects")] private List<Transform> _allTransforms;
    [SerializeField] private Transform _defaultActive;

    public void SetActiveByName(string name)
    {
        var isGameObjectWithSpecifiedNameFound = false;
        foreach (var t in _allTransforms)
        {
            if (t.name == name)
            {
                t.gameObject.SetActive(true);
                isGameObjectWithSpecifiedNameFound = true;
            }
            else
            {
                t.gameObject.SetActive(false);
            }
        }

        if (!isGameObjectWithSpecifiedNameFound)
        {
            Debug.LogWarning($"GameObject named: '{name}' was not found. Default one was be activated.");
            _defaultActive.gameObject.SetActive(true);
        }
    }

    void Awake()
    {
        foreach (var t in _allTransforms)
        {
            t.gameObject.SetActive(t == _defaultActive);
        }
    }

    void Reset()
    {
        _allTransforms = transform.GetAllChildren().ToList();
    }
}