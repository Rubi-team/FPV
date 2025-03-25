namespace FPV
{
    /// <summary>
    /// Base class for all View related classes.
    /// A View's purpose is to display data and objects (typically contained in the model)
    /// </summary>
    public class NetworkView : NetworkElement
    {
    }

    /// <summary>
    /// Base class for all View related classes.
    /// </summary>
    public class NetworkView<T> : NetworkView where T : BaseNetworkApplication
    {
        /// <summary>
        /// Returns app as a custom 'T' type.
        /// </summary>
        public new T App => (T)base.App;

        internal void Show()
        {
            gameObject.SetActive(true);
        }

        internal void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}