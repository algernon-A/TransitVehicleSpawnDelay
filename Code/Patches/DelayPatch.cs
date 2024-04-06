// <copyright file="DelayPatch.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace TransitVehicleSpawnDelay
{
    using System.Collections.Generic;
    using ColossalFramework;
    using HarmonyLib;

    /// <summary>
    /// Harmony patch to add delays between spawining of public transit vehicles.
    /// Priority is to run before IPT2 and TLM to ensure no conflict.
    /// </summary>
    [HarmonyPatch(typeof(DepotAI), nameof(DepotAI.StartTransfer))]
    [HarmonyBefore("github.com/bloodypenguin/ImprovedPublicTransport", "com.klyte.redirectors.TLM", "com.redirectors.TLM")]
    public static class DelayPatch
    {
        // Dictionary of depots and framecounts.
        private static readonly Dictionary<ushort, uint> DepotFrames = new Dictionary<ushort, uint>();

        // Last spawned framecount, separate for each transit type.
        private static uint tramFrame = 0;
        private static uint busFrame = 0;
        private static uint trolleybusFrame = 0;
        private static uint blimpFrame = 0;
        private static uint helicopterFrame = 0;

        /// <summary>
        /// Harmony Prefix patch to DepotAI.StartTransfer to enforce a minimum time between tranit vehicles spawning.
        /// </summary>
        /// <param name="__instance">AI instance reference.</param>
        /// <param name="buildingID">Building ID.</param>
        /// <param name="reason">Transfer reason.</param>
        /// <returns>True (continue on to orignal method) if vehicle can spawn, false (preempt original execution) if spawning is blocked.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony")]
        public static bool Prefix(DepotAI __instance, ushort buildingID, TransferManager.TransferReason reason)
        {
            // Convert any BiofuelBus reason to normal bus for our purposes.
            TransferManager.TransferReason thisReason = reason == TransferManager.TransferReason.BiofuelBus ? TransferManager.TransferReason.Bus : reason;

            // Only cover offers that match this transit vehicle type.
            if ((__instance.m_transportInfo == null || thisReason != __instance.m_transportInfo.m_vehicleReason) && (__instance.m_secondaryTransportInfo == null || thisReason != __instance.m_secondaryTransportInfo.m_vehicleReason))
            {
                // Execute game code.
                return true;
            }

            // Get current frame count.
            uint currentFrame = Singleton<SimulationManager>.instance.m_currentFrameIndex;

            // Get targeted spawn time.
            uint spawnTime = GetSpawnTime(buildingID, thisReason);

            // Check to see if we've reached minimum spawn frame count.
            if (currentFrame < spawnTime)
            {
                // Elapsed framecount is too low; prevent execution of game method.
                return false;
            }

            // Spawning is successful; update timer with current time as new last-spawned time.
            if (ModSettings.PerDepot)
            {
                // Per-depot spawning; update spawning time.
                DepotFrames[buildingID] = currentFrame;
            }

            // Update global spawn timer (still do this even if using per-depot in case user changes preferences from depot to global).
            switch (thisReason)
            {
                case TransferManager.TransferReason.Bus:
                case TransferManager.TransferReason.BiofuelBus:
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
        /// <param name="depotID">Depot buildingID.</param>
        /// <param name="reason">Transfer reason.</param>
        /// <returns>Current minimum spawn time.</returns>
        internal static uint GetSpawnTime(ushort depotID, TransferManager.TransferReason reason)
        {
            // Handling per transit type.
            switch (reason)
            {
                case TransferManager.TransferReason.Bus:
                    return (ModSettings.PerDepot ? GetDepotFrame(depotID) : busFrame) + ModSettings.BusDelay;

                case TransferManager.TransferReason.Tram:
                    return (ModSettings.PerDepot ? GetDepotFrame(depotID) : tramFrame) + ModSettings.TramDelay;

                case TransferManager.TransferReason.Trolleybus:
                    return (ModSettings.PerDepot ? GetDepotFrame(depotID) : trolleybusFrame) + ModSettings.TrolleybusDelay;

                case TransferManager.TransferReason.PassengerHelicopter:
                    return (ModSettings.PerDepot ? GetDepotFrame(depotID) : helicopterFrame) + ModSettings.HelicopterDelay;

                case TransferManager.TransferReason.Blimp:
                    return (ModSettings.PerDepot ? GetDepotFrame(depotID) : blimpFrame) + ModSettings.BlimpDelay;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Gets the last stored frame index for the given depot.
        /// </summary>
        /// <param name="depotID">Building ID of depot.</param>
        /// <returns>Last stored frame index for the given depot.</returns>
        private static uint GetDepotFrame(ushort depotID) => DepotFrames.TryGetValue(depotID, out uint frame) ? frame : 0;
    }
}