using BepInEx;
using BepInEx.Configuration;
using RoR2;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.AddressableAssets;

[module: UnverifiableCode]
#pragma warning disable CS0618 // Type or member is obsolete
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618 // Type or member is obsolete

[assembly: HG.Reflection.SearchableAttribute.OptIn]

namespace FMPModelSwap
{
    [BepInPlugin("com.DestroyedClone.ForgiveFumoPlease", "Forgive Fumo Please", "1.0.2")]
    public class Main : BaseUnityPlugin
    {
        public static GameObject DeathProjectile;
        public static AssetBundle MainAssets;
        public static GameObject cirnoAsset;
        public static Mesh cirnoMesh;
        public static Material[] cirnoMaterials;
        public static Sprite cirnoIcon;
        public static GameObject cirnoBodyDisplay;

        public static ConfigEntry<bool> CfgChangeName;
        public static ConfigEntry<int> CfgLore;
        public static ConfigEntry<bool> CfgChangeItemDisplay;

        public void Start()
        {
            CfgChangeName = Config.Bind("", "Fumo Name", true, "If true, then the Forgive Me Please is renamed to Fumo");
            CfgLore = Config.Bind("", "Fumo Lore", 1, "-1 - Unchanged" +
                "\n0 - Cirno's Perfect Math Class." +
                "\n1 - Chirumiru");
            CfgChangeItemDisplay = Config.Bind("", "Item Displays", true, "If true then the item display used for each character will be replaced with the fumo.");

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("FMPModelSwap.cirno_complete"))
            {
                MainAssets = AssetBundle.LoadFromStream(stream);
            }
            cirnoAsset = MainAssets.LoadAsset<GameObject>("Assets/cirno/cirno_complete.prefab");
            cirnoIcon = MainAssets.LoadAsset<Sprite>("Assets/cirno/cirnofumo_icon.png");
            cirnoMesh = cirnoAsset.GetComponentInChildren<MeshFilter>().sharedMesh;
            cirnoMaterials = cirnoAsset.GetComponentInChildren<MeshRenderer>().materials;
            DontDestroyOnLoad(cirnoAsset);

            //On.RoR2.CharacterBody.Start += CharacterBody_Start;
            ModifyProjectilePrefab();
            if (CfgChangeItemDisplay.Value)
                ModifyCharacterDisplayPrefab();
            RoR2.RoR2Application.onLoad += ModifyDisplayPrefabs;
        }

        public void ModifyCharacterDisplayPrefab()
        {
            cirnoBodyDisplay = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/DeathProjectile/DisplayDeathProjectile.prefab").WaitForCompletion();
            var rendererInfo = cirnoBodyDisplay.GetComponent<ItemDisplay>().rendererInfos[0];
            //cirnoBodyDisplay.AddComponent<FumoRendererInfo>().itemDisplay = cirnoBodyDisplay.GetComponent<ItemDisplay>();
            rendererInfo.defaultMaterial = cirnoMaterials[0];
            (rendererInfo.renderer as SkinnedMeshRenderer).SetMaterialArray(cirnoMaterials);
            (rendererInfo.renderer as SkinnedMeshRenderer).sharedMesh = cirnoMesh;
            cirnoBodyDisplay.AddComponent<FumoRendererInfo>().itemDisplay = cirnoBodyDisplay.GetComponent<ItemDisplay>();
            var mdlChild = cirnoBodyDisplay.transform.GetChild(0);
            mdlChild.localScale = Vector3.one * 0.2f;
            mdlChild.rotation *= Quaternion.Euler(0f, 180f, 0f);
        }

        public class FumoRendererInfo : MonoBehaviour
        {
            public ItemDisplay itemDisplay;
            public float age = 0;
            public float duration = 5f;

            public void Start()
            {
                var rendererInfo = itemDisplay.rendererInfos[0];
                rendererInfo.defaultMaterial = cirnoMaterials[0];
                (rendererInfo.renderer as SkinnedMeshRenderer).SetMaterialArray(cirnoMaterials);
                (rendererInfo.renderer as SkinnedMeshRenderer).sharedMesh = cirnoMesh;
            }

            public void FixedUpdate()
            {
                age += Time.fixedDeltaTime;
                if (age > duration)
                {
                    enabled = false;
                }
                itemDisplay.rendererInfos[0].defaultMaterial = cirnoMaterials[0];
            }
        }

