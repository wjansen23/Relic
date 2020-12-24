using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPG.Saving
{
    [ExecuteAlways]                                          //Enables execution during editting as well as playing.
    public class SaveableEntity : MonoBehaviour
    {
        [SerializeField] string m_uniqueIdentifier = "";    //Used to uniquely ID a specific saveable game object
        static Dictionary<string, SaveableEntity> m_globalIDLookup = new Dictionary<string, SaveableEntity>();  //Stores all the ID's for saveable entitites

//Ensure this code is not part of the packaged build.
#if UNITY_EDITOR
        private void Update()
        {
            //Do not do anything and return if game is playing
            if (Application.IsPlaying(gameObject)) return;

            //Check to see if object has a path, if not return. To avoid assign ID to prefabs.
            if (string.IsNullOrEmpty(gameObject.scene.path )) return;

            //Finds the serialization of this monobehavoir and returns it to the object.
            SerializedObject serializeObject = new SerializedObject(this);

            //Get the property that is being serialized
            SerializedProperty serializeProperty = serializeObject.FindProperty("m_uniqueIdentifier");

            //Check to see if the unique ID has been set for the savable object.  if not, set it.
            if (string.IsNullOrEmpty(serializeProperty.stringValue) || !IsIdUnique(serializeProperty.stringValue))
            {
                serializeProperty.stringValue = System.Guid.NewGuid().ToString();

                //Need to apply the changes back to the object
                serializeObject.ApplyModifiedProperties();
            }

            //Update global id dictionary
            m_globalIDLookup[serializeProperty.stringValue] = this;
        }
#endif

        //Checks if an entities ID is unique or not.
        private bool IsIdUnique(string canidateID)
        {
            //Check if key already exists in dictionary
            if (!m_globalIDLookup.ContainsKey(canidateID)) return true;

            //Check if key is pointing to self.
            if (m_globalIDLookup[canidateID] == this) return true;

            //check if object that key is assoicated with is null. If so, remove entry from dictionary
            if (m_globalIDLookup[canidateID]==null)
            {
                m_globalIDLookup.Remove(canidateID);
                return true;
            }

            //Check for ID mistmatches.  If so, remove from dictionary.
            if (m_globalIDLookup[canidateID].GetUniqueIdentifier() != canidateID)
            {
                m_globalIDLookup.Remove(canidateID);
                return true;
            }

            return false;
        }

        ///////////////////////////// PUBLIC METHODS ////////////////////////////////////////////   

        //Returns the unique ID for the savable entity
        public string GetUniqueIdentifier()
        {
            return m_uniqueIdentifier;
        }

        //Captures the entities state
        public object CaptureState()
        {
            Dictionary<string, object> state = new Dictionary<string, object>();

            //Find all saveable components and capture their state using their type
            //as the dictionary key.
            foreach (ISaveable saveable in GetComponents<ISaveable>())
            {
                state[saveable.GetType().ToString()] = saveable.CaptureState();  
            }

            return state;
        }

        //Sets the entities state
        public void RestoreState(object state)
        {
            //Cast to dictionary
            Dictionary<string, object> stateDict = (Dictionary<string, object>)state;

            //Loop through all saveable components and restore its state
            foreach (ISaveable saveable in GetComponents<ISaveable>())
            {
                string typeString = saveable.GetType().ToString();

                //Make sure dictionary has a an entry for the component
                if (stateDict.ContainsKey(typeString))
                {
                    saveable.RestoreState(stateDict[typeString]);
                }
            }
        }

    }
}
