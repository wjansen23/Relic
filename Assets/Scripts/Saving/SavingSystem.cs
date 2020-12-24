using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.Collections.Generic;
using System.Resources;
using UnityEngine.SceneManagement;
using System.Collections;

namespace RPG.Saving
{
    public class SavingSystem : MonoBehaviour
    {

        string m_FileExtension = ".sav";                                                        //Extension for saved games

        //Creates the save path for save games
        private string GetPathFromSaveFile(string savefile)
        {
            return Path.Combine(Application.persistentDataPath, savefile + m_FileExtension);
        }

        //Deserialize game data from the filestream and return it 
        private Dictionary<string, object> LoadFile(string saveFile)
        {
            string path = GetPathFromSaveFile(saveFile);

            //Check is save file exists at path location
            if (!File.Exists(path))
            {
                //No file exists so create a new game state object and return it
                return new Dictionary<string, object>();
            }

            //Debug.Log("Loading from File: " + path);
            using (FileStream fs = File.Open(path, FileMode.Open))
            {
                //Deserialize game state data from save file.
                BinaryFormatter formatter = new BinaryFormatter();
                return (Dictionary<string, object>)formatter.Deserialize(fs);
            }
        }

        //Restores the state of the game based upon the savefile data
        private void RestoreState(Dictionary<string, object> state)
        {
            //Finds all savable objects in the game and store their state within the 
            //state dictionary.
            foreach (SaveableEntity saveable in FindObjectsOfType<SaveableEntity>())
            {
                string id = saveable.GetUniqueIdentifier();

                //Check if id exists in state dictionary, if so, restore
                if (state.ContainsKey(id))
                {
                    saveable.RestoreState(state[id]);
                }
            }
        }

        //Captures the currect state of the game and returns it.
        private void CaptureState(Dictionary<string, object> state)
        {
            //Finds all savable objects in the game and store their state within the 
            //state dictionary.
            foreach(SaveableEntity saveable in FindObjectsOfType<SaveableEntity>())
            {
                state[saveable.GetUniqueIdentifier()] = saveable.CaptureState();
            }

            //Save a reference to the scene
            state["lastSceneBuildIndex"] = SceneManager.GetActiveScene().buildIndex;
        }

        //Saves game's state data to the save file
        private void SaveFile(string saveFile, object state)
        {
            string path = GetPathFromSaveFile(saveFile);

            //Debug.Log("Saving to File: " + path);
            using (FileStream fs = File.Open(path, FileMode.Create))
            {
                //Save the current state of the game to file.
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fs, state);
            }
        }


        ///////////////////////////// PUBLIC METHODS ////////////////////////////////////////////     

        //Interface for saving the game state
        public void Save(string saveFile)
        {
            //Get the current savefile game data
            Dictionary<string, object> state = LoadFile(saveFile);

            //Merge current state information to savefile game data
            CaptureState(state);

            //Save game data to file.
            SaveFile(saveFile,state);
        }

        //Interface for loading game state 
        public void Load(string saveFile)
        {
            RestoreState(LoadFile(saveFile));
        }

        //Used to load the last scene the player was in prior to restoring the state of the game.
        public IEnumerator LoadLastScence(string saveFile)
        {
            //Get the current savefile game data
            Dictionary<string, object> state = LoadFile(saveFile);

            //Get Scene index and check if we are already in that scene. If not, then load it.
            int sceneIndex = SceneManager.GetActiveScene().buildIndex;

            //Make sure savefile contains sceneIndex key
            if (state.ContainsKey("lastSceneBuildIndex"))
            {
                sceneIndex = (int)state["lastSceneBuildIndex"];
            }
            yield return SceneManager.LoadSceneAsync(sceneIndex);

            //Restore state of entities
            RestoreState(state);
        }

        //Deletes the current savefile.
        public void DeleteSavefile(string filename)
        {
            File.Delete(GetPathFromSaveFile(filename));
        }
    }
}





////Used to load the last scene the player was in prior to restoring the state of the game.
//public IEnumerator LoadLastScence(string saveFile)
//{
//    //Get the current savefile game data
//    Dictionary<string, object> state = LoadFile(saveFile);

//    //Make sure savefile contains sceneIndex key
//    if (state.ContainsKey("lastSceneBuildIndex"))
//    {
//        //Get Scene index and check if we are already in that scene. If not, then load it.
//        int sceneIndex = (int)state["lastSceneBuildIndex"];
//        if (sceneIndex != SceneManager.GetActiveScene().buildIndex)
//        {
//            yield return SceneManager.LoadSceneAsync(sceneIndex);
//        }
//    }

//    //Restore state of entities
//    RestoreState(state);
//}
