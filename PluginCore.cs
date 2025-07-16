/* Plugin template by Timo 'lino' Kissing <http://ac.ranta.info/DecalDev> */
/* Original template by Lonewolf <http://www.the-lonewolf.com> */

using Decal.Adapter;
using Decal.Adapter.Wrappers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Runtime.CompilerServices;
using System.Timers;

namespace WaynesWorld
{
    public partial class PluginCore : PluginBase
    {
        private static string DIR_SEP = "\\";
        private static string PLUGIN = "WaynesWorld";

        private static string FILENAME_ERRORLOG = "errorlog.txt";
        private static string FILENAME_SETTINGS = "settings.xml";

        List<WorldObject> seekList = new List<WorldObject>();
        bool loginComplete = false;
        bool autoLootEnabled = false;
        public bool isLogLocked = false; // used to prevent re-entrancy issues with logging

        private Timer lootTimer;
        private int isBusy => CoreManager.Current.Actions.BusyState;
        private Queue<Action> actionQueue = new Queue<Action>();

        private AutoLootStateMachine autoLootStateMachine;

        public string settingsFolder
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + DIR_SEP + "Decal Plugins" + DIR_SEP + PLUGIN;
            }
        }

        public string errorLogFile { get { return settingsFolder + DIR_SEP + FILENAME_ERRORLOG; } }        

        protected override void Startup()
        {
            try
            {
                autoLootStateMachine = new AutoLootStateMachine(this);

                initCharStats();
                initWorldFilter();
                initChatEvents();
                initEcho2Filter();
                initPath();
                //initTimer();
                loadSettings();
                // precompile rules for performance
                compileRules();
                ErrorLogging.log($"{pluginSettings.ToString()}", int.Parse(editLogLevel.Text)); // Dump settings to log

                soundPlayerCreate.Load(); // Optional: Preload the file
                soundPlayerDestroy.Load(); // Play the sound when the plugin starts

                //TODO: Code for startup events

            }
            catch (Exception ex)
            {
                ErrorLogging.LogError(errorLogFile, ex);
            }
        }

        protected override void Shutdown()
        {
            try
            {
                saveSettings();
                
                destroyChatEvents();
                destroyCharStats();
                destroyWorldFilter();
                destroyEcho2Filter();
                destroyTimer();

                soundPlayerCreate.Dispose(); // Clean up resources
                soundPlayerDestroy.Dispose(); // Clean up resources

                //TODO: Code for shutdown events
            }
            catch (Exception ex)
            {
                ErrorLogging.LogError(errorLogFile, ex);
            }
        }
    
        protected void initPath()
        {
            if (!Directory.Exists(settingsFolder))
            {
                try
                {
                    Directory.CreateDirectory(settingsFolder);
                }
                catch(Exception ex)
                {
                    ErrorLogging.LogError("c:\\WaynesWorld-errorlog.txt", ex);
                }
                
            }            
        }    
    
    }
}