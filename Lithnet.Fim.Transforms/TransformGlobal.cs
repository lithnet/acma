using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lithnet.Fim.Transforms
{
    public static class TransformGlobal
    {
        public static bool HostProcessSupportsLoopbackTransforms { get; set; }
        public static bool HostProcessSupportsNativeDateTime { get; set; }
    }
}
