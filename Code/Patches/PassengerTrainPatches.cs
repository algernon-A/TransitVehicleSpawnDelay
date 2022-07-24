using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ColossalFramework;
using HarmonyLib;


namespace TransitVehicleSpawnDelay
{
    /// <summary>
    /// Harmony patch to add minimum station dwell times for trains.
    /// </summary>
    [HarmonyPatch(typeof(PassengerTrainAI))]
    public static class PassengerTrainPatches
    {
        // Minimum dwell time for trains.
        internal static uint minTrainDwell = 128;

        // Dictionary of vehicles.
        private readonly static Dictionary<ushort, uint> vehicleFrames = new Dictionary<ushort, uint>();


        private readonly static Dictionary<ushort, ushort> currentStops = new Dictionary<ushort, ushort>();


        /// <summary>
        /// Harmony Prefix patch to PassengerTrainAI.CanLeave to enforce a minimum station dwell time..
        /// </summary>
        /// <param name="__instance">PassengerTrainAI instance</param>
        /// <param name="__result">Original method result</param>
        /// <param name="vehicleID">Vehicle ID</param>
        /// <param name="vehicleData">Vehicle data reference</param>
        /// <returns>True (continue on to orignal method) if vehicle can proceed, false (preempt original execution) if proceeding is blocked</returns>
        [HarmonyPatch(nameof(PassengerTrainAI.CanLeave))]
        [HarmonyPrefix]
        public static bool CanLeave(PassengerTrainAI __instance, ref bool __result, ushort vehicleID, ref Vehicle vehicleData)
        {
            // Only interested in leading vehicles.
            if (vehicleData.m_leadingVehicle == 0)
            {
                // Get current frame count.
                uint currentFrame = Singleton<SimulationManager>.instance.m_currentFrameIndex;

                // Get targeted earliest departure time.
                uint spawnTime = GetTrainFrame(vehicleID) + minTrainDwell;

                // Check to see if we've reached minimum delay frame count.
                if (currentFrame < spawnTime)
                {
                    // Elapsed framecount is too low - we're not leaving yet.
                    // Continue to load and passengers while we wait.
                    LoadPassengers(__instance, vehicleID, ref vehicleData, currentStops[vehicleID], vehicleData.m_targetBuilding);

                    // Set return value and prevent execution of game method.
                    __result = false;
                    return false;
                }
            }

            // If we got here, we're okay to proceed and continue on to execute original game code.
            return true;
        }


        /// <summary>
        /// Harmony Prefix patch to PassengerTrainAI.ArriveAtTarget to record train arrival times.
        /// </summary>
        /// <param name="vehicleID">Vehicle ID</param>
        /// <param name="vehicleID">Vehicle data record</param>
        [HarmonyPatch("ArriveAtTarget")]
        [HarmonyPrefix]
        public static void ArriveAtTarget(ushort vehicleID, ref Vehicle data)
        {
            vehicleFrames[vehicleID] = Singleton<SimulationManager>.instance.m_currentFrameIndex;
            currentStops[vehicleID] = data.m_targetBuilding;
        }


        /// <summary>
        /// Gets the last stored frame index for the given vehicle.
        /// </summary>
        /// <param name="vehicleID">Vehicle ID</param>
        /// <returns>Last stored frame index for the given vehicle</returns>
        private static uint GetTrainFrame(ushort vehicleID) => vehicleFrames.TryGetValue(vehicleID, out uint frame) ? frame : 0;


        /// <summary>
        /// Harmony reverse patch stub to access private method PassengerTrainAI.LoadPassengers.
        /// </summary>
        /// <param name="instance">PassengerTrainAI instance</param>
        /// <param name="vehicleID">Vehicle ID</param>
        /// <param name="vehicleData">Vehicle data reference</param>
        /// <param name="currentStop">Current stop (building) ID</param>
        /// <param name="nextStop">Next stop (building) ID</param>
        /// <exception cref="NotImplementedException">Thrown if reverse patch has failed</exception>
        [HarmonyReversePatch]
        [HarmonyPatch(typeof(PassengerTrainAI), "LoadPassengers")]
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void LoadPassengers(PassengerTrainAI instance, ushort vehicleID, ref Vehicle data, ushort currentStop, ushort nextStop)
        {
            Logging.Error("LoadPassengers reverse Harmony patch wasn't applied with params ", instance, vehicleID, data, currentStop, nextStop);
            throw new NotImplementedException();
        }
    }
}