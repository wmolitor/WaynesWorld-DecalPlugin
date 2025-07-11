/* Plugin created by $username$ $year$ */

using Decal.Adapter.Wrappers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;


namespace WaynesWorld
{
    public partial class PluginCore
    {
        public PluginSettings pluginSettings;
        // Default health for auto-heal threshold
        private const int AUTO_HEAL_THRESHOLD = 50;

        private void loadSettings()
        {
            pluginSettings = PluginSettings.load(settingsFolder + DIR_SEP + FILENAME_SETTINGS, errorLogFile);
            initSettings();
            logSettings();
        }

        private void saveSettings()
        {
            pluginSettings.save(settingsFolder + DIR_SEP + FILENAME_SETTINGS, errorLogFile);
        }

        private void initSettings()
        {
            /* INITIALISE THE STATE OF VIEW ELEMENTS HERE */
        }

        private void logSettings()
        {
            if (pluginSettings == null || pluginSettings.Items == null)
            {
                WriteToChat("[FSM][SETTINGS] Plugin settings are null. Please check your settings file.");
                return;
            }
            WriteToChat("[FSM][SETTINGS] Plugin Settings:");
            foreach (var item in pluginSettings.Items)
            {
                WriteToChat($"   Item: {item.RuleName}, Regex: {item.Regex}, Enabled: {item.Enabled}");
            }
            return;
        }


        public class PluginSettings
        {
            /* ADD PUBLIC PROPERTIES FOR PERSISTABLE SETTINGS HERE */
            /* The serializer only works with public properties (or fields) that have getters and setters */


                [XmlArray("items")]
                [XmlArrayItem("item")]
                public List<ItemRule> Items { get; set; } = new List<ItemRule>();

            public PluginSettings()
            {
                /* SET DEFAULTS FOR THE PROPERTIES HERE */
            }

            public static PluginSettings load(string file, string errorLogFile)
            {
                try
                {
                    if (File.Exists(file))
                    {
                        using (FileStream myFileStream = new FileStream(file, FileMode.Open))
                        {
                            XmlSerializer mySerializer = new XmlSerializer(typeof(PluginSettings));
                            PluginSettings mySettings = (PluginSettings)mySerializer.Deserialize(myFileStream);
                            myFileStream.Close();
                            return mySettings;
                        }

                    }

                }
                catch (Exception ex)
                {
                    ErrorLogging.LogError(errorLogFile, ex);
                }
                return new PluginSettings();
            }

            public void save(string file, string errorLogFile)
            {
                try
                {
                    using (StreamWriter myWriter = new StreamWriter(file))
                    {
                        XmlSerializer mySerializer = new XmlSerializer(typeof(PluginSettings));
                        myWriter.AutoFlush = true;
                        mySerializer.Serialize(myWriter, this);
                        myWriter.Close();
                    }
                }
                catch (Exception ex)
                {
                    ErrorLogging.LogError(errorLogFile, ex);
                }
            }
        }
    }
}

/*
 * return string.Join(";",
        worldObject.SpellCount,
        worldObject.Behavior,
        worldObject.Category,
        worldObject.Container,
        worldObject.Id,
        worldObject.Name,
        worldObject.ObjectClass?.ToString() ?? "",
        worldObject.Type,
        worldObject.Values(LongValueKey.Value),
        worldObject.Values(LongValueKey.Material)
    );

*/