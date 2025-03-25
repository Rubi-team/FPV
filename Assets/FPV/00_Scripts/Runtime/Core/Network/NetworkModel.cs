namespace FPV
{
    /// <summary>
    /// Base class for all Model related classes.
    /// A Model's purpose is to contain data about something (tipically its view)
    /// </summary>
    public class NetworkModel : NetworkElement
    {
    }

    /// <summary>
    /// Base class for all Model related classes.
    /// </summary>
    public class NetworkModel<T> : NetworkModel where T : BaseNetworkApplication
    {
        /// <summary>
        /// Returns app as a custom 'T' type.
        /// </summary>
        public new T App => (T)base.App;
    }
}