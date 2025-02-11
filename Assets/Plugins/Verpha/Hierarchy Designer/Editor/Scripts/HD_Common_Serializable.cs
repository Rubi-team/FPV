#if UNITY_EDITOR
using System.Collections.Generic;

namespace Verpha.HierarchyDesigner
{
    [System.Serializable]
    internal class HD_Common_Serializable<T>
    {
        public List<T> items;

        public HD_Common_Serializable(List<T> items)
        {
            this.items = items;
        }
    }
}
#endif