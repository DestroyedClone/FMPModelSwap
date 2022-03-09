using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using RoR2.ContentManagement;
using System.Collections;
using R2API;

namespace FMPModelSwap.Modules
{
    public class FumoContent : IContentPackProvider
    {
        internal static ContentPack contentPack = new ContentPack();

        public static DamageAPI.ModdedDamageType SpotterDebuffOnHit;
        public static DamageAPI.ModdedDamageType Shock5sNoDamage;

        public static AssetBundle assetBundle;

        //Decouple from Spotter?
        public static BuffDef spotterStatDebuff;

        //These show up on the enemy
        public static BuffDef spotterBuff;
        public static BuffDef spotterScepterBuff;
        public static BuffDef spotterCooldownBuff;

        //These show up on the Sniper
        public static BuffDef spotterPlayerCooldownBuff;
        public static BuffDef spotterPlayerReadyBuff;


        public static List<GameObject> bodyPrefabs = new List<GameObject>();
        public static List<BuffDef> buffDefs = new List<BuffDef>();
        public static List<EffectDef> effectDefs = new List<EffectDef>();
        public static List<Type> entityStates = new List<Type>();
        public static List<GameObject> masterPrefabs = new List<GameObject>();
        public static List<GameObject> projectilePrefabs = new List<GameObject>();
        public static List<SkillDef> skillDefs = new List<SkillDef>();
        public static List<SkillFamily> skillFamilies = new List<SkillFamily>();
        public static List<SurvivorDef> survivorDefs = new List<SurvivorDef>();
        public static List<>

        public string identifier => "SniperClassic.content";

        public IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
        {
            CreateBuffs();
            contentPack.assetCollections.
            contentPack.bodyPrefabs.Add(bodyPrefabs.ToArray());
            contentPack.buffDefs.Add(buffDefs.ToArray());
            contentPack.effectDefs.Add(effectDefs.ToArray());
            contentPack.entityStateTypes.Add(entityStates.ToArray());
            contentPack.masterPrefabs.Add(masterPrefabs.ToArray());
            contentPack.projectilePrefabs.Add(projectilePrefabs.ToArray());
            contentPack.skillDefs.Add(skillDefs.ToArray());
            contentPack.skillFamilies.Add(skillFamilies.ToArray());
            contentPack.survivorDefs.Add(survivorDefs.ToArray());
            yield break;
        }

        public IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args)
        {
            ContentPack.Copy(contentPack, args.output);
            yield break;
        }

        public IEnumerator FinalizeAsync(FinalizeAsyncArgs args)
        {
            args.ReportProgress(1f);
            yield break;
        }

        private void FixScriptableObjectName(BuffDef buff)
        {
            (buff as ScriptableObject).name = buff.name;
        }

        public void CreateBuffs()
        {
            BuffDef spotterDef = ScriptableObject.CreateInstance<BuffDef>();
            spotterDef.buffColor = new Color(1f, 1f, 1f);
            spotterDef.canStack = false;
            spotterDef.isDebuff = false;
            spotterDef.name = "SniperClassicSpotted";
            spotterDef.iconSprite = FumoContent.assetBundle.LoadAsset<Sprite>("BuffSpotterReady.png");
            FixScriptableObjectName(spotterDef);
            FumoContent.buffDefs.Add(spotterDef);
            FumoContent.spotterBuff = spotterDef;

            BuffDef spotterCooldownDef = ScriptableObject.CreateInstance<BuffDef>();
            spotterCooldownDef.buffColor = new Color(1f, 1f, 1f);
            spotterCooldownDef.canStack = true;
            spotterCooldownDef.iconSprite = FumoContent.assetBundle.LoadAsset<Sprite>("BuffSpotterCooldown.png");
            spotterCooldownDef.isDebuff = false;
            spotterCooldownDef.name = "SniperClassicSpottedCooldown";
            FixScriptableObjectName(spotterCooldownDef);
            FumoContent.buffDefs.Add(spotterCooldownDef);
            FumoContent.spotterCooldownBuff = spotterCooldownDef;

            BuffDef spotterStatDebuffDef = ScriptableObject.CreateInstance<BuffDef>();
            spotterStatDebuffDef.buffColor = new Color(0.8392157f, 0.227450982f, 0.227450982f);
            spotterStatDebuffDef.canStack = false;
            spotterStatDebuffDef.iconSprite = LegacyResourcesAPI.Load<Sprite>("textures/bufficons/texbuffweakicon");
            spotterStatDebuffDef.isDebuff = true;
            spotterStatDebuffDef.name = "SniperClassicSpottedStatDebuff";
            FixScriptableObjectName(spotterStatDebuffDef);
            FumoContent.buffDefs.Add(spotterStatDebuffDef);
            FumoContent.spotterStatDebuff = spotterStatDebuffDef;

            BuffDef spotterScepterDef = ScriptableObject.CreateInstance<BuffDef>();
            spotterScepterDef.buffColor = new Color(78f * 2f / 255f, 80f * 2f / 255f, 111f * 2f / 255f);
            spotterScepterDef.canStack = false;
            spotterScepterDef.isDebuff = false;
            spotterScepterDef.name = "SniperClassicSpottedScepter";
            spotterScepterDef.iconSprite = LegacyResourcesAPI.Load<Sprite>("textures/bufficons/texbuffcloakicon");
            FixScriptableObjectName(spotterScepterDef);
            FumoContent.buffDefs.Add(spotterScepterDef);
            FumoContent.spotterScepterBuff = spotterScepterDef;

            BuffDef spotterPlayerReadyDef = ScriptableObject.CreateInstance<BuffDef>();
            spotterPlayerReadyDef.buffColor = new Color(1f, 1f, 1f);
            spotterPlayerReadyDef.canStack = false;
            spotterPlayerReadyDef.isDebuff = false;
            spotterPlayerReadyDef.name = "SniperClassicSpotterPlayerReady";
            spotterPlayerReadyDef.iconSprite = FumoContent.assetBundle.LoadAsset<Sprite>("BuffSpotterReady.png");
            FixScriptableObjectName(spotterPlayerReadyDef);
            FumoContent.buffDefs.Add(spotterPlayerReadyDef);
            FumoContent.spotterPlayerReadyBuff = spotterPlayerReadyDef;

            BuffDef spotterPlayerCooldownDef = ScriptableObject.CreateInstance<BuffDef>();
            spotterPlayerCooldownDef.buffColor = new Color(1f, 1f, 1f);
            spotterPlayerCooldownDef.canStack = true;
            spotterPlayerCooldownDef.iconSprite = FumoContent.assetBundle.LoadAsset<Sprite>("BuffSpotterCooldown.png");
            spotterPlayerCooldownDef.isCooldown = true;
            spotterPlayerCooldownDef.isDebuff = false;
            spotterPlayerCooldownDef.name = "SniperClassicSpotterPlayerCooldown";
            FixScriptableObjectName(spotterPlayerCooldownDef);
            FumoContent.buffDefs.Add(spotterPlayerCooldownDef);
            FumoContent.spotterPlayerCooldownBuff = spotterPlayerCooldownDef;
        }
    }
}