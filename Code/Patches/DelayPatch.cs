using ColossalFramework;
using HarmonyLib;
using System.Collections.Generic;


namespace TransitVehicleSpawnDelay
{
    /// <summary>
    /// Harmony patch to add delays between spawining of public transit vehicles.
    /// Priority is to run before IPT2 and TLM to ensure no conflict.
    /// </summary>
    [HarmonyPatch(typeof(DepotAI), nameof(DepotAI.StartTransfer))]
    [HarmonyBefore("github.com/bloodypenguin/ImprovedPublicTransport", "com.klyte.redirectors.TLM")]
    public static class DelayPatch
    {
        // Last spawned framecount, separate for each transit type.
        private static uint tramFrame = 0;
        private static uint busFrame = 0;
        private static uint trolleybusFrame = 0;
        private static uint blimpFrame = 0;
        private static uint helicopterFrame = 0;

        // Dictionary of depots.
        private readonly static Dictionary<ushort, uint> depotFrames = new Dictionary<ushort, uint>();


        /// <summary>
        /// Harmony Prefix patch to DepotAI.StartTransfer to enforce a minimum time between tranit vehicles spawning.
        /// </summary>
        /// <param name="__instance">AI instance reference</param>
        /// <param name="reason">Transfer reason</param>
        /// <returns>True (continue on to orignal method) if vehicle can spawn, false (preempt original execution) if spawning is blocked</returns>
        public static bool Prefix(DepotAI __instance, ushort buildingID, TransferManager.TransferReason reason)
        {
            // Only cover offers that match this transit vehicle type.
            if (reason != __instance.m_transportInfo.m_vehicleReason && reason != __instance.m_secondaryTransportInfo.m_vehicleReason)
            {
                // Execute game code.
                return true;
            }

            // Get current frame count.
            uint currentFrame = Singleton<SimulationManager>.instance.m_currentFrameIndex;

            // Get targeted spawn time.
            uint spawnTime = GetSpawnTime(buildingID, reason);

            // Check to see if we've reached minimum spawn frame count.
            if (currentFrame < spawnTime)
            {
                // Elapsed framecount is too low; prevent execution of game method.
                return false;
            }

            // Spawning is successful; update timer with current time as new last-spawned time.
            if (ModSettings.perDepot)
            {
                // Per-depot spawning; update spawning time.
                UpdateDepotFrame(buildingID, currentFrame);
            }
            
            // Update global spawn timer (still do this even if using per-depot in case user changes preferences from depot to global).
            switch (reason)
            {
                case TransferManager.TransferReason.Bus:
                    busFrame = currentFrame;
                    break;

                case TransferManager.TransferReason.Tram:
                    tramFrame = currentFrame;
                    break;

                case TransferManager.TransferReason.Trolleybus:
                    trolleybusFrame = currentFrame;
                    break;

                case TransferManager.TransferReason.PassengerHelicopter:
                    helicopterFrame = currentFrame;
                    break;

                case TransferManager.TransferReason.Blimp:
                    blimpFrame = currentFrame;
                    break;
            }

            // If we got here, we continue on to execute original game code (no delay enforced).
            return true;
        }


        /// <summary>
        /// Gets the current minimum spawn frame for the specified depotID or TransferReason.
        /// </summary>
        /// <param name="depotID"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        internal static uint GetSpawnTime(ushort depotID, TransferManager.TransferReason reason)
        {
            // Handling per transit type.
            switch (reason)
            {
                case TransferManager.TransferReason.Bus:
                    return (ModSettings.perDepot ? GetDepotFrame(depotID) : busFrame) + ModSettings.busDelay;

                case TransferManager.TransferReason.Tram:
                    return (ModSettings.perDepot ? GetDepotFrame(depotID) : tramFrame) + ModSettings.tramDelay;

                case TransferManager.TransferReason.Trolleybus:
                    return (ModSettings.perDepot ? GetDepotFrame(depotID) : trolleybusFrame) + ModSettings.trolleybusDelay;

                case TransferManager.TransferReason.PassengerHelicopter:
                    return (ModSettings.perDepot ? GetDepotFrame(depotID) : helicopterFrame) + ModSettings.helicopterDelay;

                case TransferManager.TransferReason.Blimp:
                    return (ModSettings.perDepot ? GetDepotFrame(depotID) : blimpFrame) + ModSettings.blimpDelay;
                default:
                    return 0;
            }
        }


        /// <summary>
        /// Gets the last stored frame index for the given depot.
        /// </summary>
        /// <param name="depotID">Building ID of depot</param>
        /// <returns>Last stored frame index for the given depot</returns>
        private static uint GetDepotFrame(ushort depotID) => depotFrames.TryGetValue(depotID, out uint frame) ? frame : 0;


        /// <summary>
        /// Stores the given frame index for the given depot.
        /// </summary>
        /// <param name="depotID">Building ID of depot</param>
        /// <param name="frame">Frame index to store</param>
        private static void UpdateDepotFrame(ushort depotID, uint frame)
        {
            // Update the existing entry if we have one, otherwise add a new entry.
            if (depotFrames.ContainsKey(depotID))
            {
                depotFrames[depotID] = frame;
            }
            else
            {
                depotFrames.Add(depotID, frame);
            }    
        }
    }
}