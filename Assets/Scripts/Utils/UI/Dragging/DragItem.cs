using UnityEngine;
using UnityEngine.EventSystems;
using RPG.Control;
using static UnityEngine.EventSystems.PointerEventData;

namespace RPG.Core.UI.Dragging
{
    /// <summary>
    /// Allows a UI element to be dragged and dropped from and to a container.
    /// 
    /// Create a subclass for the type you want to be draggable. Then place on
    /// the UI element you want to make draggable.
    /// 
    /// During dragging, the item is reparented to the parent canvas.
    /// 
    /// After the item is dropped it will be automatically return to the
    /// original UI parent. It is the job of components implementing `IDragContainer`,
    /// `IDragDestination and `IDragSource` to update the interface after a drag
    /// has occurred.
    /// <typeparam name="T">The type that represents the item being dragged.</typeparam>
    /// </summary>
    public class DragItem<T> : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler where T : class
    {
        Vector3 m_startPosition;        //Starting position of the item being dragged
        Transform m_originalParent;    //Reference to parent object for the drag item
        IDragSource<T> m_source;        //Reference to the source object for the drag item
        Canvas m_parentCanvas;         //Reference to the parent canvas for the drag item

        ///////////////////////////// INTERFACES //////////////////////////////////////////// 

        /// <summary>
        /// Starts the process of allowing an item to be dragged.
        /// Preserves starting position and parent.
        /// </summary>
        /// <param name="eventData"></param>
        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            m_startPosition = transform.position;
            m_originalParent = transform.parent;

            //Need to disable otherwise we won't get the drop event
            GetComponent<CanvasGroup>().blocksRaycasts = false;

            //Reparent to root canvas so tha it can be dragged around the screen.
            transform.SetParent(m_parentCanvas.transform, true);

            //Get the player controller and disable
            //PlayerController PlayerControlComp = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            //PlayerControlComp.enabled = false;
        }

