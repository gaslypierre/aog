﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.XPath;

namespace AgOpenGPS
{
    public class CFeatureSettings
    {
        public CFeatureSettings() { }

        //public bool ;
        public bool isHeadlandOn = true;
        public bool isTramOn = true;
        public bool isBoundaryOn = true;
        public bool isBndContourOn = true;
        public bool isRecPathOn = true;
        public bool isABSmoothOn = true;
        public bool isHideContourOn = true;
        public bool isWebCamOn = true;
        public bool isOffsetFixOn = true;
        public bool isAgIOOn = true;
        public bool isContourOn = true;
        public bool isYouTurnOn = true;
        public bool isSteerModeOn = true;


        public CFeatureSettings(CFeatureSettings _feature)
        {
            isHeadlandOn = _feature.isHeadlandOn;
            isTramOn = _feature.isTramOn;
            isBoundaryOn = _feature.isBoundaryOn;
            isBndContourOn = _feature.isBndContourOn;
            isRecPathOn = _feature.isRecPathOn;
            isABSmoothOn = _feature.isABSmoothOn;
            isHideContourOn = _feature.isHideContourOn;
            isWebCamOn = _feature.isWebCamOn;
            isOffsetFixOn = _feature.isOffsetFixOn;
            isAgIOOn = _feature.isAgIOOn;
            isContourOn = _feature.isContourOn;
            isYouTurnOn = _feature.isYouTurnOn;
            isSteerModeOn = _feature.isSteerModeOn;
        }
    }

    public static class SettingsIO
    {
        /// <summary>
        /// Import an XML and save to 1 section of user.config
        /// </summary>
        /// <param name="settingFile">Either Settings or Vehicle or Tools</param>
        /// <param name="settingsFilePath">Usually Documents.Drive.Folder</param>
        internal static void ImportSingle(string settingFile, string settingsFilePath)
        {
            if (!File.Exists(settingsFilePath))
            {
                throw new FileNotFoundException();
            }

            //var appSettings = Properties.Settings.Default;
            try
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);

                string sectionName = "";

                if (settingFile == "Vehicle")
                {
                    sectionName = Properties.Vehicle.Default.Context["GroupName"].ToString();
                }
                else if (settingFile == "Settings")
                {
                    sectionName = Properties.Settings.Default.Context["GroupName"].ToString();
                }
                //else if (settingFile == "Tool")
                //{
                //    sectionName = Properties.Tool.Default.Context["GroupName"].ToString();
                //}
                //else if (settingFile == "DataSource")
                //{
                //    sectionName = Properties.Tool.Default.Context["GroupName"].ToString();
                //}

                var document = XDocument.Load(Path.Combine(settingsFilePath));
                var settingsSection = document.XPathSelectElements($"//{sectionName}").Single().ToString();
                config.GetSectionGroup("userSettings").Sections[sectionName].SectionInformation.SetRawXml(settingsSection);
                config.Save(ConfigurationSaveMode.Modified);

                if (settingFile == "Vehicle")
                {
                    Properties.Vehicle.Default.Reload();
                }
                else if (settingFile == "Settings")
                {
                    Properties.Settings.Default.Reload();
                }
            }
            catch (Exception) // Should make this more specific
            {
                // Could not import settings.
                if (settingFile == "Vehicle")
                {
                    Properties.Vehicle.Default.Reload();
                }
                else if (settingFile == "Settings")
                {
                    Properties.Settings.Default.Reload();
                }
            }
        }

        internal static void ExportSingle(string settingsFilePath)
        {
            Properties.Settings.Default.Save();
            Properties.Vehicle.Default.Save();

            //Export the entire settings as an xml
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
            config.SaveAs(settingsFilePath);
        }

        internal static void ExportAll(string settingsFilePath)
        {
            Properties.Settings.Default.Save();
            Properties.Vehicle.Default.Save();

            //Export the entire settings as an xml
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
            config.SaveAs(settingsFilePath);
        }

        internal static void ImportAll(string settingsFilePath)
        {
            if (!File.Exists(settingsFilePath))
            {
                return;
            }

            try
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
                string sectionName = Properties.Settings.Default.Context["GroupName"].ToString();

                var document = XDocument.Load(Path.Combine(settingsFilePath));
                var settingsA = document.XPathSelectElements($"//{sectionName}").Single().ToString();

                config.GetSectionGroup("userSettings").Sections[sectionName].SectionInformation.SetRawXml(settingsA);
                config.Save(ConfigurationSaveMode.Modified);

                //ConfigurationManager.RefreshSection(sectionName);
                Properties.Settings.Default.Reload();


                config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
                sectionName = Properties.Vehicle.Default.Context["GroupName"].ToString();

                document = XDocument.Load(Path.Combine(settingsFilePath));
                settingsA = document.XPathSelectElements($"//{sectionName}").Single().ToString();

                config.GetSectionGroup("userSettings").Sections[sectionName].SectionInformation.SetRawXml(settingsA);
                config.Save(ConfigurationSaveMode.Modified);

                Properties.Vehicle.Default.Reload();
            }

            catch (Exception) // Should make this more specific
            {
                // Could not import settings.
                Properties.Settings.Default.Reload();
                Properties.Vehicle.Default.Reload();
            }            
        }
    }
}
