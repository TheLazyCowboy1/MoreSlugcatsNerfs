using MonoMod.RuntimeDetour;
using System;
using RainMeadow;
using System.Reflection;

namespace RainMeadowCompat;

/**<summary>
 * This class is designed to make setting up Rain Meadow compatibility as easy as possible.
 * All you really need to concern yourself with within this file are:
 * AddLobbyData() - here is where you'll initialize your ResourceDatas.
 * LogSomething() - for convenient logging.
 * 
 * Make sure to EITHER
 * call InitializeMeadowCompatibility() on OnEnable()
 * OR call ModsInitialized() when/after mods are initialized, and before the lobby is joined.
 * </summary>
 */
public class MeadowCompatSetup
{
    //the mod id of Rain Meadow
    public const string RAIN_MEADOW_ID = "henpemaz_rainmeadow";

    //whether Rain Meadow is currently enabled. Set by ModsInitialized()
    public static bool MeadowEnabled = false;

    //keeps track of whether the OnModsInit hook was added
    private static bool AddedOnModsInit = false;

    /**<summary>
     * Use SafeMeadowInterface.InitializeMeadowCompatibility() instead.
     * 
     * The easiest way to set up Meadow compatibility, since everything is managed here.
     * Should be called by OnEnable().
     * MUST be called before mods are initialized.
     * If mods are already initialized, use ModsInitialized() instead.
     * </summary>
     */
    public static void InitializeMeadowCompatibility()
    {
        On.RainWorld.OnModsInit += RainWorld_OnModsInit;

        AddedOnModsInit = true;
    }

    
    private static void RainWorld_OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
    {
        orig(self);

        ModsInitialized();
    }

    /**<summary>
     * Use SafeMeadowInterface.ModsInitialized() instead.
     * 
     * Should be called when or after mods are initialized.
     * Automatically called if InitializeMeadowCompatibility() was called at OnEnable().
     * Checks if Rain Meadow is installed.
     * </summary>
     */
    public static void ModsInitialized()
    {
        foreach (ModManager.Mod mod in ModManager.ActiveMods)
        {
            if (mod.id == RAIN_MEADOW_ID)
            {
                MeadowEnabled = true;
                AddLobbyHook();
                break;
            }
        }
    }

    private static Hook LobbyHook = null;
    private static void AddLobbyHook()
    {
        try
        {
            LobbyHook = new Hook(
                typeof(OnlineResource).GetMethod("Available", BindingFlags.Instance | BindingFlags.NonPublic),
                typeof(MeadowCompatSetup).GetMethod(nameof(OnLobbyAvailable), BindingFlags.Static | BindingFlags.NonPublic)
            );

            //in the future, we may instaed use something like:
            //Lobby.OnAvailable += AddLobbyData;
        }
        catch (Exception ex) { LogSomething(ex); }
    }

    /**<summary>
     * Use SafeMeadowInterface.RemoveHooks() instead.
     * 
     * Should be called by OnDisable().
     * Removes any hooks added by this file.
     * </summary>
     */
    public static void RemoveHooks()
    {
        try
        {
            if (AddedOnModsInit) On.RainWorld.OnModsInit -= RainWorld_OnModsInit;

            //destroy the OnLobbyAvailable hook, if it exists
            if (LobbyHook != null) LobbyHook.Dispose();
        }
        catch (Exception ex) { LogSomething(ex); }
    }


    private delegate void orig_OnLobbyAvailable(OnlineResource self);
    private static void OnLobbyAvailable(orig_OnLobbyAvailable orig, OnlineResource self)
    {
        orig(self);

        AddLobbyData(self);
    }

    /**
     * This is the place to add all your initial data.
     * This function is called as soon as a lobby is available.
     * This is the best place to add static data,
     *  such as config (Remix) options, global variables, randomizer files, etc.
     * 
     * Other data (like for items) should likely be added elsewhere.
     * ...I'm not sure where, yet.
     */
    private static void AddLobbyData(OnlineResource lobby)
    {
        if (!lobby.TryGetData(out ConfigData currentConfigData))
            lobby.AddData(new ConfigData());

        //lobby.AddData<ExampleData>(new ExampleData());
    }


    /**<summary>
     * Add your preferred logging method here.
     * If you don't want any logs... just let this function be empty, I guess.
     * </summary>
     */
    public static void LogSomething(object obj)
    {
        Nerfs.Nerfs.LogSomething(obj);
    }

    /**<summary>
     * If you want EXTRA debug info, you can use this.
     * Splitting logging into two separate methods makes it easier to
     * disable unneeded logging messages without fully deleting them.
     * </summary>
     */
    public static void ExtraDebug(object obj)
    {
        Nerfs.Nerfs.LogSomething(obj);
    }
}
