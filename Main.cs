using BepInEx;
using System.Collections.Generic;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using UnityEngine;

[module: UnverifiableCode]
#pragma warning disable CS0618 // Type or member is obsolete
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618 // Type or member is obsolete

namespace FMPModelSwap
{
    [BepInPlugin("com.DestroyedClone.ForgiveFumoPlease", "Forgive Fumo Please", "1.0.1")]
    public class Main : BaseUnityPlugin
    {
        public static GameObject DeathProjectile;
        public static AssetBundle MainAssets;
        public static GameObject cirnoAsset;

        public void Start()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("FMPModelSwap.cirno_complete"))
            {
                MainAssets = AssetBundle.LoadFromStream(stream);
            }
            cirnoAsset = MainAssets.LoadAsset<GameObject>("Assets/cirno/cirno_complete.prefab");
            DontDestroyOnLoad(cirnoAsset);

            //On.RoR2.CharacterBody.Start += CharacterBody_Start;

            DeathProjectile = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/DeathProjectile");
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
            var smr = DeathProjectile.GetComponentInChildren<SkinnedMeshRenderer>();
            smr.SetMaterials(new List<Material>(cirnoAsset.GetComponentInChildren<MeshRenderer>().materials));
            smr.sharedMesh = cirnoAsset.GetComponentInChildren<MeshFilter>().sharedMesh;
            smr.transform.parent.localScale = Vector3.one * 0.03f;
        }
    }
}