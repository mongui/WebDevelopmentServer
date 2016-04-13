using System;
using System.Text.RegularExpressions;

namespace WDS
{
    class DataBaseConfig
    {
        //public void LoadSettings()
        //{
        //}

        public Boolean CheckRegex(String pass)
        {
            Match password = Regex.Match(pass, @"
                                    ^             # Match the start of the string
                                    (?=.*\p{Lu})  # Positive lookahead assertion, is true when there is an uppercase letter
                                    (?=.*\p{Ll})  # Positive lookahead assertion, is true when there is an lowercase letter
                                    (?=.*\P{L})   # Positive lookahead assertion, is true when there is a non-letter
                                    \S{8,}        # At least 8 non whitespace characters
                                    $             # Match the end of the string
                                    ", RegexOptions.IgnorePatternWhitespace);

            return password.Success;
        }

        public void ClearConfig()
        {
            // Not needed right now.
        }

        public void SetDefaultConfig(String Password)
        {
            ClearConfig();

            Globals.DataBasePassChange = true;
            Config.SaveSetting("DataBasePass", Password);
        }
    }
}
