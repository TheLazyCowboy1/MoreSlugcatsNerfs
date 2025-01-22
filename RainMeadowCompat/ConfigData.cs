using System;
using RainMeadow;
using static RainMeadowCompat.EasyConfigSync;

namespace RainMeadowCompat;

/**<summary>
 * Syncs config data between the host and the clients.
 * 
 * Use EasyConfigSync to list which configs you want synced.
 * </summary>
 */
public class ConfigData : ManuallyUpdatedData
{
    //We don't want clients overriding the host's settings
    public override bool HostControlled => true;

    public ConfigData() {
        CurrentState = new ConfigState(this);
    }
    

    /**
     * Honestly, there really shouldn't be any need to ever call this function.
     * Configs shouldn't be changed after joining the lobby.
     * ...except when a client tries to override the host's data.
     * Which hopefully never should happen, right??
     */
    public override void UpdateData()
    {
        CurrentState = new ConfigState(this);
    }

    private class ConfigState : ManuallyUpdatedState
    {
        public override Type GetDataType() => typeof(ConfigData);

        [OnlineField]
        bool[] Bools;
        [OnlineField]
        int[] Ints;
        [OnlineField]
        float[] Floats;
        [OnlineField]
        string[] Strings;

        /**<summary>
         * An empty constructor is necessary. Rain Meadow will not start without it.
         * I don't know why...
         * 
         * I have no clue when or why this happens, so just assume it "never does."
         * </summary>
         */
        public ConfigState() : base(null) { }
        public ConfigState(ConfigData data) : base(data)
        {
            Bools = new bool[BoolConfigs.Count];
            for (int i = 0; i < Bools.Length; i++)
                Bools[i] = BoolConfigs[i].Value;

            Ints = new int[IntConfigs.Count];
            for (int i = 0; i < Ints.Length; i++)
                Ints[i] = IntConfigs[i].Value;

            Floats = new float[FloatConfigs.Count];
            for (int i = 0; i < Floats.Length; i++)
                Floats[i] = FloatConfigs[i].Value;

            Strings = new string[StringConfigs.Count];
            for (int i = 0; i < Strings.Length; i++)
                Strings[i] = StringConfigs[i].Value;

            MeadowCompatSetup.LogSomething("Initialized a new ConfigState.");

        }

        public override void UpdateReceived(ManuallyUpdatedData data, OnlineResource resource)
        {
            //update options
            for (int i = 0; i < Bools.Length; i++)
                BoolConfigs[i].Value = Bools[i];

            for (int i = 0; i < Ints.Length; i++)
                IntConfigs[i].Value = Ints[i];

            for (int i = 0; i < Floats.Length; i++)
                FloatConfigs[i].Value = Floats[i];

            for (int i = 0; i < Strings.Length; i++)
                StringConfigs[i].Value = Strings[i];

            MeadowCompatSetup.LogSomething("Updated config values.");
        }
    }
}
