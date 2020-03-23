/*  Author: Lee, SMS
 *  @20091122
 *  All rights reserved.
 *  !!!Curently this class is not used!!!
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace TplCore.Element
{
    sealed class Tag : ICloneable
    {
        private string Value;

        /// <summary>
        /// String that indicate a reference tag (in case a tag appearance more one time)
        /// </summary>

        public override string ToString()
        {
            return Value;
        }

        public void ToString(ref StringBuilder builder)
        {
            if (null == builder) return;
            builder.Append(Value);
        }

        public static implicit operator Tag(string value)
        {
            Tag tag = new Tag();
            tag.Value = value;
            return tag;
        }

        public static implicit operator string(Tag tag)
        {
            return tag.Value;
        }

        /// <summary>
        /// Copy tag
        /// </summary>
        /// <returns>return a copied tag</returns>
        public object Clone()
        {
            Tag tag = new Tag();
            tag.Value = Value;
            return tag;
        }
    }
}
