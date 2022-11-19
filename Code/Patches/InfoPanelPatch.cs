// <copyright file="InfoPanelPatch.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace TransitVehicleSpawnDelay
{
    using System.Linq;
    using AlgernonCommons;
    using ColossalFramework;
    using ColossalFramework.UI;
    using HarmonyLib;
    using UnityEngine;

    /// <summary>
    /// Harmony Postfix patch to add span countdown timer display to depot info panels.
    /// </summary>
    [HarmonyPatch(typeof(CityServiceWorldInfoPanel), "UpdateBindings")]
    public static class InfoPanelPatch
    {
        // Timer label reference.
        private static TimerLabel s_timerLabel;

        /// <summary>
        /// Harmony Postfix patch to CityServiceWorldInfoPanel.UpdateBindings to set up a countdown timer when spawning is blocked.
        /// </summary>
        public static void Postfix()
        {
            // Currently selected building.
            ushort buildingID = WorldInfoPanel.GetCurrentInstanceID().Building;

            // Create spawn delay label if it isn't already set up.
            if (s_timerLabel == null)
            {
                // Get info panel.
                CityServiceWorldInfoPanel infoPanel = UIView.library.Get<CityServiceWorldInfoPanel>(typeof(CityServiceWorldInfoPanel).Name);

                // Get ParkButtons UIPanel.
                UIComponent wrapper = infoPanel?.Find("Wrapper");
                UIComponent mainSectionPanel = wrapper?.Find("MainSectionPanel");
                UIComponent mainBottom = mainSectionPanel?.Find("MainBottom");
                UIComponent buttonPanels = mainSectionPanel?.Find("ButtonPanels");
                UIComponent parkButtons = mainSectionPanel?.Find("ParkButtons");

                if (parkButtons != null)
                {
                    // Add timer countdown label.
                    s_timerLabel = parkButtons.AddUIComponent<TimerLabel>();
                    s_timerLabel.textScale = 0.75f;
                    s_timerLabel.textColor = new Color32(185, 221, 254, 255);
                    s_timerLabel.font = Resources.FindObjectsOfTypeAll<UIFont>().FirstOrDefault((UIFont f) => f.name == "OpenSans-Regular");
                }
                else
                {
                    Logging.Error("couldn't find CityServiceWorldInfoPanel components");
                    return;
                }
            }

            // Local references.
            Building[] buildingBuffer = Singleton<BuildingManager>.instance.m_buildings.m_buffer;
            BuildingInfo buildingInfo = buildingBuffer[buildingID].Info;

            // Is this a depot building?
            DepotAI depotAI = buildingInfo.GetAI() as DepotAI;
            if (depotAI == null)
            {
                // Not a depot building - hide the label.
                s_timerLabel.Hide();
            }
            else
            {
                // Depot building - show the label.
                s_timerLabel.BuildingID = buildingID;
                s_timerLabel.Reason = depotAI.m_transportInfo.m_vehicleReason;
                s_timerLabel.Show();
            }
        }
    }
}