/*  Author: Lee, SMS
 *  @20091122 - Created
 *  Revision:
 *  @20091226 - Auto-located external css and js
 *  @20100113 - Auto-located img src and form action
 *  All rights reserved.
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Web;
using TplCore;

namespace TplCore.Element
{
    public abstract class Template : BaseElement
    {
        protected StringBuilder Value;
        protected bool IsAssigned = false;
        protected bool RenderChild = true;

        const int __tz = 1000; // Initialize StringBuilder length

        /// <summary>
        /// Assign to template
        /// </summary>
        /// <param name="value">Value to be assigned</param>
        public void SetValue(string value)
        {
            if (null != Value) Value = null;
            Value = new StringBuilder(value);
            IsAssigned = true;
            RenderChild = false;
        }

        /// <summary>
        /// Assign empty string to template
        /// </summary>
        public void Clear()
        {
            Value = null;
            RenderChild = false;
        }

        public void Append(object obj)
        {
            if (null == obj) return;
            string text = obj.ToString();
            if (null == Value)
            {
                Value = new StringBuilder(text);
            }
            else
            {
                Value.Append(obj.ToString());
            }
        }

        public abstract void ToString(ref StringBuilder bd);

        public static implicit operator string(Template t)
        {
            if (!t.Visible) return null;

            StringBuilder bd = new StringBuilder(__tz);
            t.ToString(ref bd);
            return bd.ToString();
        }

        /// <summary>
        /// Registered components collection.
        /// </summary>
        protected List<string> RegisteredComponents = new List<string>(10);

        /// <summary>
        /// Handle registered child components.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="component"></param>
        /// <returns></returns>
        public override bool RegisterComponent(string name, IComponent component)
        {
            if (RegisteredComponents.Contains(name))
            {
                component.HideLinkedResources();
                return false;
            }
            else
            {
                RegisteredComponents.Add(name);
                return true;
            }
        }

        /// <summary>
        /// Handle registered child components.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="component"></param>
        /// <returns></returns>
        public override bool RegisterComponent(IComponent component)
        {
            return RegisterComponent(component.GetType().Name, component);
        }
    }
}
