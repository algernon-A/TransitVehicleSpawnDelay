// <copyright file="ModSettings.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace TransitVehicleSpawnDelay
{
    using System.Xml.Serialization;
    using AlgernonCommons.XML;

    /// <summary>
    /// Global mod settings.
    /// </summary>
    [XmlRoot("TransitVehicleSpawnDelay")]
    public class ModSettings : SettingsXMLBase
    {
        // Settings file name
        [XmlIgnore]
        private static readonly string SettingsFileName = "TransitVehicleSpawnDelay.xml";

        /// <summary>
        /// Gets or sets the tram spawning delay framecount.
        /// </summary>
        [XmlElement("TramDelay")]
        public uint XMLTramDelay { get => TramDelay; set => TramDelay = value; }

        /// <summary>
        /// Gets or sets the bus spawning delay framecount.
        /// </summary>
        [XmlElement("BusDelay")]
        public uint XMLBusDelay { get => BusDelay; set => BusDelay = value; }

        /// <summary>
        /// Gets or sets the trolleybus spawning delay framecount.
        /// </summary>
        [XmlIgnore]
        [XmlElement("TrolleybusDelay")]
        public uint XMLTrolleybusDelay { get => TrolleybusDelay; set => TrolleybusDelay = value; }

        /// <summary>
        /// Gets or sets the helicopter spawning delay framecount.
        /// </summary>
        [XmlIgnore]
        [XmlElement("HelicopterDelay")]
        public uint XMLHelicopterDelauy { get => HelicopterDelay; set => HelicopterDelay = value; }

        /// <summary>
        /// Gets or sets the blimp spawning delay framecount.
        /// </summary>
        [XmlElement("BlimpDelay")]
        public uint XMLBlimpDelay { get => BlimpDelay; set => BlimpDelay = value; }

        /// <summary>
        /// Gets or sets a value indicating whether per-depot delay settings are in effect (true) or not (false).
        /// </summary>
        // Use separate timer for each depot.
        [XmlElement("PerDepot")]
        public bool XMLPerDepot { get => PerDepot; set => PerDepot = value; }

        /// <summary>
        /// Gets or sets the tram spawning delay framecount.
        /// </summary>
        [XmlIgnore]
        internal static uint TramDelay { get; set; } = 1024;

        /// <summary>
        /// Gets or sets the bus spawning delay framecount.
        /// </summary>
        [XmlIgnore]
        internal static uint BusDelay { get; set; } = 1024;

        /// <summary>
        /// Gets or sets the trolleybus spawning delay framecount.
        /// </summary>
        [XmlIgnore]
        internal static uint TrolleybusDelay { get; set; } = 1024;

        /// <summary>
        /// Gets or sets the helicopter spawning delay framecount.
        /// </summary>
        [XmlIgnore]
        internal static uint HelicopterDelay { get; set; } = 1024;

        /// <summary>
        /// Gets or sets the blimp spawning delay framecount.
        /// </summary>
        [XmlIgnore]
        internal static uint BlimpDelay { get; set; } = 1024;

        /// <summary>
        /// Gets or sets a value indicating whether per-depot delay settings are in effect (true) or not (false).
        /// </summary>
        [XmlIgnore]
        internal static bool PerDepot { get; set; } = false;

        /// <summary>
        /// Loads settings from file.
        /// </summary>
        internal static void Load() => XMLFileUtils.Load<ModSettings>(SettingsFileName);

        /// <summary>
        /// Saves settings to file.
        /// </summary>
        internal static void Save() => XMLFileUtils.Save<ModSettings>(SettingsFileName);
    }
}