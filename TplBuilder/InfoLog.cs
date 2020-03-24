using EnvDTE;
using System;

namespace TplBuilder
{
    public static class InfoLog
    {
        private static OutputWindow _outputWindow;
        public const string vsWindowKindOutput = "{34E76E81-EE4A-11D0-AE2E-00A0C90FFFC3}";

        static InfoLog()
        {
            try
            {
                var window = VisualStudioRef.ApplicationObject.Windows.Item(vsWindowKindOutput);
                _outputWindow = window.Object as OutputWindow;
            }
            catch (Exception ex)
            {
                ErrorLog.Log(ex);
            }
        }

        public static void Log(string message)
        {
            try
            {
                if (_outputWindow.ActivePane != null)
                {
                    _outputWindow.ActivePane.Activate();
                    _outputWindow.ActivePane.OutputString(Config.MsgCaption + ": " + message + "\n");
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Log(ex);
            }
        }

        public static void Log(Exception ex)
        {
            Log(ex.ToString());
        }
    }
}
