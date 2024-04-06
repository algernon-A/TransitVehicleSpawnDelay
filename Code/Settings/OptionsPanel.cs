// <copyright file="OptionsPanel.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace TransitVehicleSpawnDelay
{
    using System.Text;
    using AlgernonCommons.Translation;
    using AlgernonCommons.UI;
    using ColossalFramework.UI;

    /// <summary>
    /// TVSD options panel.
    /// </summary>
    public class OptionsPanel : OptionsPanelBase
    {
        // Layout constants.
        private const float Margin = 5f;
        private const float LeftMargin = 24f;
        private const float GroupMargin = 30f;

        // Slider display string caching.
        private readonly string approxString = Translations.Translate("VSD_TIM_APP");
        private readonly string hourString = Translations.Translate("VSD_TIM_HR");
        private readonly string hoursString = Translations.Translate("VSD_TIM_HRS");
        private readonly string minuteString = Translations.Translate("VSD_TIM_MN");
        private readonly string minutesString = Translations.Translate("VSD_TIM_MNS");
        private readonly string secondsNormalString = Translations.Translate("VSD_TIM_SNS");

        /// <summary>
        /// Performs on-demand panel setup.
        /// </summary>
        protected override void Setup()
        {
            // Size and placement.
            width = width - (relativePosition.x * 2);
            height = height - (relativePosition.y * 2);
            autoLayout = false;

            // Add controls.
            // Y position indicator.
            float currentY = Margin;

            // Language choice.
            UIDropDown languageDropDown = UIDropDowns.AddPlainDropDown(this, LeftMargin, currentY, Translations.Translate("TRN_CHOICE"), Translations.LanguageList, Translations.Index);
            languageDropDown.eventSelectedIndexChanged += (control, index) =>
            {
                Translations.Index = index;
                OptionsPanelManager<OptionsPanel>.LocaleChanged();
            };
            currentY += languageDropDown.parent.height + Margin;

            // Per-depot timing checkbox.
            UICheckBox perDepotCheck = UICheckBoxes.AddPlainCheckBox(this, Margin, currentY, Translations.Translate("VSD_DEP_PER"));
            perDepotCheck.isChecked = ModSettings.PerDepot;
            perDepotCheck.eventCheckChanged += (control, value) => { ModSettings.PerDepot = value; };
            currentY += perDepotCheck.height + GroupMargin;

            // Bus delay slider.
            UISlider busSlider = AddDelaySlider(ref currentY, "VSD_BUS_DEL", ModSettings.BusDelay);
            busSlider.eventValueChanged += (control, value) => { ModSettings.BusDelay = (uint)value; };

            // Tram delay slider.
            UISlider tramSlider = AddDelaySlider(ref currentY, "VSD_TRA_DEL", ModSettings.TramDelay);
            tramSlider.eventValueChanged += (control, value) => { ModSettings.TramDelay = (uint)value; };

            // Trolleybus delay slider.
            UISlider trolleybusSlider = AddDelaySlider(ref currentY, "VSD_TRO_DEL", ModSettings.TrolleybusDelay);
            trolleybusSlider.eventValueChanged += (control, value) => { ModSettings.TrolleybusDelay = (uint)value; };

            // Helicopter delay slider.
            UISlider helicopterSlider = AddDelaySlider(ref currentY, "VSD_HEL_DEL", ModSettings.HelicopterDelay);
            helicopterSlider.eventValueChanged += (control, value) => { ModSettings.HelicopterDelay = (uint)value; };

            // Trolleybus delay slider.
            UISlider blimpSlider = AddDelaySlider(ref currentY, "VSD_BLI_DEL", ModSettings.BlimpDelay);
            blimpSlider.eventValueChanged += (control, value) => { ModSettings.BlimpDelay = (uint)value; };
        }

        /// <summary>
        /// Adds a delay slider.
        /// </summary>
        /// <param name="yPos">Relative y-position indicator (will be incremented with slider height.</param>
        /// <param name="labelKey">Translation key for slider label.</param>
        /// <param name="initialValue">Initial slider value.</param>
        /// <returns>New delay slider with attached game-time label.</returns>
        private UISlider AddDelaySlider(ref float yPos, string labelKey, uint initialValue)
        {
            // Create new slider.
            UISlider newSlider = UISliders.AddPlainSliderWithValue(this, Margin, yPos, Translations.Translate(labelKey), 1f, 16636f, 1f, initialValue);

            // Game-time label.
            UILabel timeLabel = UILabels.AddLabel(newSlider.parent, Margin, 50f, string.Empty);
            newSlider.objectUserData = timeLabel;

            // Force set slider value to populate initial time label and add event handler.
            SetTimeLabel(newSlider, initialValue);
            newSlider.eventValueChanged += SetTimeLabel;

            // Increment y position indicator.
            yPos += newSlider.parent.height + timeLabel.height;

            return newSlider;
        }

        /// <summary>
        /// Sets the game-time label for a delay slider.
        /// </summary>
        /// <param name="c">Calling component.</param>
        /// <param name="value">Slider value.</param>
        private void SetTimeLabel(UIComponent c, float value)
        {
            // Ensure that there's a valid label attached to the slider.
            if (c.objectUserData is UILabel label)
            {
                // Comvert frame count to hours per current SimulationManager settings.
                System.TimeSpan timespan = System.TimeSpan.FromHours(value / SimulationManager.DAYTIME_HOUR_TO_FRAME);

                // Format label to display hours and minutes.
                StringBuilder labelString = new StringBuilder(((uint)value >> 6).ToString());
                labelString.Append(' ');
                labelString.Append(secondsNormalString);
                labelString.Append(System.Environment.NewLine);
                labelString.Append(approxString);
                labelString.Append(' ');
                labelString.Append(timespan.Hours);
                labelString.Append(' ');
                labelString.Append(timespan.Hours == 1 ? hourString : hoursString);
                labelString.Append(' ');
                labelString.Append(timespan.Minutes);
                labelString.Append(' ');
                labelString.Append(timespan.Minutes == 1 ? minuteString : minutesString);
                labelString.Append(' ');

                label.text = labelString.ToString();
            }
        }
    }
}