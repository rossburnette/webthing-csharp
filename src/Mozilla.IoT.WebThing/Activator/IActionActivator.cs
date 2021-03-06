using System;
using System.Collections.Generic;

namespace Mozilla.IoT.WebThing.Activator
{
    internal interface IActionActivator
    {
        Action CreateInstance(IServiceProvider serviceProvider, Thing thing, string name, IDictionary<string, object> input);
    }
}
