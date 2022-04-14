using BepInEx;
using System.Collections.Generic;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using UnityEngine;
using System.Linq;
using RoR2;
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

        public void Start()
        {
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
            RoR2.RoR2Application.onLoad += ModifyDisplayPrefabs;
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

            var display = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/DeathProjectile/DisplayDeathProjectile.prefab").WaitForCompletion();
            var rendererInfo = display.GetComponent<ItemDisplay>().rendererInfos[0];
            rendererInfo.defaultMaterial = cirnoMaterials[0];
            rendererInfo.renderer.SetMaterialArray(cirnoMaterials);
            (rendererInfo.renderer as SkinnedMeshRenderer).sharedMesh = cirnoMesh;
            display.transform.localScale = Vector3.one * 0.02f;
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

            foreach (var language in RoR2.Language.GetAllLanguages())
            {
                language.SetStringByToken(itemDef.nameToken, "Fumo");
            }
            //itemDef.unlockableDef.achievementIcon = itemDef.pickupIconSprite;
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