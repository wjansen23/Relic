using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class PersistentObjectsSpawner : MonoBehaviour
    {
        [SerializeField] GameObject m_persistentObjectPrefab;             //Prefab that should exist between scenes.

        static bool m_HasSpawned= false;                                  //Used to check if persistentObjects have spawned in scene.  Live and dies with the application.

        private void Awake()
        {
            if (m_HasSpawned) return;
            SpawnPersistentObjects();
            m_HasSpawned = true;
        }

        private void SpawnPersistentObjects()
        {
            GameObject persistentObject = Instantiate(m_persistentObjectPrefab);
            DontDestroyOnLoad(persistentObject);
        }
    }
}
