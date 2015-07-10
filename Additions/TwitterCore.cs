using Android.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Twitter.Sdk.Android.Core
{
    public partial class TwitterCore
    {
        [Register("doInBackground", "()Ljava/lang/Object;", "GetDoInBackgroundHandler")]
        protected override unsafe global::Java.Lang.Object DoInBackground()
        {
            return DoInBackgroun();
        }

        public override unsafe int CompareTo(Java.Lang.Object another)
        {
            return 0;
        }
    }
}
