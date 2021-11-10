using ColossalFramework;
using HarmonyLib;


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

        /// <summary>
        /// Harmony Prefix patch to DepotAI.StartTransfer to enforce a minimum time between tranit vehicles spawning.
        /// </summary>
        /// <param name="__instance">AI instance reference</param>
        /// <param name="reason">Transfer reason</param>
        /// <returns>True (continue on to orignal method) if vehicle can spawn, false (preempt original execution) if spawning is blocked</returns>
        public static bool Prefix(DepotAI __instance, TransferManager.TransferReason reason)
        {
            // Only cover offers that match this transit vehicle type.
            if (reason != __instance.m_transportInfo.m_vehicleReason && reason != __instance.m_secondaryTransportInfo.m_vehicleReason)
            {
                // Execute game code.
                return true;
            }

            // Get current frame count.
            uint currentFrame = Singleton<SimulationManager>.instance.m_currentFrameIndex;

            // Handling per transit type.
            switch (reason)
            {
                case TransferManager.TransferReason.Bus:
                    // Check to see if we've reached minimum spawn frame count.
                    if (currentFrame < busFrame + ModSettings.busDelay)
                    {
                        // Elapsed framecount is too low; prevent execution of game method.
                        return false;
                    }

                    // Spawning can proceed; update last spawned frame.
                    busFrame = currentFrame;
                    break;


                case TransferManager.TransferReason.Tram:
                    if (currentFrame < tramFrame + ModSettings.tramDelay)
                    {
                        // Elapsed framecount is too low; prevent execution of game method.
                        return false;
                    }
                    // Spawning can proceed; update last spawned frame.
                    tramFrame = currentFrame;
                    break;


                case TransferManager.TransferReason.Trolleybus:
                    if (currentFrame < trolleybusFrame + ModSettings.trolleybusDelay)
                    {
                        // Elapsed framecount is too low; prevent execution of game method.
                        return false;
                    }
                    // Spawning can proceed; update last spawned frame.
                    trolleybusFrame = currentFrame;
                    break;
            }

            // If we got here, we continue on to execute original game code (no delay enforced).
            return true;
        }
    }
}