        public void ModifyProjectilePrefab()
        {
            DeathProjectile = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/DeathProjectile/DeathProjectile.prefab").WaitForCompletion();

            var smr = DeathProjectile.GetComponentInChildren<SkinnedMeshRenderer>();
            smr.SetMaterials(cirnoMaterials.ToList());
            smr.sharedMesh = cirnoMesh;
            smr.transform.parent.localScale = Vector3.one * 0.06f;
        }

        [SystemInitializer()]
        public static void ModifyIconAndItemDisplay()
        {
            var equipmentDef = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/Base/DeathProjectile/DeathProjectile.asset").WaitForCompletion();
            equipmentDef.pickupIconSprite = cirnoIcon;
        }

        //onLoad
        public static void ModifyDisplayPrefabs()
        {
            var itemDef = RoR2.RoR2Content.Equipment.DeathProjectile;
            var mesh = itemDef.pickupModelPrefab.transform.Find("Mesh");
            mesh.GetComponent<MeshFilter>().sharedMesh = cirnoMesh;
            mesh.GetComponent<MeshRenderer>().SetMaterials(cirnoMaterials.ToList());
            mesh.localScale = Vector3.one * 0.03f;
            mesh.SetPositionAndRotation(new Vector3(0, -0.3f, 0), Quaternion.Euler(new Vector3(270, 200, 0)));

            if (CfgChangeName.Value || CfgLore.Value > 0)
            {
                foreach (var language in RoR2.Language.GetAllLanguages())
                {
                    if (CfgChangeName.Value)
                        language.SetStringByToken(itemDef.nameToken, "Fumo");
                    switch (CfgLore.Value)
                    {
                        case 0:
                            language.SetStringByToken(itemDef.loreToken, Baka.cirnoPerfectMathClassLyricsRomanji);
                            break;

                        case 1:
                            language.SetStringByToken(itemDef.loreToken, Baka.chirumiruRomanji);
                            break;
                    }
                }
            }

            //itemDef.unlockableDef.achievementIcon = itemDef.pickupIconSprite;

            if (CfgChangeItemDisplay.Value)
            {
                foreach (var bodyPrefab in BodyCatalog.allBodyPrefabs)
                {
                    var modelLocator = bodyPrefab.GetComponent<ModelLocator>();
                    if (modelLocator)
                    {
                        var modelTransform = modelLocator.modelTransform;
                        if (modelTransform)
                        {
                            var characterModel = modelTransform.GetComponent<CharacterModel>();
                            if (characterModel && characterModel.itemDisplayRuleSet && characterModel.itemDisplayRuleSet.keyAssetRuleGroups != null)
                            {
                                var keyAssetRuleGroup = characterModel.itemDisplayRuleSet
                                       .keyAssetRuleGroups
                                       .FirstOrDefault(x => x.keyAsset == RoR2Content.Equipment.DeathProjectile);
                                if (keyAssetRuleGroup.keyAsset)
                                {
                                    var displayRuleGroup = keyAssetRuleGroup.displayRuleGroup;
                                    if (!displayRuleGroup.isEmpty)
                                    {
                                        var rule = displayRuleGroup.rules[0];
                                        //Debug.Log($"{bodyPrefab.name} has it");
                                        var resultingScale = rule.localScale.x * 0.5f;
                                        if (resultingScale > 0.04f)
                                        {
                                            rule.localScale *= 0.5f;
                                        } else
                                        {
                                            //Debug.Log($"Resulting scale too small ({rule.localScale.x} -> {resultingScale})");
                                        }
                                        displayRuleGroup.rules[0] = rule;
                                        /*characterModel.itemDisplayRuleSet.SetDisplayRuleGroup(RoR2Content.Equipment.DeathProjectile,
                                            displayRuleGroup);*/
                                    }
                                }
                            }
                        }
                    }
                }

            }
        }

        private void CharacterBody_Start(On.RoR2.CharacterBody.orig_Start orig, RoR2.CharacterBody self)
        {
            orig(self);
            if (self.isPlayerControlled)
            {
                var a = Instantiate(cirnoAsset, self.transform);
                a.transform.localPosition = Vector3.zero;
                a.transform.localScale = Vector3.one * 0.2f;
            }
        }
    }
}