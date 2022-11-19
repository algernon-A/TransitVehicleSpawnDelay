// <copyright file="TimerLabel.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace TransitVehicleSpawnDelay
{
    using System.Text;
    using AlgernonCommons.Translation;
    using ColossalFramework;
    using ColossalFramework.UI;
    using UnityEngine;

    /// <summary>
    /// Custom UILabel class for the timer countdown label.
    /// </summary>
    public sealed class TimerLabel : UILabel
    {
        // Private string references.
        private readonly string blockedString = Translations.Translate("VSD_TIM_BLK");
        private readonly string hourString = Translations.Translate("VSD_TIM_HR");
        private readonly string hoursString = Translations.Translate("VSD_TIM_HRS");
        private readonly string minuteString = Translations.Translate("VSD_TIM_MN");
        private readonly string minutesString = Translations.Translate("VSD_TIM_MNS");

        // Target field.
        private ushort _buildingID;
        private TransferManager.TransferReason _reason;

        /// <summary>
        /// Sets the target building ID.
        /// </summary>
        public ushort BuildingID { set => _buildingID = value; }

        /// <summary>
        /// Sets the countdown transfer reason.
        /// </summary>
        public TransferManager.TransferReason Reason { set => _reason = value; }

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
                int timerValue = (int)(DelayPatch.GetSpawnTime(_buildingID, _reason) - Singleton<SimulationManager>.instance.m_currentFrameIndex);

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
        /// <param name="value">Fame count until spawning is next permitted.</param>
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
