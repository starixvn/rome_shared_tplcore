using System;
using System.Collections.Generic;
using System.Text;

namespace TplCore.Element
{
    public interface IElement
    {
        bool RegisterComponent(string name, IComponent component);
        bool RegisterComponent(IComponent component);
    }
}
