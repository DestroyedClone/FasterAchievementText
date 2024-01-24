using BepInEx;
using BepInEx.Configuration;
using RoR2;
using System;
using System.Security;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.AddressableAssets;

[module: UnverifiableCode]
#pragma warning disable CS0618 // Type or member is obsolete
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618 // Type or member is obsolete

namespace FasterAchievementText
{
    [BepInPlugin("com.DestroyedClone.FasterAchievementText", "Faster Achievement Text", "0.0.1")]
    public class Plugin : BaseUnityPlugin
    {
        public static ConfigEntry<bool> cfgEnableBlur;
        public static ConfigEntry<float> cfgDurationOverride;
        public static ConfigEntry<bool> cfgEnableSound;
        public static bool subscribedToDisablingSound = false;
        public static GameObject Prefab;

        public void Awake()
        {
            Prefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/AchievementNotificationPanel.prefab").WaitForCompletion();

            Prefab.AddComponent<DestroyOnTimer>();
            ConfigSetup();
            CfgEnableBlur_SettingChanged(null, null);
            CfgDurationOverride_SettingChanged(null, null);
            CfgEnableSound_SettingChanged(null, null);
        }

        public void ConfigSetup()
        {
            cfgEnableBlur = Config.Bind("", "Enable Blur", false, "");
            cfgEnableBlur.SettingChanged += CfgEnableBlur_SettingChanged;
            cfgDurationOverride = Config.Bind("", "Duration Override", 1f, "Normal duration is like 5s or something, anything beyond that will be cut off by the normal duration.");
            cfgDurationOverride.SettingChanged += CfgDurationOverride_SettingChanged;
            cfgEnableSound = Config.Bind("", "Enable Notification Sound", true, "");
            cfgEnableSound.SettingChanged += CfgEnableSound_SettingChanged;
        }

        private void CfgEnableSound_SettingChanged(object _, EventArgs e)
        {
            if (!cfgEnableSound.Value)
            {
                if (!subscribedToDisablingSound)
                {
                    On.RoR2.Util.PlaySound_string_GameObject += Util_PlaySound_string_GameObject;
                }
            }
            else
            {
                if (subscribedToDisablingSound)
                {
                    On.RoR2.Util.PlaySound_string_GameObject += Util_PlaySound_string_GameObject;
                }
            }
        }

        private uint Util_PlaySound_string_GameObject(On.RoR2.Util.orig_PlaySound_string_GameObject orig, string soundString, GameObject gameObject)
        {
            switch (soundString)
            {
                case "Play_UI_achievementUnlock_enhanced":
                case "Play_UI_skill_unlock":
                case "Play_UI_achievementUnlock":
                    soundString = "";
                    break;
            }
            return orig(soundString, gameObject);
        }

        private void CfgEnableBlur_SettingChanged(object _, EventArgs e)
        {
            Prefab.transform.Find("Blur").gameObject.SetActive(cfgEnableBlur.Value);
        }

        private void CfgDurationOverride_SettingChanged(object _, EventArgs e)
        {
            Prefab.GetComponent<DestroyOnTimer>().duration = cfgDurationOverride.Value;
        }
    }
}