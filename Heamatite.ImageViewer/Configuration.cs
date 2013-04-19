using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Heamatite
{

	public interface IConfiguration
	{
		string StartupDirectory { get; set; }

		bool ImageViewFullScreen { get; set; }

		int ImageViewWidth { get; set; }

		int ImageViewHeight { get; set; }

		bool MainViewFullScreen { get; set; }

		int MainViewHeight { get; set; }

		int MainViewWidth { get; set; }
	}

	class Configuration1 : IConfiguration
	{
		WindowSettings ConfigSettings;
		public Configuration1()
		{
			ConfigSettings = GetRoamingConfiguration(out  Config);
			if (string.IsNullOrEmpty(StartupDirectory)) { StartupDirectory = @"c:\"; }
		}

		public string StartupDirectory
		{
			get { return ConfigSettings.StartupDirectory.Value; }
			set { ConfigSettings.StartupDirectory.Value = value; Save(); }
		}

		public bool ImageViewFullScreen
		{
			get { return ConfigSettings.ImageViewFullScreen.Value; }
			set { ConfigSettings.ImageViewFullScreen.Value = value; Save(); }
		}

		public int ImageViewWidth
		{
			get { return ConfigSettings.ImageViewWidth.Value; }
			set { ConfigSettings.ImageViewWidth.Value = value; Save(); }
		}

		public int ImageViewHeight
		{
			get { return ConfigSettings.ImageViewHeight.Value; }
			set { ConfigSettings.ImageViewHeight.Value = value; Save(); }
		}
		public bool MainViewFullScreen
		{
			get { return ConfigSettings.MainViewFullScreen.Value; }
			set { ConfigSettings.MainViewFullScreen.Value = value; Save(); }
		}

		public int MainViewWidth
		{
			get { return ConfigSettings.MainViewWidth.Value; }
			set { ConfigSettings.MainViewWidth.Value = value; Save(); }
		}

		public int MainViewHeight
		{
			get { return ConfigSettings.MainViewHeight.Value; }
			set { ConfigSettings.MainViewHeight.Value = value; Save(); }
		}

		private void Save()
		{
			Config.Save(ConfigurationSaveMode.Modified);
		}

		Configuration Config;

		public static WindowSettings GetRoamingConfiguration(out Configuration config)
		{
			// Define the custom section to add to the
			// configuration file.
			string sectionName = "windowSettings";
			WindowSettings currentSection = null;

			// Get the roaming configuration 
			// that applies to the current user.
			Configuration roamingConfig =
				ConfigurationManager.OpenExeConfiguration(
				 ConfigurationUserLevel.PerUserRoamingAndLocal);

			// Map the roaming configuration file. This
			// enables the application to access 
			// the configuration file using the
			// System.Configuration.Configuration class
			ExeConfigurationFileMap configFileMap =
				new ExeConfigurationFileMap();
			configFileMap.ExeConfigFilename =
				roamingConfig.FilePath;

			// Get the mapped configuration file.
			config =
				ConfigurationManager.OpenMappedExeConfiguration(
					configFileMap, ConfigurationUserLevel.None);

			try
			{
				currentSection =
						 (WindowSettings)config.GetSection(
							 sectionName);

				// Synchronize the application configuration
				// if needed. The following two steps seem
				// to solve some out of synch issues 
				// between roaming and default
				// configuration.
				//config.Save(ConfigurationSaveMode.Full);

				// Force a reload of the changed section, 
				// if needed. This makes the new values available 
				// for reading.
				//ConfigurationManager.RefreshSection(sectionName);

				if (currentSection == null)
				{
					// Create a custom configuration section.
					currentSection = new WindowSettings();

					// Define where in the configuration file 
					// hierarchy the associated 
					// configuration section can be declared.
					// The following assignment assures that 
					// the configuration information can be 
					// defined in the user.config file in the 
					// roaming user directory. 
					currentSection.SectionInformation.AllowExeDefinition =
						ConfigurationAllowExeDefinition.MachineToLocalUser;

					// Allow the configuration section to be 
					// overridden by lower-level configuration files.
					// This means that lower-level files can contain
					// the section (use the same name) and assign 
					// different values to it as done by the
					// function GetApplicationConfiguration() in this
					// example.
					currentSection.SectionInformation.AllowOverride =
						true;
					// Add configuration information to 
					// the configuration file.
					config.Sections.Add(sectionName, currentSection);

					currentSection.StartupDirectory = new StringConfigElement();
					currentSection.ImageViewFullScreen = new BoolConfigElement();
					currentSection.MainViewFullScreen = new BoolConfigElement();

					currentSection.ImageViewHeight = new IntConfigElement();
					currentSection.ImageViewWidth = new IntConfigElement();
					currentSection.MainViewHeight = new IntConfigElement();
					currentSection.MainViewWidth = new IntConfigElement();

				}
				config.Save(ConfigurationSaveMode.Modified);
				// Force a reload of the changed section. This 
				// makes the new values available for reading.
				ConfigurationManager.RefreshSection(
					sectionName);

				return currentSection;
			}
			catch (ConfigurationErrorsException e)
			{
				System.Diagnostics.Debug.WriteLine("[Exception error: {0}]",
						e.ToString());
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine("[Exception error: {0}]",
						e.ToString());
			}
			return null;
		}


	}

	public class WindowSettings : ConfigurationSection
	{
		[ConfigurationProperty("StartupDirectory")]
		public StringConfigElement StartupDirectory
		{
			get { return (StringConfigElement)base["StartupDirectory"]; }
			set { base["StartupDirectory"] = value; }
		}
		[ConfigurationProperty("ImageViewHeight")]
		public IntConfigElement ImageViewHeight
		{
			get { return (IntConfigElement)base["ImageViewHeight"]; }
			set { base["ImageViewHeight"] = value; }
		}
		[ConfigurationProperty("ImageViewFullScreen")]
		public BoolConfigElement ImageViewFullScreen
		{
			get { return (BoolConfigElement)base["ImageViewFullScreen"]; }
			set { base["ImageViewFullScreen"] = value; }
		}
		[ConfigurationProperty("ImageViewWidth")]
		public IntConfigElement ImageViewWidth
		{
			get { return (IntConfigElement)base["ImageViewWidth"]; }
			set { base["ImageViewWidth"] = value; }
		}

		[ConfigurationProperty("MainViewHeight")]
		public IntConfigElement MainViewHeight
		{
			get { return (IntConfigElement)base["MainViewHeight"]; }
			set { base["MainViewHeight"] = value; }
		}
		[ConfigurationProperty("MainViewWidth")]
		public IntConfigElement MainViewWidth
		{
			get { return (IntConfigElement)base["MainViewWidth"]; }
			set { base["MainViewWidth"] = value; }
		}
		[ConfigurationProperty("MainViewFullScreen")]
		public BoolConfigElement MainViewFullScreen
		{
			get { return (BoolConfigElement)base["MainViewFullScreen"]; }
			set { base["MainViewFullScreen"] = value; }
		}
	}

	public class IntConfigElement : ConfigurationElement
	{
		[ConfigurationProperty("value", DefaultValue = 0, IsRequired = true, IsKey = true)]
		public int Value
		{
			get { return int.Parse(this["value"].ToString()); }
			set { this["value"] = value; }
		}

		public IntConfigElement() { }

		public IntConfigElement(int value)
		{
			Value = value;
		}
	}
	public class BoolConfigElement : ConfigurationElement
	{
		[ConfigurationProperty("value", DefaultValue = false, IsRequired = true, IsKey = true)]
		public bool Value
		{
			get { return bool.Parse(this["value"].ToString()); }
			set { this["value"] = value; }
		}

		public BoolConfigElement() { }

		public BoolConfigElement(bool value)
		{
			Value = value;
		}
	}

	public class StringConfigElement : ConfigurationElement
	{
		[ConfigurationProperty("value", DefaultValue = "", IsRequired = true, IsKey = true)]
		public string Value
		{
			get { return this["value"].ToString(); }
			set { this["value"] = value; }
		}

		public StringConfigElement() { }

		public StringConfigElement(string value)
		{
			Value = value;
		}
	}
}
