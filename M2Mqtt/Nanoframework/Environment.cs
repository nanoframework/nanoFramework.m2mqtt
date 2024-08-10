/*
Contributors:   
   .NET Foundation and Contributors - nanoFramework support
*/

using System;

namespace Mqtt
{
    static class Environment
    {
        public static int TickCount => (int)DateTime.UtcNow.Ticks;
    }
}
