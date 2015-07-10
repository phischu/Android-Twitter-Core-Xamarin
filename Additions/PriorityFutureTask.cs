using Android.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IO.Fabric.Sdk.Android.Services.Concurrency
{
    public partial class PriorityFutureTask
    {
        public virtual unsafe global::System.Collections.ICollection Dependencies
        {
            [Register("getDependencies", "()Ljava/util/Collection;", "GetGetDependenciesHandler")]
            get
            {
                return (global::System.Collections.ICollection)Dependencis();
            }
        }
    }
}
