using AlgernonCommons;
using AlgernonCommons.Patching;
using AlgernonCommons.Translation;
using AlgernonCommons.UI;
using ColossalFramework.UI;
using ICities;


namespace TransitVehicleSpawnDelay
{
    public class Mod : PatcherMod, IUserMod
    {
        public static string ModName => "Transit Vehicle Spawn Delay";
        public static string Version => AssemblyUtils.TrimmedCurrentVersion;

        public override string LogName => ModName;
        public override string HarmonyID => "com.github.algernon-A.csl.tvsd";

        public string Name => ModName + " " + Version;
        public string Description => Translations.Translate("VSD_DESC");


        /// <summary>
        /// Called by the game when the mod is enabled.
        /// </summary>
        public override void OnEnabled()
        {
            base.OnEnabled();

            // Add the options panel event handler for the start screen (to enable/disable options panel based on visibility).
            // First, check to see if UIView is ready.
            if (UIView.GetAView() != null)
            {
                // It's ready - attach the hook now.
                OptionsPanelManager<OptionsPanel>.OptionsEventHook();
            }
            else
            {
                // Otherwise, queue the hook for when the intro's finished loading.
                LoadingManager.instance.m_introLoaded += OptionsPanelManager<OptionsPanel>.OptionsEventHook;
            }
        }


        /// <summary>
        /// Called by the game when the mod options panel is setup.
        /// </summary>
        public void OnSettingsUI(UIHelperBase helper)
        {
            // Create options panel.
            OptionsPanelManager<OptionsPanel>.Setup(helper);
        }


        /// <summary>
        /// Load mod settings.
        /// </summary>
        public override void LoadSettings() => ModSettings.Load();


        /// <summary>
        /// Saves mod settings.
        /// </summary>
        public override void SaveSettings() => ModSettings.Save();
    }
}
