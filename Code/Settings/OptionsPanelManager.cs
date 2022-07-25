using System;
using UnityEngine;
using ICities;
using ColossalFramework.UI;
using ColossalFramework.Globalization;


namespace TransitVehicleSpawnDelay
{
    /// <summary>
    /// Class to handle the mod's options panel.
    /// </summary>
    internal static class OptionsPanelManager
    {
        // Parent UI panel reference.
        internal static UIScrollablePanel optionsParentPanel;
        private static UIPanel gameOptionsPanel;

        // Instance references.
        private static GameObject optionsGameObject;
        private static OptionsPanel optionsPanel;

        // Accessors.
        internal static OptionsPanel Panel => optionsPanel;
        internal static bool IsOpen => optionsGameObject != null;


        /// <summary>
        /// Options panel setup.
        /// </summary>
        /// <param name="helper">UIHelperBase parent</param>
        internal static void Setup(UIHelperBase helper)
        {
            // Set up tab strip and containers.
            optionsParentPanel = ((UIHelper)helper).self as UIScrollablePanel;
            optionsParentPanel.autoLayout = false;
        }


        /// <summary>
        /// Attaches an event hook to options panel visibility, to enable/disable mod hokey when the panel is open.
        /// </summary>
        internal static void OptionsEventHook()
        {
            // Get options panel instance.
            gameOptionsPanel = UIView.library.Get<UIPanel>("OptionsPanel");

            if (gameOptionsPanel == null)
            {
                Logging.Error("couldn't find OptionsPanel");
            }
            else
            {
                // Simple event hook to create/destroy GameObject based on appropriate visibility.
                gameOptionsPanel.eventVisibilityChanged += (control, isVisible) =>
                {
                    // Create/destroy based on whether or not we're now visible.
                    if (isVisible)
                    {
                        Create();
                    }
                    else
                    {
                        Close();

                        // Save settings on close.
                        ModSettings.Save();
                    }
                };

                // Recreate panel on system locale change.
                LocaleManager.eventLocaleChanged += LocaleChanged;
            }
        }


        /// <summary>
        /// Refreshes the options panel (destroys and rebuilds) on a locale change when the options panel is open.
        /// </summary>
        internal static void LocaleChanged()
        {
            if (gameOptionsPanel != null && gameOptionsPanel.isVisible)
            {
                Logging.KeyMessage("changing locale");

                Close();
                Create();
            }
        }


        /// <summary>
        /// Creates the panel object in-game and displays it.
        /// </summary>
        private static void Create()
        {
            try
            {
                // If no instance already set, create one.
                if (optionsGameObject == null)
                {
                    // Give it a unique name for easy finding with ModTools.
                    optionsGameObject = new GameObject("VSDOptionsPanel");
                    optionsGameObject.transform.parent = optionsParentPanel.transform;

                    // Attach to game options panel.
                    optionsGameObject.transform.parent = optionsParentPanel.transform;

                    // Create a base panel attached to our game object, perfectly overlaying the game options panel.
                    optionsPanel = optionsGameObject.AddComponent<OptionsPanel>();
                    optionsPanel.width = optionsParentPanel.width;
                    optionsPanel.height = 725f;

                    // Needed to ensure position is consistent if we regenerate after initial opening (e.g. on language change).
                    optionsPanel.relativePosition = new Vector2(10f, 10f);
                }
            }
            catch (Exception e)
            {
                Logging.LogException(e, "exception creating options panel");
            }
        }


        /// <summary>
        /// Closes the panel by destroying the object (removing any ongoing UI overhead).
        /// </summary>
        private static void Close()
        {
            // Save settings on close.
            ModSettings.Save();

            // We're no longer visible - destroy our game object.
            if (optionsGameObject != null)
            {
                GameObject.Destroy(optionsPanel);
                GameObject.Destroy(optionsGameObject);

                // Release references.
                optionsPanel = null;
                optionsGameObject = null;
            }
        }
    }
}