// ConfigManager.cs
using UnityEngine;
using System.IO;

namespace SimpleAnimatedUI
{
    public class ConfigManager : MonoBehaviour
    {
        private static ConfigManager _instance;
        public static ConfigManager Instance => _instance;

        private PageConfigData _config;
        public PageConfigData Config => _config;

        public System.Action OnConfigLoaded;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            LoadConfig();
        }

        private void LoadConfig()
        {
            string configPath = GetConfigPath();

            if (File.Exists(configPath))
            {
                try
                {
                    string json = File.ReadAllText(configPath);
                    _config = JsonUtility.FromJson<PageConfigData>(json);
                    Debug.Log($"[ConfigManager] Config loaded from: {configPath}");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"[ConfigManager] Error loading config: {e.Message}");
                    CreateDefaultConfig();
                }
            }
            else
            {
                Debug.LogWarning($"[ConfigManager] Config file not found. Creating a default one: {configPath}");
                CreateDefaultConfig();
            }

            OnConfigLoaded?.Invoke();
        }

        private void CreateDefaultConfig()
        {
            _config = new PageConfigData();
            SaveConfig();
        }

        public void SaveConfig()
        {
            try
            {
                string configPath = GetConfigPath();
                string directory = Path.GetDirectoryName(configPath);

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                string json = JsonUtility.ToJson(_config, true);
                File.WriteAllText(configPath, json);
                Debug.Log($"[ConfigManager] The config is saved in: {configPath}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[ConfigManager] Error saving config: {e.Message}");
            }
        }

        public void ReloadConfig()
        {
            LoadConfig();
        }

        private string GetConfigPath()
        {
#if UNITY_EDITOR
            return Path.Combine(Application.dataPath, "Configs", "page_config.json");
#else
            // В билде используем папку с данными
            return Path.Combine(Application.persistentDataPath, "Configs", "page_config.json");
#endif
        }

        public float GetFadeInDuration() => _config != null ? _config.pageFadeInDuration : 0.4f;
        public float GetFadeOutDuration() => _config != null ? _config.pageFadeOutDuration : 0.4f;
        public DG.Tweening.Ease GetFadeInEase() => _config != null ? _config.GetFadeInEase() : DG.Tweening.Ease.InSine;
        public DG.Tweening.Ease GetFadeOutEase() => _config != null ? _config.GetFadeOutEase() : DG.Tweening.Ease.InSine;
    }
}