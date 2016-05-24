using System;
using System.Net;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Threading;

namespace iostamagotchi
{
    public class Settings
    {
        // Dictionary with key names of our settings and their default values
        private Dictionary<String, Object> m_keyNames;
        private IsolatedStorageSettings m_settings;

        /// <summary>
        /// Constructor
        /// </summary>
        public Settings()
        {
            this.initializeValues();
        }

        /// <summary>
        /// Initialize dictionary with key names of the application settings and their default values
        /// </summary>
        private void initializeValues()
        {
            this.m_settings = IsolatedStorageSettings.ApplicationSettings;
            this.m_keyNames = new Dictionary<String, Object>();
            this.m_keyNames.Add("plan.id", 1);
            this.m_keyNames.Add("plan.idx", 0);
            this.m_keyNames.Add("plan.name", "Plan1");
            this.m_keyNames.Add("plan.notifications", true);

            this.m_keyNames.Add("quizlet.clientId", "M9uwK6wgas");
            this.m_keyNames.Add("quizlet.secret", "JA8CHNaESc2FhCXuyOeR3Q");
            this.m_keyNames.Add("quizlet.token", "");
            this.m_keyNames.Add("quizlet.perPage", "50");
            this.m_keyNames.Add("quizlet.imagesOnly", "false");
            this.m_keyNames.Add("quizlet.word", "verbs");

            this.m_keyNames.Add("study.setId", 0);
            this.m_keyNames.Add("study.known", 0);
            this.m_keyNames.Add("study.total", 0);
            this.m_keyNames.Add("study.rank", 0);
            this.m_keyNames.Add("study.on", true);
            this.m_keyNames.Add("study.improved", 0);
            this.m_keyNames.Add("study.algorithm", true);

            this.m_keyNames.Add("testing.score", 0);
            this.m_keyNames.Add("testing.known", 0);
            this.m_keyNames.Add("testing.total", 0);
            this.m_keyNames.Add("testing.setId", 0);
            this.m_keyNames.Add("testing.inRow", 0);
            this.m_keyNames.Add("testing.lastCheer", 0);
            this.m_keyNames.Add("testing.on", true);

            this.m_keyNames.Add("player.score", 0);
        }

        /// <summary>
        /// Saves data to Isolated Storaga using mutex
        /// </summary>
        public void Save()
        {
            Mutex mutex = new Mutex(true, "SpacechiAgentData");
            mutex.WaitOne();
            this.m_settings.Save();
            mutex.ReleaseMutex();
        }

        /// <summary>
        /// Update a setting value for our application. If the setting does not
        /// exist, then add the setting.
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool AddOrUpdateValue(string Key, Object value)
        {
            bool valueChanged = false;

            // If the key exists
            if (this.m_settings.Contains(Key))
            {
                // If the value has changed
                if (this.m_settings[Key] != value)
                {
                    // Store the new value
                    this.m_settings[Key] = value;
                    valueChanged = true;
                }
            }
            // Otherwise create the key.
            else
            {
                this.m_settings.Add(Key, value);
                valueChanged = true;
            }
            return valueChanged;
        }

        /// <summary>
        /// Get the current value of the setting, or if it is not found, set the 
        /// setting to the default setting.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T GetValueOrDefault<T>(string Key)
        {
            T value;

            // If the key exists, retrieve the value.
            if (this.m_settings.Contains(Key))
            {
                value = (T)this.m_settings[Key];
            }
            // Otherwise, use the default value.
            else
            {
                // check, if we know this property
                if (this.m_keyNames.ContainsKey(Key))
                {
                    value = (T)this.m_keyNames[Key];
                }
                else // nope, we dont know you so we throw an exception here
                {
                    throw new ArgumentNullException();
                }
            }
            return value;
        }
    }
}
