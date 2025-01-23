using RainMeadow;
using System;
using System.Reflection;

namespace RainMeadowCompat;

/**<summary>
 * This class is designed to streamline the use of RPCs as much as possible.
 * The recommended use is to define two things:
 * 1. Define a public static method with the [RPCMethod] flag and a RPCEvent as its first parameter.
 * Example:
 * [RPCMethod]
 * public static void ExampleRPCMethod(RPCEvent rpcEvent, int myFavoriteNumber) { }
 * 
 * 2. Define a public static instance of an EasyRPC defining which method to use
 * and from whom / to whom the RPC should be sent.
 * Example:
 * public static EasyRPC MyRPC = new(ExampleRPCMethod, Recipient.Host, Recipient.Clients);
 * </summary>
 */
public class EasyRPC
{
    public enum Recipient
    {
        Host,
        Clients,
        All
    }

    private Recipient SentBy;
    private Recipient SendTo;
    private bool SendToSelf;
    private Delegate DelegateMethod;

    /**<summary>
     * Defines what the RPC does and who sends/receives it.
     * </summary>
     * <param name="delegateMethod">
     * A public function/method with the [RPCMethod] flag and RPCEvent as its first parameter.
     * This function will run when the RPC is received.
     * Example:
     * [RPCMethod]
     * public static void ExampleRPCMethod(RPCEvent rpcEvent, int myFavoriteNumber) { }
     * </param>
     * <param name="sentBy">
     * Restricts who can send the RPC: The Host, Clients, or anyone.
     * </param>
     * <param name="sendTo">
     * Restricts who can receive the RPC: The Host, Clients, or anyone.
     * </param>
     * <param name="sendToSelf">
     * Allows the RPC to be sent to oneself. False by default.
     * Be careful not to cause any infinite feedback loops if you make <paramref name="sendToSelf"/> true.
     * </param>
     */
    public EasyRPC(Delegate delegateMethod, Recipient sentBy = Recipient.Clients, Recipient sendTo = Recipient.Host, bool sendToSelf = false)
    {
        DelegateMethod = delegateMethod;
        SentBy = sentBy;
        SendTo = sendTo;
        SendToSelf = sendToSelf;
    }

    /**<summary>
     * An easy method of sending the RPC to players.
     * </summary>
     * <param name="invokeOnce">
     * If true, the RPC will only be sent once. Thus, the recipient might miss it.
     * If false, the RPC will be sent continually until the recipient acknowledges it.
     * </param>
     */
    public void InvokeRPC(bool invokeOnce, params object[] args) => InvokeRPC(invokeOnce, SentBy, SendTo, SendToSelf, args);

    /**<summary>
     * Temporarily overrides the recipients defined in the constructor, and sends the RPC to players.
     * </summary>
     * <param name="invokeOnce">
     * If true, the RPC will only be sent once. Thus, the recipient might miss it.
     * If false, the RPC will be sent continually until the recipient acknowledges it.
     * </param>
     */
    public void InvokeRPC(bool invokeOnce, Recipient sentBy, Recipient sendTo, bool sendToSelf, params object[] args)
    {
        try
        {
            //detect if I match the "SentBy" requirement
            if (sentBy == Recipient.Host && !OnlineManager.lobby.isOwner)
                return;
            if (sentBy == Recipient.Clients && OnlineManager.lobby.isOwner)
                return;

            //loop through online players
            foreach (var player in OnlineManager.players)
            {
                //detect if I am sending to myself
                if (!sendToSelf && player.isMe)
                    continue;

                //detect if player matches "SendTo" requirement
                if (sendTo == Recipient.Host && player != OnlineManager.lobby.owner)
                    continue;
                if (sendTo == Recipient.Clients && player == OnlineManager.lobby.owner)
                    continue;

                //invoke the RPC
                if (invokeOnce)
                    player.InvokeOnceRPC(DelegateMethod, args);
                else
                    player.InvokeRPC(DelegateMethod, args);

                MeadowCompatSetup.ExtraDebug("Invoked RPC");
            }
        }
        catch (Exception ex)
        {
            MeadowCompatSetup.LogSomething(ex);
        }

        //try to detect if the [RPCMethod] attribute is missing, for easy debugging
        try
        {
            if (DelegateMethod.Method.GetCustomAttribute<RPCMethodAttribute>() == null)
                throw new Exception("No [RPCMethod] attribute!");
        }
        catch (Exception ex)
        {
            MeadowCompatSetup.LogSomething("Failed to find [RPCMethod] attribute!");
            MeadowCompatSetup.LogSomething(ex);
        }
    }
}
