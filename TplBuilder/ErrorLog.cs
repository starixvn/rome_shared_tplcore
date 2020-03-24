using System;
using System.Windows.Forms;

namespace TplBuilder
{
    public static class ErrorLog
    {
        public static void Log(string message)
        {
            MessageBox.Show(message, Config.MsgCaption);
        }

        public static void Log(Exception ex)
        {
            string message = string.Format("Message:\n{0}\n\nStackTrace:\n{1}", ex.Message, ex.StackTrace);
            Log(message);
        }
    }
}
