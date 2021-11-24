using BepInEx;
using R2API.Utils;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using UnityEngine;
using System.Collections.Generic;

[module: UnverifiableCode]
#pragma warning disable CS0618 // Type or member is obsolete
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618 // Type or member is obsolete

namespace FMPModelSwap
{
    [BepInPlugin("com.DestroyedClone.ForgiveFumoPlease", "Forgive Fumo Please", "1.0.0")]
    [BepInDependency(R2API.R2API.PluginGUID, R2API.R2API.PluginVersion)]
    [NetworkCompatibility(CompatibilityLevel.NoNeedForSync, VersionStrictness.DifferentModVersionsAreOk)]
    public class Main : BaseUnityPlugin
    {
        public static GameObject DeathProjectile = Resources.Load<GameObject>("Prefabs/Projectiles/DeathProjectile");
        public static AssetBundle MainAssets;
        public static GameObject cirnoAsset;

        public void Awake()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("FMPModelSwap.cirno_complete"))
            {
                MainAssets = AssetBundle.LoadFromStream(stream);
            }
            cirnoAsset = MainAssets.LoadAsset<GameObject>("Assets/cirno/cirno_complete.prefab");
            DontDestroyOnLoad(cirnoAsset);

            //On.RoR2.CharacterBody.Start += CharacterBody_Start;

            ModifyPrefab();
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

        public static void ModifyPrefab()
        {
            //var assetCopy = Instantiate(cirnoAsset);
            //var smr = DeathProjectile.GetComponentInChildren<SkinnedMeshRenderer>();
            //assetCopy.transform.SetParent(DeathProjectile.transform);

            var smr = DeathProjectile.GetComponentInChildren<SkinnedMeshRenderer>();
            smr.SetMaterials(new List<Material>(cirnoAsset.GetComponentInChildren<MeshRenderer>().materials));
            smr.sharedMesh = cirnoAsset.GetComponentInChildren<MeshFilter>().sharedMesh;
            smr.transform.parent.localScale = Vector3.one * 0.03f;

            //DeathProjectile.GetComponentInChildren<MeshFilter>().sharedMesh = cirnoAsset.GetComponentInChildren<MeshFilter>().sharedMesh;
            //var meshRenderer = DeathProjectile.GetComponentInChildren<MeshRenderer>();
            //meshRenderer.material = moneyAsset.GetComponentInChildren<MeshRenderer>().material;
            //meshRenderer.SetMaterials(new List<Material>(cirnoAsset.GetComponentInChildren<MeshRenderer>().materials));
            //meshRenderer.transform.localScale = Vector3.one * 9;
            //Destroy(ShareMoneyPack.transform.Find("Display/Mesh/Particle System").gameObject);

            //DeathProjectile.name = "gay";
            //assetCopy.transform.localPosition = Vector3.zero;
        }
    }
}