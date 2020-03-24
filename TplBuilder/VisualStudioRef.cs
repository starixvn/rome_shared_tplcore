namespace TplBuilder
{
    using EnvDTE80;
    using System;

    public static class VisualStudioRef
    {
        private static DTE2 _applicationObject;

        public static DTE2 ApplicationObject
        {
            get
            {
                if (_applicationObject == null)
                {
                    throw new ArgumentNullException("ApplicationObject cannot be null!");
                }

                return _applicationObject;
            }

            set
            {
                _applicationObject = value;
            }
        }
    }
}
