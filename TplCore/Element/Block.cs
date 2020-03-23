/*  Author: Lee, SMS
 *  @20091122 - Created
 *  Revision:
 *  @20091226 - Auto-located external css and js
 *  @20100113 - Auto-located img src and form action
 *  All rights reserved.
 */

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections;

namespace TplCore.Element
{
    public abstract class Block : BaseElement, ICloneable
    {
        /// <summary>
        /// Not null indicate that the block was assigned value
        /// </summary>
        protected StringBuilder Value;
        protected bool IsAssigned = false;
        protected bool RenderChild = true;
        const int __bz = 1000;
        private static Hashtable __hashTypes = new Hashtable();

        /// <summary>
        /// Assign to block
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
        /// Append to block
        /// </summary>
        /// <param name="value"></param>
        public void Append(string value)
        {
            if (null == Value) Value = new StringBuilder(__bz);
            Value.Append(value);
            IsAssigned = true;
        }

        /// <summary>
        /// Append to block
        /// </summary>
        /// <param name="value"></param>
        public void Append(Block b)
        {
            if (null == Value) Value = new StringBuilder(__bz);
            b.ToString(ref Value);
            IsAssigned = true;
        }

        /// <summary>
        /// Append to block
        /// </summary>
        /// <param name="value"></param>
        public void Append(Template t)
        {
            if (null == Value) Value = new StringBuilder(__bz);
            t.ToString(ref Value);
            IsAssigned = true;
        }

        public override string ToString()
        {
            if (!Visible) return null;
            StringBuilder bd = new StringBuilder();
            ToString(ref bd);
            return bd.ToString();
        }

        public abstract void ToString(ref StringBuilder bd);

        /// <summary>
        /// Assign empty string to block
        /// </summary>
        public void Clear()
        {
            Value = null;
            RenderChild = false;
        }

        /// <summary>
        /// Copy block
        /// </summary>
        /// <returns>Return a copied block</returns>
        public object Clone()
        {
            Type type = GetType();

            // Create instance
            object b = type.GetConstructor(Type.EmptyTypes).Invoke(null);

            // Copy fields
            FieldInfo[] f;
            if (!__hashTypes.Contains(type.Name))
            {
                f = type.GetFields(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public);
                __hashTypes.Add(type.Name, f);
            }
            else
            {
                f = (FieldInfo[])(__hashTypes[type.Name]);
            }

            foreach (FieldInfo fi in f)
            {
                object obj = fi.GetValue(this);
                if (null != obj)
                {
                    fi.SetValue(b, ((ICloneable)(obj)).Clone());
                }
                else
                {
                    fi.SetValue(b, null);
                }
            }

            return b;
        }

        public static implicit operator string(Block b)
        {
            StringBuilder bd = new StringBuilder(__bz);
            b.ToString(ref bd);
            return bd.ToString();
        }
    }
}
