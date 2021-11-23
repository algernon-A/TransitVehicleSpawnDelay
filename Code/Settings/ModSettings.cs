using System;
using System.IO;
using System.Xml.Serialization;


namespace TransitVehicleSpawnDelay
{
    /// <summary>
    /// Global mod settings.
    /// </summary>
	[XmlRoot("TransitVehicleSpawnDelay")]
    public class ModSettings
    {
        // Framecount delays, separate for each transit type.
        [XmlIgnore]
        internal static uint tramDelay = 1024;

        [XmlIgnore]
        internal static uint busDelay = 1024;

        [XmlIgnore]
        internal static uint trolleybusDelay = 1024;

        [XmlIgnore]
        internal static uint helicopterDelay = 1024;

        [XmlIgnore]
        internal static uint blimpDelay = 1024;


        [XmlIgnore]
        internal static bool perDepot = false;


        // Settings file name
        [XmlIgnore]
        private static readonly string SettingsFileName = "TransitVehicleSpawnDelay.xml";


        // Language.
        [XmlElement("Language")]
        public string Language
        {
            get => Translations.Language;

            set => Translations.Language = value;
        }


        // Framecount delays, separate for each transit type.
        [XmlElement("TramDelay")]
        public uint XMLTramDelay { get => tramDelay; set => tramDelay = value; }

        [XmlElement("BusDelay")]
        public uint XMLBusDelay { get => busDelay; set => busDelay = value; }

        [XmlElement("TrolleybusDelay")]
        public uint XMLTrolleybusDelay { get => trolleybusDelay; set => trolleybusDelay = value; }

        [XmlElement("HelicopterDelay")]
        public uint XMLHelicopterDelauy { get => helicopterDelay; set => helicopterDelay = value; }

        [XmlElement("BlimpDelay")]
        public uint XMLBlimpDelay { get => blimpDelay; set => blimpDelay = value; }


        // Use separate timer for each depot.
        [XmlElement("PerDepot")]
        public bool XMLPerDepot { get => perDepot; set => perDepot = value; }


        /// <summary>
        /// Load settings from XML file.
        /// </summary>
        internal static void Load()
        {
            try
            {
                // Check to see if configuration file exists.
                if (File.Exists(SettingsFileName))
                {
                    // Read it.
                    using (StreamReader reader = new StreamReader(SettingsFileName))
                    {
                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(ModSettings));
                        if (!(xmlSerializer.Deserialize(reader) is ModSettings ZoningModSettingsFile))
                        {
                            Logging.Error("couldn't deserialize settings file");
                        }
                    }
                }
                else
                {
                    Logging.Message("no settings file found");
                }
            }
            catch (Exception e)
            {
                Logging.LogException(e, "exception reading XML settings file");
            }
        }


        /// <summary>
        /// Save settings to XML file.
        /// </summary>
        internal static void Save()
        {
            try
            {
                // Pretty straightforward.  Serialisation is within GBRSettingsFile class.
                using (StreamWriter writer = new StreamWriter(SettingsFileName))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(ModSettings));
                    xmlSerializer.Serialize(writer, new ModSettings());
                }
            }
            catch (Exception e)
            {
                Logging.LogException(e, "exception saving XML settings file");
            }
        }
    }
}