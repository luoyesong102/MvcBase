using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Esmart.Framework.Patterns.Ballot
{
    /// <summary>
    /// ballot operation enum
    /// </summary>
    public enum BallotRelation
    {
        /// <summary>
        /// all true => return true
        /// </summary>
        AND,
        /// <summary>
        /// all false => return true
        /// </summary>
        NONE,
        /// <summary>
        /// one true => return true
        /// </summary>
        OR,
    }   
    

    /// <summary>
    /// ballot type definition
    /// </summary>
    [Serializable]
    public enum BallotType
    {
        ///// <summary>
        ///// Is it ok to open a valve?
        ///// </summary>
        ////OkToOpenValve,

        ///// <summary>
        ///// Is it ok to close a valve?
        ///// </summary>
        ////OkToCloseValve,

        /***********************************************************************/
        /* Sequencer related ballot subjects.                                  */
        /***********************************************************************/
        /// <summary>
        /// Asks voters if it's ok to start sequencer.
        /// </summary>
        OkToRunSequencer,

        /// <summary>
        /// Asks voters if the system has a deadlock.
        /// </summary>
        IsSystemDeadlocked,

        /// <summary>
        /// Asks voters if it's ok to move susceptor from  Loadlock to Buffer.
        /// </summary>
        OkToMoveSusceptorToBuffer,
        
        /// <summary>
        /// Asks voters if it's ok to move susceptor from Buffer to a reactor.
        /// </summary>
        OkToMoveSusceptorToReactor,

        /// <summary>
        /// Asks voters if it's ok to move susceptor from a reactor to Cooldown.
        /// </summary>
        OkToMoveSusceptorToCooldown,

        /// <summary>
        /// Asks voters if it's ok to move susceptor from Cooldown to Loadlock.
        /// </summary>
        OkToMoveSusceptorToLoadlock,

        /// <summary>
        /// Asks voter if it's ok to launch the recipe in this reactor.
        /// </summary>
        OkToLaunchRecipe,

        /// <summary>
        /// Asks voters if chamber pressure and temperature are at transfer level.
        /// </summary>
        OkToOpenSlitValve,

        /// <summary>
        /// Asks voters if it's ok to run Loadlock service routine.
        /// </summary>
        OkToRunLoadlockServiceRoutine,

        /// <summary>
        /// Asks voters if it's ok to run Transfer service routine.
        /// </summary>
        OkToRunTransferServiceRoutine,

        /// <summary>
        /// Asks voters if it's ok to run reactor service routine.
        /// </summary>
        OkToRunReactorServiceRoutine,

        /// <summary>
        /// Asks voters if it's ok to open Loadlock slow pump.
        /// </summary>
        OkToOpenLlSlowPump,

        /// <summary>
        /// Asks voters if it's ok to open Loadlock fast pump.
        /// </summary>
        OkToOpenLlFastPump,

        /// <summary>
        /// Asks voters if it's ok to open Loadlock slow pump.
        /// </summary>
        OkToOpenTmSlowPump,

        /// <summary>
        /// Asks voters if it's ok to open Loadlock fast pump.
        /// </summary>
        OkToOpenTmFastPump,

        /// <summary>
        /// Asks voters if it's ok to open Loadlock fast vent.
        /// </summary>
        OkToOpenLlFastVent,

        /// <summary>
        /// Asks voters if it's ok to open Loadlock slow vent.
        /// </summary>
        OkToOpenLlSlowVent,


        /// <summary>
        /// Asks voters if it's ok to open TM vent valve.
        /// </summary>
        OkToOpenTmVent,

        /// <summary>
        /// Askew futures if loadlock is ready to accept.
        /// </summary>
        IsLoadlockReadyToAccept,

        /***********************************************************************/
        /* Recipe related ballot subjects.                                     */
        /***********************************************************************/
       
        ///// <summary>
        ///// Asks voters if it's ok to rotate reactor spindle
        ///// </summary>
        ////OkToRotateSpindle,

        ///// <summary>
        ///// Asks voters if it's ok to enable DC power output
        ///// </summary>
        ////OkToEnableDcPower,

        ///// <summary>
        ///// Asks voters if it's ok to move shutter up
        ///// </summary>
        ////OkToMoveShutterWorkPlace,

        ///// <summary>
        ///// Asks voters if it's ok to move shutter down
        ///// </summary>
        ////OkToMoveShutterHomePlace,

        ///// <summary>
        ///// Asks voters if it's ok to enable throttle valve
        ///// </summary>
        ////OkToEnableThrottleValve,

        ///// <summary>
        ///// Asks voters if it's ok to switch to idle
        ///// </summary>
        ////OkToIdle,

        /// <summary>
        /// Asks voters if it's ok to put online
        /// </summary>
        OkToPutOnline,

       // /// <summary>
       // /// Asks voters if it's ok to transfer susceptor in reactor
       // /// </summary>
       // //OkToTransfer,

       ///// <summary>
       // /// Asks voters if it's ready for process
       // /// </summary>
       // //OkToProcess,

       // /// <summary>
       // /// Asks voters if process is completed
       // /// </summary>
       // //IsProcessCompleted,

        /// <summary>
        /// Asks voters if it has error?
        /// </summary>
        IsError,

        /// <summary>
        /// Asks voters if it has warning?
        /// </summary>
        IsWarning,

        ///// <summary>
        ///// Asks voters if it's ok to open lid
        ///// </summary>
        ////OkToOpenLid,

        ///// <summary>
        ///// Ask voters if it's ok to enable spindle
        ///// </summary>
        ////OkToEnableSpindle,

        ///// <summary>
        ///// Ask voters if it's ok to move shut up
        ///// </summary>
        ////OkToMoveShutUp,

        ///// <summary>
        ///// Ask voters if it's ok to move shut down
        ///// </summary>
        ////OkToMoveShutDown,

        // Adds your definitions here.

    }
}
