using System;
using System.Text.RegularExpressions;

namespace WDS
{
    class DataBaseConfig
    {
        public Boolean CheckRegex(String pass)
        {
            Match password = Regex.Match(pass ?? "", @"
                                    ^             # Match the start of the string
                                    (?=.*\p{Lu})  # Positive lookahead assertion, is true when there is an uppercase letter
                                    (?=.*\p{Ll})  # Positive lookahead assertion, is true when there is an lowercase letter
                                    (?=.*\P{L})   # Positive lookahead assertion, is true when there is a non-letter
                                    \S{8,}        # At least 8 non whitespace characters
                                    $             # Match the end of the string
                                    ", RegexOptions.IgnorePatternWhitespace);

            return password.Success;
        }

        /// <summary>
        /// Stores a new root password. It is applied the next time the database server starts.
        /// </summary>
        public void SetDefaultConfig(String Password)
        {
            Globals.DataBasePass = Password;
            Globals.DataBasePassPending = "1";
            Config.SaveSetting("DataBasePass", Password);
            Config.SaveSetting("DataBasePassPending", "1");
        }
    }
}
