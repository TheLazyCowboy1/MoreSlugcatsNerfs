using RainMeadow;

namespace RainMeadowCompat;

/**<summary>
 * This file is here for you to safely signal to signal your Rain Meadow data
 * or receive Rain Meadow information
 * through the SafeMeadowInterface class.
 * 
 * Anything you want to call here should always be called by SafeMeadowInterface instead.
 * 
 * All functions in this file are PURELY examples.
 * </summary>
 */
public class MeadowInterface
{
    /**<summary>
     * This is an example of how you could signal that there has been a change to some data.
     * This function is likely totally unnecessary:
     *  No one's going to be changing config data mid-game, right??
     * But it's here just in case someone does something crazy like that.
     * And it's a good example for how to signal an update to online data.
     * 
     * For example:
     * UpdateRandomizerData() would have:
     * OnlineManager.lobby.GetData<RandomizerData>().UpdateData();
     * </summary>
     */
    public static void UpdateConfigData()
    {
        if (!MeadowCompatSetup.MeadowEnabled) return;

        try
        {
            OnlineManager.lobby.GetData<ConfigData>().UpdateData();
        }
        catch { return; }
    }

    /**<summary>
     * Here is an example of safely getting information from Rain Meadow.
     * In this hypothetical case:
     * If Rain Meadow is enabled, and the player is not the host,
     * I want the randomization process to be skipped.
     * 
     * So I call if (MeadowInterface.ShouldSkipRandomization()) return;
     * Thus safely deciding whether or not to skip the process.
     * </summary>
     */
    public static bool ShouldSkipRandomization()
    {
        if (!MeadowCompatSetup.MeadowEnabled) return false;

        try
        {
            return OnlineManager.lobby != null && !OnlineManager.lobby.isOwner;
        }
        catch { return false; }
    }
}
