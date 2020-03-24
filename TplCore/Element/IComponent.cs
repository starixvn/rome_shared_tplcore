using System;
using System.Collections.Generic;
using System.Text;


namespace TplCore.Element
{
    public interface IComponent
    {
        /// <summary>
        /// Exclude external resources when the component appear in more than one place in the parent template.
        /// </summary>
        void HideLinkedResources();

        /// <summary>
        /// Render component to string.
        /// </summary>
        /// <returns></returns>
        string ToString();
    }
}
