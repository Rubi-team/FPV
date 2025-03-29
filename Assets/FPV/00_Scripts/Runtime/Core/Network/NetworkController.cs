using System;

namespace FPV
{
    /// <summary>
    /// Base class for all Controllers in the application.
    /// A Controller's purpose is to act as bridge between its view and model, 
    /// reacting on events and performing operations on either side
    /// </summary>
    public class NetworkController : NetworkElement
    {
    }

    /// <summary>
    /// Base class for all Controller related classes.
    /// </summary>
    public abstract class NetworkController<T> : NetworkController where T : BaseNetworkApplication
    {
        /// <summary>
        /// Returns app as a custom 'T' type.
        /// </summary>
        public new T App => (T)base.App;

        /// <summary>
        /// Subscribes to an AppEvent
        /// </summary>
        /// <param name="evt">Callback for an AppEvent</param>
        internal void AddListener<E>(Action<E> evt) where E : AppEvent
        {
            App.EventManager.AddListener(evt);
        }

        internal void RemoveListener<E>(Action<E> evt) where E : AppEvent
        {
            App.EventManager.RemoveListener(evt);
        }

        internal abstract void RemoveListeners();
    }
}