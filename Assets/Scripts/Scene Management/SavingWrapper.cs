using RPG.Saving;
using System.Collections;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {

        SavingSystem m_SaveSystem;                                      //Reference to the saving system

        [SerializeField] float m_FadeInTime = .5f;                      //Time to fade in when loading last scene

        const string m_DefaultSaveFile = "SaveGame";                    //Default save file name

        private void Awake()
        {
            m_SaveSystem = GetComponent<SavingSystem>();
            StartCoroutine(LoadLastScene());
        }

        //This co-routine loads the last scene.
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

        private void Update()
        {
            //Save game when user presses S
            if (Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }

            //Load last save when user pressed L
            if (Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }

            //Delete the savefile
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                DeleteSave();
            }
        }

        ///////////////////////////// PUBLIC METHODS ////////////////////////////////////////////     

        //Public interface for saving the game
        public void Save()
        {
            m_SaveSystem.Save(m_DefaultSaveFile);
        }

        //Public interface for loading the game
        public void Load()
        {
            m_SaveSystem.Load(m_DefaultSaveFile);
        }

        //Delete the current save file
        public  void DeleteSave()
        {
            GetComponent<SavingSystem>().DeleteSavefile(m_DefaultSaveFile);
        }
    }
}