        /// <summary>
        /// Updates the item's position to where the mouse is currently pointing
        /// </summary>
        /// <param name="eventData"></param>
        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            transform.position = eventData.position;
        }

        /// <summary>
        /// Once dragging is complete.  Reset item back to its parent container and check to see where item ended on.
        /// if another container then drop item into container
        /// otherwise drop item into the world.
        /// </summary>
        /// <param name="eventData"></param>
        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            //Put icon being dragged back to its original position and let the source deal with updating the image.
            //This is done to preseve the layout of gameobjects in the inventory.
            transform.position = m_startPosition;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            transform.SetParent(m_originalParent, true);

            IDragDestination<T> container;
            //Determine if the mouse is over some UI. 
            //If so get drag destination container.
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                //Special case for dropping items into the world
                container = m_parentCanvas.GetComponent<IDragDestination<T>>();
            }
            else
            {
                //Over the UI
                container = GetContainer(eventData);
            }

            //We found a drag destination then remove item from source and add to destination
            if (container != null)
            {
                DropItemIntoContainer(container,eventData.button);
            }

            //Get the player controller and disable
            //PlayerController PlayerControlComp = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            //PlayerControlComp.enabled = true;
        }

        ///////////////////////////// PRIVATE METHODS //////////////////////////////////////////// 

        private void Awake()
        {
            //Set the source and the parent canvas for the drag item
            m_parentCanvas = GetComponentInParent<Canvas>();
            m_source = GetComponentInParent<IDragSource<T>>();  //Container where the item is coming from.
        }

        /// <summary>
        /// Returns a DragDestination container if one exists where the mouse is pointing.
        /// </summary>
        /// <param name="eventData"></param>
        /// <returns></returns>
        private IDragDestination<T> GetContainer(PointerEventData eventData)
        {
            //Make sure the mouse is pointing to something
            if (eventData.pointerEnter)
            {
                //Mouse is pointing to a valid object, get its container via DragDestination.
                var container = eventData.pointerEnter.GetComponentInParent<IDragDestination<T>>();
                return container;
            }
            return null;
        }

        private void DropItemIntoContainer(IDragDestination<T> destination, InputButton button)
        {
            //check if destination is the same as source.  If so, do nothing and return
            if (object.ReferenceEquals(destination, m_source)) return;

            var destinationContainer = destination as IDragContainer<T>;    //Cast destination to drag container
            var sourceContainer = m_source as IDragContainer<T>;            //Cast source to drag container

            //Check if swap is not possible. Must satisfy one of the following:
            //Either the source or destination are not containers
            //Destination is a container but does not contain and item (i.e. its empty)
            //Destination and source are both containers and have the same item
            if (destinationContainer == null || sourceContainer == null || destinationContainer.GetItem() == null || object.ReferenceEquals(destinationContainer.GetItem(), sourceContainer.GetItem()))
            {
                //Can't swap but try to move item from source to destination.
                AttemptSimpleTransfer(destination, button);
                return;
            }

            AttemptSwap(destinationContainer, sourceContainer);
        }

        /// <summary>
        /// Attempt to move item from to the destination
        /// </summary>
        /// <param name="destination"></param>
        /// <returns></returns>
        private bool AttemptSimpleTransfer(IDragDestination<T> destination,InputButton button)
        {
            //Debug.Log("In simple transfer");
            //Get the current item and number from the source container
            var draggingItem = m_source.GetItem();
            var draggingNumber = 0;
            var maxCanReceive = destination.MaxAcceptable(draggingItem);

            if (button == InputButton.Left)
            {
                draggingNumber = m_source.GetNumber();
            }
            else
            {
                draggingNumber = 1;
            }

            //Determine how many of the item can be transfered based on the destination container
            //var maxCanReceive = destination.MaxAcceptable(draggingItem);
            var numToTransfer = Mathf.Min(maxCanReceive, draggingNumber);

            //If there are items to be transfered, descrease count in source and add # of items to destination.
            if (numToTransfer > 0)
            {
                m_source.RemoveItems(numToTransfer);
                destination.AddItems(draggingItem, numToTransfer);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Methods attempts to swap the items in the destination and source container.
        /// </summary>
        /// <param name="destinationContainer"></param>
        /// <param name="sourceContainer"></param>
        private void AttemptSwap(IDragContainer<T> destinationContainer, IDragContainer<T> sourceContainer)
        {
            //Debug.Log("In SWAP");
            //Get the item and its number for both the source and destination containers
            //This is so we know what to swap
            var removedSourceNum = sourceContainer.GetNumber();
            var removedSourceItem = sourceContainer.GetItem();
            var removeDestinationNum = destinationContainer.GetNumber();
            var removeDestinationItem = destinationContainer.GetItem();

            //Debug.Log("S#:: " + removedSourceNum + " D#::" + removeDestinationNum);

            //Perform a series of checks to make sure there are items to swap.
            if (removeDestinationNum == 0||removedSourceNum==0) return; ///Added to account either the dest or source not having any items.
            if (destinationContainer.MaxAcceptable(removedSourceItem) < 1) return; ///No need to swap if destination cannot accept source item
            if (sourceContainer.MaxAcceptable(removeDestinationItem) < 1) return; ///No need to swap if source cannot accept destination item

            //Zero out item counts from source and destination
            m_source.RemoveItems(removedSourceNum);
            destinationContainer.RemoveItems(removeDestinationNum);

            var sourceTakebackNum = CalculateTakeBack(removedSourceItem, removedSourceNum, sourceContainer, destinationContainer);
            var destinationTakebackNum = CalculateTakeBack(removeDestinationItem, removeDestinationNum, destinationContainer, sourceContainer);

            //Do takebacks if numbers being moved are exceeding container maximums
            if (sourceTakebackNum > 0)
            {
                //Put takeback number of source items back into container. 
                sourceContainer.AddItems(removedSourceItem, sourceTakebackNum);
                removedSourceNum -= sourceTakebackNum;
            }
            if (destinationTakebackNum > 0)
            {
                //Debug.Log(" D2#::" + destinationTakebackNum);
                //Put takeback number of destination items back into container. 
                destinationContainer.AddItems(removeDestinationItem, destinationTakebackNum);
                removeDestinationNum -= destinationTakebackNum;
            }

            //Debug.Log(sourceContainer.MaxAcceptable(removeDestinationItem) +"::" + removeDestinationNum + "||"+ destinationContainer.MaxAcceptable(removedSourceItem) +"::"+ removedSourceNum);
            //Abort if we can't do the swap
            if (sourceContainer.MaxAcceptable(removeDestinationItem) < removeDestinationNum || destinationContainer.MaxAcceptable(removedSourceItem) < removedSourceNum)
            {
                //Debug.Log(" D3#::" + removeDestinationNum);
                //reset everything back to the way it was
                destinationContainer.AddItems(removeDestinationItem, removeDestinationNum);
                sourceContainer.AddItems(removedSourceItem, removedSourceNum);
                return;
            }

            //Do the swap
            if (removeDestinationNum > 0)
            {
                sourceContainer.AddItems(removeDestinationItem, removeDestinationNum);
            }

            if(removedSourceNum > 0)
            {
                destinationContainer.AddItems(removedSourceItem, removedSourceNum);
            }
        }

        /// <summary>
        /// Calculates whether or not all the items can be moved to a destination
        /// and how many must be left at the source if the number exceeds the 
        /// maximum number of items a destination can accept
        /// </summary>
        /// <param name="removeItem"></param>
        /// <param name="removeNum"></param>
        /// <param name="removeSource"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        private int CalculateTakeBack(T removeItem, int removeNum, IDragContainer<T> removeSource, IDragContainer<T> destination)
        {
            var takebacknum = 0;
            
            //Get Maximum number of the item the destination can hold
            var destinationMaxAccept = destination.MaxAcceptable(removeItem);

            //Debug.Log("CT ::" + destinationMaxAccept + " RN::" + removeNum);

            //Check if more of an item is being moved to the destination than can be accepted
            if (destinationMaxAccept < removeNum)
            {
                //Calculate how much must be given back to the source because the number being moved
                //Is greater then the destination will accept
                takebacknum = removeNum - destinationMaxAccept;

                //Get maximum number of the item the source can hold
                var sourceTakebackAccept = removeSource.MaxAcceptable(removeItem);

                //Make sure the source can accept the number of items not being transfered.
                //if not Abort and reset
                if (sourceTakebackAccept < takebacknum)
                {
                    return 0;
                }
            }

            return takebacknum;
        }
    }
}
