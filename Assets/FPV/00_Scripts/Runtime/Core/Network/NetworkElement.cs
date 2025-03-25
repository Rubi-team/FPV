using Unity.Netcode;
using UnityEngine;

namespace FPV
{
    /// <summary>
    /// Extension of the element class to handle different BaseApplication types.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NetworkElement<T> : NetworkElement where T : BaseNetworkApplication
    {
        /// <summary>
        /// Returns app as a custom 'T' type.
        /// </summary>
        public new T App => (T)base.App;
    }

    /// <summary>
    /// Base class for all MVC related classes.
    /// </summary>
    public class NetworkElement : NetworkBehaviour
    {
        /// <summary>
        /// Reference to the root application of the scene.
        /// </summary>
        public BaseNetworkApplication App => m_app = FindInParent<BaseNetworkApplication>(m_app);

        private BaseNetworkApplication m_app;

        /// <summary>
        /// Finds a instance of 'T' if 'var' is null. Returns 'var' otherwise.
        /// </summary>
        /// <typeparam name="T">Type to find</typeparam>
        /// <param name="p_var"></param>
        /// <param name="searchGlobally">If true searches in all scope, otherwise, searches in children.</param>
        /// <returns></returns>
        internal T Find<T>(T p_var, bool searchGlobally = false) where T : Object
        {
            return p_var == null
                ? searchGlobally
                    ? FindFirstObjectByType<T>()
                    : transform.GetComponentInChildren<T>(true)
                : p_var;
        }

        private T FindInParent<T>(T p_var) where T : Object
        {
            return p_var == null
                ? transform.GetComponentInParent<T>()
                : p_var;
        }

        /// <summary>
        /// Notifies to the listening controllers the event
        /// </summary>
        /// <param name="eventID">The name of the event to notify</param>
        /// <param name="data">The parameters to pass to the listening controllers</param>
        internal void Broadcast(AppEvent evt)
        {
            App.Broadcast(evt);
        }
    }
}