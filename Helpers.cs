
using Serilog;
using System.Configuration;


namespace com.Brainsoft.NeurPsy.Experiments
{

    public static class Helpers
    {

        public static int LoadPositiveIntCfgValue(string key, int defaultValue)
        {
            string rawCfgValue = ConfigurationManager.AppSettings.Get(key);
            if (rawCfgValue == null)
            {
                Log.Warning($"App setting \"{key}\" not found.");
                goto useDefault;
            }

            // All app settings are strings. Convert to integer.
            int parsedCfgValue;
            if (!int.TryParse(rawCfgValue, out parsedCfgValue))
            {
                Log.Error($"App setting \"{key}\" must be an integer (is \"${rawCfgValue}\").");
                goto useDefault;
            }

            if (parsedCfgValue <= 0)
            {
                Log.Error($"App setting \"{key}\" must be positive (is ${parsedCfgValue})");
                goto useDefault;
            }

            Log.Information($"\"{key}\" set to {parsedCfgValue}.");
            return parsedCfgValue;
        useDefault:
            Log.Warning($"\"{key}\" defaulting to {defaultValue}.");
            return defaultValue;
        } // end LoadCharIntervalMs()
    }
    //======================================================================
}