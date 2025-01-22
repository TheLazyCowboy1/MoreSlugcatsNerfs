using System;
using RainMeadow;

namespace RainMeadowCompat;

/**<summary>
 * This class is really quite simple:
 * Make a class that extends this one:
 * E.g: public class RandomizerData : RainMeadowCompat.ManuallyUpdatedData
 * 
 * In it, do 4 things:
 * Specify whether it should be "HostControlled".
 * Add a construct that INITIALIZES CurrentState.
 * Make UpdateData() reconstruct CurrentState.
 * Make a ManuallyUpdatedState of your own (see ManuallyUpdatedState).
 * Example:
 * UpdateData() => CurrentState = new RandomizerState();
 * private class RandomizerState : RainMeadowCompat.ManuallyUpdatedState
 * </summary>
 */
public abstract class ManuallyUpdatedData : OnlineResource.ResourceData
{

    public ManuallyUpdatedState CurrentState;
    public ulong LastUpdateTime = (ulong)DateTime.Now.Ticks;


    /**<summary>
     * If true, the host will not receive client data,
     * and if any client tries to send data, the host will override it.
     * </summary>
     */
    public abstract bool HostControlled { get; }

    /**<summary>
     * In your implementation, be sure to initialize CurrentState.
     * Example:
     * public RandomizerData() {
     * CurrentState = new RandomizerData(this);
     * }
     * </summary>
     */
    public ManuallyUpdatedData()
    {}

    /**<summary>
     * In this function, you must reconstruct CurrentState.
     * Example:
     * CurrentState = new RandomizerState(this);
     * </summary>
     */
    public abstract void UpdateData();


    /**<summary>
     * Although this function is only supposed to be called when there are changes made to the state,
     * I found that it would be called every tick.
     * Additionally, I like manual control.
     * Therefore, instead of returning a new state, I simply return the old one
     * and update CurrentState whenever I feel like it.
     * </summary>
     */
    public override ResourceDataState MakeState(OnlineResource resource)
    {
        return CurrentState;
    }

    /**<summary>
     * Called when a new state is created.
     * </summary>
     */
    public ulong ResetUpdateTime()
    {
        LastUpdateTime = (ulong)DateTime.Now.Ticks;
        MeadowCompatSetup.ExtraDebug("Updated LastUpdateTime: " + LastUpdateTime);
        return LastUpdateTime;
    }

}
