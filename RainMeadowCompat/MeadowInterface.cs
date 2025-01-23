using RainMeadow;
using System;

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

    public static OnlinePhysicalObject PlayerToOnlineObject(Player player)
    {
        return player.abstractPhysicalObject.GetOnlineObject();
    }

    public static void InvokeSubtractQuarterPipRPC(Player player, int amount)
    {
        //new SubtractQuarterPipRPC().Invoke(true, PlayerToOnlineObject(player));
        //InvokeRPC(true, Recipient.Clients, Recipient.Host, false, TestDelegateMethod, PlayerToOnlineObject(player), amount);
        MoreSlugcatsNerfsRPCS.SubtractQuarterPip.InvokeRPC(false, PlayerToOnlineObject(player), amount);
    }
    
}
