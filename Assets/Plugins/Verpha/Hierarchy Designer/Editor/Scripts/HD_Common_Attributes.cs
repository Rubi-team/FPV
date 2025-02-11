#if UNITY_EDITOR
using System;

namespace Verpha.HierarchyDesigner
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    internal class HD_Common_Attributes : Attribute
    {
        public HierarchyDesigner_Attribute_Tools Category { get; private set; }

        public HD_Common_Attributes(HierarchyDesigner_Attribute_Tools category)
        {
            Category = category;
        }
    }

    internal enum HierarchyDesigner_Attribute_Tools
    {
        Activate,
        Deactivate,
        Count,
        Lock,
        Unlock,
        Rename,
        Select,
        Sort
    }
}
#endif