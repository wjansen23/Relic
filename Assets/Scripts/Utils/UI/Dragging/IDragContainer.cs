using UnityEngine;

namespace RPG.Core.UI.Dragging
{
    /// <summary>
    /// Acts both as a source and destination for dragging. If we are dragging
    /// between two containers then it is possible to swap items.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDragContainer<T> : IDragDestination<T>, IDragSource<T> where T:class
    {
    }
}
