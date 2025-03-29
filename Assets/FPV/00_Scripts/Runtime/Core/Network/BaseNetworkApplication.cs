namespace FPV
{
    /// <summary>
    /// Extension of the BaseApplication class to handle different types of Model View Controllers.
    /// </summary>
    /// <typeparam name="M"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <typeparam name="C"></typeparam>
    public class BaseNetworkApplication<M, V, C> : BaseNetworkApplication
        where M : NetworkElement
        where V : NetworkElement
        where C : NetworkElement
    {
        internal new BaseNetworkApplication<M, V, C> Instance => (BaseNetworkApplication<M, V, C>)(object)base.Instance;

        /// <summary>
        /// Model reference using the new type.
        /// </summary>
        public M Model => (M)(object)NetworkModel;

        /// <summary>
        /// View reference using the new type.
        /// </summary>
        public V View => (V)(object)NetworkView;

        /// <summary>
        /// Controller reference using the new type.
        /// </summary>
        public C Controller => (C)(object)NetworkController;
    }

    /// <summary>
    /// Root class for the scene's scripts.
    /// </summary>
    public class BaseNetworkApplication : NetworkElement
    {
        internal BaseNetworkApplication Instance { get; private set; }

        internal EventManager EventManager;

        /// <summary>
        /// Fetches the root Model instance.
        /// </summary>
        internal NetworkModel NetworkModel => mNetworkModel = Find<NetworkModel>(mNetworkModel);

        private NetworkModel mNetworkModel;

        /// <summary>
        /// Fetches the root View instance.
        /// </summary>
        internal NetworkView NetworkView => mNetworkView = Find<NetworkView>(mNetworkView);

        private NetworkView mNetworkView;

        /// <summary>
        /// Fetches the root Controller instance.
        /// </summary>
        internal NetworkController NetworkController =>
            mNetworkController = Find(mNetworkController);

        private NetworkController mNetworkController;

        /// <summary>
        /// Initializes the BaseApplication
        /// </summary>
        public BaseNetworkApplication()
        {
            EventManager ??= new EventManager();
        }

        protected virtual void Awake()
        {
            EventManager ??= new EventManager();
        }

        /// <summary>
        /// Notifies an event to the component's of the app
        /// </summary>
        /// <param name="evt"></param>
        internal new void Broadcast(AppEvent evt)
        {
            EventManager.Broadcast(evt);
        }
    }
}