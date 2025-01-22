using System.Collections.Generic;

namespace RainMeadowCompat;

/**<summary>
 * Your ticket to the easiest config syncing ever!!
 * 
 * Just call RegisterConfigs().
 * </summary>
 */
public class EasyConfigSync
{
    public static List<Configurable<bool>> BoolConfigs = new();
    public static List<Configurable<int>> IntConfigs = new();
    public static List<Configurable<float>> FloatConfigs = new();
    public static List<Configurable<string>> StringConfigs = new();

    /**<summary>
     * Provide configs that you want synced.
     * Just call this function and list your configurables. That's it!
     * 
     * Currently accepts:
     * Configurable<bool>
     * Configurable<int>
     * Configurable<float>
     * Configurable<string>
     * </summary>
     */
    public static void RegisterConfigs(params ConfigurableBase[] configs)
    {
        foreach (ConfigurableBase config in configs)
        {
            if (config is Configurable<bool>) BoolConfigs.Add(config as Configurable<bool>);
            else if (config is Configurable<int>) IntConfigs.Add(config as Configurable<int>);
            else if (config is Configurable<float>) FloatConfigs.Add(config as Configurable<float>);
            else if (config is Configurable<string>) StringConfigs.Add(config as Configurable<string>);
            else
                MeadowCompatSetup.LogSomething("Couldn't add config" + config.ToString() + ". Not a bool, int, float, or string.");
        }

        SafeMeadowInterface.UpdateConfigData();
    }

    public static void RemoveConfigs(params ConfigurableBase[] configs)
    {
        foreach (ConfigurableBase config in configs)
        {
            if (config is Configurable<bool>) BoolConfigs.Remove(config as Configurable<bool>);
            else if (config is Configurable<int>) IntConfigs.Remove(config as Configurable<int>);
            else if (config is Configurable<float>) FloatConfigs.Remove(config as Configurable<float>);
            else if (config is Configurable<string>) StringConfigs.Remove(config as Configurable<string>);
            else
                MeadowCompatSetup.LogSomething("Couldn't remove config" + config.ToString() + ". Not a bool, int, float, or string.");
        }

        SafeMeadowInterface.UpdateConfigData();
    }
}
