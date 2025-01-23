using RainMeadow;
using System;
using static RainMeadowCompat.EasyRPC;

namespace RainMeadowCompat;

public static class MoreSlugcatsNerfsRPCS
{
    public static EasyRPC SubtractQuarterPip = new EasyRPC(SubtractQuarterPipMethod, Recipient.All, Recipient.All, false);

    [RPCMethod]
    public static void SubtractQuarterPipMethod(RPCEvent rpcEvent, OnlinePhysicalObject opo, int amount)
    {
        try
        {
            MeadowCompatSetup.LogSomething("SubtractQuarterPipRPC: " + amount);

            if (opo.apo.realizedObject is not Player)
            {
                MeadowCompatSetup.LogSomething("Couldn't find realized object for player!");
                return;
            }

            //loops through each player, thus indicating that each player's food display should be updated
            foreach (AbstractCreature p in opo.apo.world.game.Players)
            {
                if (p.realizedCreature == null) continue;
                Nerfs.Nerfs.Instance.Player_SubtractQuarterPips(p.realizedCreature as Player, amount, false);
            }

            MeadowCompatSetup.LogSomething("Finished RPC");
        }
        catch (Exception ex) { MeadowCompatSetup.LogSomething(ex); }
    }

}
