using Microsoft.JSInterop;

namespace learnUkk.Helper
{
    public class Core
    {
        public static string alertMessage(string message)
        {
            return "<script>alert('" + message + "');</script>";
        }

        
    }
}
