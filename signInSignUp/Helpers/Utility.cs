using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace signInSignUp.Helpers
{
    public static class Utility
    {
        public static void TogglePasswordVisibility(Entry passwordEntry, ref bool isPasswordVisible)
        {
            isPasswordVisible = !isPasswordVisible;
            passwordEntry.IsPassword = !isPasswordVisible;
        }
    }
}