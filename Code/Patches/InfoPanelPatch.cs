using System.Text;
using System.Linq;
using UnityEngine;
using ColossalFramework;
using ColossalFramework.UI;
using HarmonyLib;


namespace TransitVehicleSpawnDelay
{
    /// <summary>
    /// Harmony Postfix patch to add span countdown timer display to depot info panels.
    /// </summary>
    [HarmonyPatch(typeof(CityServiceWorldInfoPanel), "UpdateBindings")]
    public static class InfoPanelPatch
    {
        // Timer label reference.
        public static TimerLabel timerLabel;


        /// <summary>
        /// Harmony Postfix patch to CityServiceWorldInfoPanel.UpdateBindings to set up a countdown timer when spawning is blocked.
        /// </summary>
        public static void Postfix()
        {
            // Currently selected building.
            ushort buildingID = WorldInfoPanel.GetCurrentInstanceID().Building;

            // Create spawn delay label if it isn't already set up.
            if (timerLabel == null)
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
                    timerLabel = parkButtons.AddUIComponent<TimerLabel>();
                    timerLabel.textScale = 0.75f;
                    timerLabel.textColor = new Color32(185, 221, 254, 255);
                    timerLabel.font = Resources.FindObjectsOfTypeAll<UIFont>().FirstOrDefault((UIFont f) => f.name == "OpenSans-Regular");
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
                timerLabel.Hide();
            }
            else
            {
                // Depot building - show the label.
                timerLabel.buildingID = buildingID;
                timerLabel.reason = depotAI.m_transportInfo.m_vehicleReason;
                timerLabel.Show();
            }
        }
    }


    /// <summary>
    /// Custom UILabel class for the timer countdown label.
    /// </summary>
    public class TimerLabel : UILabel
    {
        // Private string references.
        private readonly string blockedString = Translations.Translate("VSD_TIM_BLK");
        private readonly string hourString = Translations.Translate("VSD_TIM_HR");
        private readonly string hoursString = Translations.Translate("VSD_TIM_HRS");
        private readonly string minuteString = Translations.Translate("VSD_TIM_MN");
        private readonly string minutesString = Translations.Translate("VSD_TIM_MNS");

        // Target IDs.
        public ushort buildingID;
        public TransferManager.TransferReason reason;


        /// <summary>
        /// Updates the label display.
        /// Called by game every update.
        /// </summary>
        public override void Update()
        {
            // Don't do anything if not visible.
            if (m_IsVisible)
            {
                // Calculate time delta.
                int timerValue = (int)(DelayPatch.GetSpawnTime(buildingID, reason) - Singleton<SimulationManager>.instance.m_currentFrameIndex);

                // If time delta is less than zero, then clear display text.
                if (timerValue < 0)
                {
                    timerValue = 0;
                    this.text = string.Empty;
                }
                else
                {
                    // Set timer display text.
                    this.text = SetTimerLabel(timerValue);

                    // Set label positon based on current label dimensions.
                    this.relativePosition = new Vector2(this.parent.width - this.width, (this.parent.height - this.height) / 2f);
                }
            }

            base.Update();
        }


        /// <summary>
        /// Sets the text for the spawn timer.
        /// </summary>
        /// <param name="targetFrame"></param>
        private string SetTimerLabel(int value)
        {
            // Comvert frame count to hours per current SimulationManager settings.
            System.TimeSpan timespan = System.TimeSpan.FromHours(value / SimulationManager.DAYTIME_HOUR_TO_FRAME);

            // Format label to display hours and minutes.
            StringBuilder labelString = new StringBuilder(blockedString);
            labelString.Append(" ");
            labelString.Append(timespan.Hours);
            labelString.Append(" ");
            labelString.Append(timespan.Hours == 1 ? hourString : hoursString);
            labelString.Append(" ");
            labelString.Append(timespan.Minutes);
            labelString.Append(" ");
            labelString.Append(timespan.Minutes == 1 ? minuteString : minutesString);
            return labelString.ToString();
        }
    }
}