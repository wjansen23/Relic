using UnityEngine;

namespace RPG.Core
{
    //This class handles to schedule of the various actions a character can perform.
    public class ActionScheduler : MonoBehaviour
    {
        IAction m_currentAction;                      //The current action being undertaken by the character


        ///////////////////////////// PUBLIC METHODS ////////////////////////////////////////////

        //Starts a character actions
        public void StartAction(IAction action)
        {
            //Check to see if the action is already scheduled
            if (m_currentAction == action) return;

            //Check to make sure action is not null
            if (m_currentAction != null)
            {
                m_currentAction.Cancel();
            }
            m_currentAction = action;
        }

        //Cancel curent action
        public void CancelCurrentAction()
        {
            StartAction(null);
        }
    }
}
