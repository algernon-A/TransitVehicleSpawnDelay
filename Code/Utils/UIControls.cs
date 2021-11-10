using UnityEngine;
using ColossalFramework.UI;


namespace TransitVehicleSpawnDelay
{
    
    /// <summary>
    /// Static utilities class for creating UI controls.
    /// </summary>
    public static class UIControls
    {
        /// <summary>
        /// Adds a plain text label to the specified UI component.
        /// </summary>
        /// <param name="parent">Parent component</param>
        /// <param name="xPos">Relative x position)</param>
        /// <param name="yPos">Relative y position</param>
        /// <param name="text">Label text</param>
        /// <param name="width">Label width (-1 (default) for autosize)</param>
        /// <param name="width">Text scale (default 1.0)</param>
        /// <returns>New text label</returns>
        public static UILabel AddLabel(UIComponent parent, float xPos, float yPos, string text, float width = -1f, float textScale = 1.0f)
        {
            // Add label.
            UILabel label = (UILabel)parent.AddUIComponent<UILabel>();

            // Set sizing options.
            if (width > 0f)
            {
                // Fixed width.
                label.autoSize = false;
                label.width = width;
                label.autoHeight = true;
                label.wordWrap = true;
            }
            else
            {
                // Autosize.
                label.autoSize = true;
                label.autoHeight = false;
                label.wordWrap = false;
            }

            // Text.
            label.textScale = textScale;
            label.text = text;

            // Position.
            label.relativePosition = new Vector2(xPos, yPos);

            return label;
        }


        /// <summary>
        /// Creates a plain dropdown using the game's option panel dropdown template.
        /// </summary>
        /// <param name="parent">Parent component</param>
        /// <param name="text">Descriptive label text</param>
        /// <param name="items">Dropdown menu item list</param>
        /// <param name="selectedIndex">Initially selected index (default 0)</param>
        /// <param name="width">Width of dropdown (default 60)</param>
        /// <returns>New dropdown menu using game's option panel template</returns>
        public static UIDropDown AddPlainDropDown(UIComponent parent, string text, string[] items, int selectedIndex = 0, float width = 270f)
        {
            UIPanel panel = parent.AttachUIComponent(UITemplateManager.GetAsGameObject("OptionsDropdownTemplate")) as UIPanel;
            UIDropDown dropDown = panel.Find<UIDropDown>("Dropdown");

            // Set text.
            panel.Find<UILabel>("Label").text = text;

            // Slightly increase width.
            dropDown.autoSize = false;
            dropDown.width = width;

            // Add items.
            dropDown.items = items;
            dropDown.selectedIndex = selectedIndex;

            return dropDown;
        }


        /// <summary>
        /// Adds a slider with a descriptive text label above.
        /// </summary>
        /// <param name="parent">Panel to add the control to</param>
        /// <param name="text">Descriptive label text</param>
        /// <param name="min">Slider minimum value</param>
        /// <param name="max">Slider maximum value</param>
        /// <param name="step">Slider minimum step</param>
        /// <param name="defaultValue">Slider initial value</param>
        /// <param name="width">Slider width (excluding value label to right) (default 600)</param>
        /// <returns>New UI slider with attached labels</returns>
        public static UISlider AddSlider(UIComponent parent, string text, float min, float max, float step, float defaultValue, float width = 600f)
        {
            // Add slider component.
            UIPanel sliderPanel = parent.AttachUIComponent(UITemplateManager.GetAsGameObject("OptionsSliderTemplate")) as UIPanel;

            // Label.
            UILabel sliderLabel = sliderPanel.Find<UILabel>("Label");
            sliderLabel.autoHeight = true;
            sliderLabel.width = width;
            sliderLabel.anchor = UIAnchorStyle.Left | UIAnchorStyle.Top;
            sliderLabel.relativePosition = Vector3.zero;
            sliderLabel.relativePosition = Vector3.zero;
            sliderLabel.text = text;

            // Slider configuration.
            UISlider newSlider = sliderPanel.Find<UISlider>("Slider");
            newSlider.minValue = min;
            newSlider.maxValue = max;
            newSlider.stepSize = step;
            newSlider.value = defaultValue;

            // Move default slider position to match resized label.
            sliderPanel.autoLayout = false;
            newSlider.anchor = UIAnchorStyle.Left | UIAnchorStyle.Top;
            newSlider.relativePosition = PositionUnder(sliderLabel);
            newSlider.width = width;

            // Increase height of panel to accomodate it all plus some extra space for margin.
            sliderPanel.autoSize = false;
            sliderPanel.width = width + 50f;
            sliderPanel.height = newSlider.relativePosition.y + newSlider.height + 20f;

            return newSlider;
        }


        /// <summary>
        /// Adds a slider with a descriptive text label above and an automatically updating value label immediately to the right.
        /// </summary>
        /// <param name="parent">Panel to add the control to</param>
        /// <param name="text">Descriptive label text</param>
        /// <param name="min">Slider minimum value</param>
        /// <param name="max">Slider maximum value</param>
        /// <param name="step">Slider minimum step</param>
        /// <param name="defaultValue">Slider initial value</param>
        /// <param name="width">Slider width (excluding value label to right) (default 600)</param>
        /// <returns>New UI slider with attached labels</returns>
        public static UISlider AddSliderWithValue(UIComponent parent, string text, float min, float max, float step, float defaultValue, float width = 600f)
        {
            // Add slider component.
            UISlider newSlider = AddSlider(parent, text, min, max, step, defaultValue, width);
            UIPanel sliderPanel = (UIPanel)newSlider.parent;

            // Value label.
            UILabel valueLabel = sliderPanel.AddUIComponent<UILabel>();
            valueLabel.name = "ValueLabel";
            valueLabel.text = newSlider.value.ToString();
            valueLabel.relativePosition = PositionRightOf(newSlider, 8f, 1f);

            // Event handler to update value label.
            newSlider.eventValueChanged += (component, value) =>
            {
                valueLabel.text = value.ToString();
            };

            return newSlider;
        }


        /// <summary>
        /// Returns a relative position to the right of a specified UI component, suitable for placing an adjacent component.
        /// </summary>
        /// <param name="uIComponent">Original (anchor) UI component</param>
        /// <param name="margin">Margin between components (default 8)</param>
        /// <param name="verticalOffset">Vertical offset from first to second component (default 0)</param>
        /// <returns>Offset position (to right of original)</returns>
        public static Vector3 PositionRightOf(UIComponent uIComponent, float margin = 8f, float verticalOffset = 0f)
        {
            return new Vector3(uIComponent.relativePosition.x + uIComponent.width + margin, uIComponent.relativePosition.y + verticalOffset);
        }


        /// <summary>
        /// Returns a relative position below a specified UI component, suitable for placing an adjacent component.
        /// </summary>
        /// <param name="uIComponent">Original (anchor) UI component</param>
        /// <param name="margin">Margin between components (default 8)</param>
        /// <param name="horizontalOffset">Horizontal offset from first to second component (default 0)</param>
        /// <returns>Offset position (below original)</returns>
        public static Vector3 PositionUnder(UIComponent uIComponent, float margin = 8f, float horizontalOffset = 0f)
        {
            return new Vector3(uIComponent.relativePosition.x + horizontalOffset, uIComponent.relativePosition.y + uIComponent.height + margin);
        }
    }
}