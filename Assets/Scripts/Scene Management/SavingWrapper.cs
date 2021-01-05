using RPG.Saving;
using System.Collections;
using UnityEngine;

namespace RPG.SceneManagement
{
    /// <summary>
    /// This class handles interaction with the saving sytem.
    /// </summary>
    public class SavingWrapper : MonoBehaviour
    {

        SavingSystem m_SaveSystem;                                      //Reference to the saving system

        [SerializeField] float m_FadeInTime = .5f;                      //Time to fade in when loading last scene
        [SerializeField] KeyCode m_SaveKey = KeyCode.S;                 //Keycode to save game
        [SerializeField] KeyCode m_LoadKey = KeyCode.L;                 //Keycode to load save game
        [SerializeField] KeyCode m_DeleteKey = KeyCode.Delete;          //KeyCode to Delete save game

        const string m_DefaultSaveFile = "SaveGame";                    //Default save file name

        private void Awake()
        {
            m_SaveSystem = GetComponent<SavingSystem>();
            StartCoroutine(LoadLastScene());
        }

        /// <summary>
        /// This co-routine loads the last scene.
        /// </summary>
        /// <returns></returns>
        private IEnumerator LoadLastScene()
        {
            //Load the last scene from the savefile on start
            yield return GetComponent<SavingSystem>().LoadLastScence(m_DefaultSaveFile);

            SceneFader fader = FindObjectOfType<SceneFader>();

            //Fade out before loading last scene
            fader.FadeOutImmediately();

            //Fade back in
            yield return fader.SceneFadeIn(m_FadeInTime);
        }

        /// <summary>
        /// Intereact with save file when player presses a key
        /// </summary>
        private void Update()
        {
            if (Input.GetKeyDown(m_SaveKey))
            {
                Save();
            }

            //Load last save when user pressed L
            if (Input.GetKeyDown(m_LoadKey))
            {
                Load();
            }

            //Delete the savefile
            if (Input.GetKeyDown(m_DeleteKey))
            {
                DeleteSave();
            }
        }

        ///////////////////////////// PUBLIC METHODS ////////////////////////////////////////////     

        /// <summary>
        /// Public interface for saving the game
        /// </summary>
        public void Save()
        {
            m_SaveSystem.Save(m_DefaultSaveFile);
        }

        /// <summary>
        /// Public interface for loading the game
        /// </summary>
        public void Load()
        {
            m_SaveSystem.Load(m_DefaultSaveFile);
        }

        /// <summary>
        /// Public interface for deleting the current save file
        /// </summary>
        public void DeleteSave()
        {
            GetComponent<SavingSystem>().DeleteSavefile(m_DefaultSaveFile);
        }
    }
}
