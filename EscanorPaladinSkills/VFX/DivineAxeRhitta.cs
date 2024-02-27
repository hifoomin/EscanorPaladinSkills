using RoR2;
using R2API;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EscanorPaladinSkills.VFX
{
    public static class DivineAxeRhitta
    {
        public static GameObject prefab;
        public static GameObject groundPrefab;

        public static void Init()
        {
            prefab = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/MissileVoid/VoidImpactEffect.prefab").WaitForCompletion(), "Divine Axe Rhitta Impact VFX", false);

            var effectComponent = prefab.GetComponent<EffectComponent>();
            effectComponent.soundName = string.Empty;
            effectComponent.applyScale = true;

            var trans = prefab.transform;

            for (int i = 0; i < trans.childCount; i++)
            {
                var child = trans.GetChild(i);
                child.localScale *= 2.5f;
            }

            var scaledHitspark1 = trans.Find("Scaled Hitspark 1 (Random Color)").GetComponent<ParticleSystemRenderer>();
            var scaledHitspark1PS = scaledHitspark1.GetComponent<ParticleSystem>().main.startLifetime;
            scaledHitspark1PS.constantMin = 0.35f;
            scaledHitspark1PS.constantMax = 0.5f;

            var newHitspark1Mat = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Nullifier/matNullifierPortalEffectCenter.mat").WaitForCompletion());
            newHitspark1Mat.SetTexture("_RemapTex", Main.escanor.LoadAsset<Texture2D>("texRampDivineAxeRhitta1.png"));
            newHitspark1Mat.SetFloat("_InvFade", 0f);
            newHitspark1Mat.SetFloat("_Boost", 8.725851f);
            newHitspark1Mat.SetFloat("_AlphaBoost", 2.285f);
            newHitspark1Mat.SetFloat("_AlphaBias", 0.02575896f);

            scaledHitspark1.material = newHitspark1Mat;

            var scaledHitspark2 = trans.Find("Scaled Hitspark 1 (Random Color) (1)").GetComponent<ParticleSystemRenderer>();
            var scaledHitspark2PS = scaledHitspark2.GetComponent<ParticleSystem>().main;
            var scaledHitspark2PSStartColor = scaledHitspark2PS.startColor;
            var scaledHitspark2PSStartLifetime = scaledHitspark2PS.startLifetime;
            scaledHitspark2PSStartLifetime.constantMin = 0.35f;
            scaledHitspark2PSStartLifetime.constantMax = 0.55f;
            scaledHitspark2PSStartColor.mode = ParticleSystemGradientMode.Color;
            scaledHitspark2PSStartColor.color = Color.white;

            var newHitspark2Mat = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/DLC1/Common/Void/matOmniHitspark1Void.mat").WaitForCompletion());
            newHitspark2Mat.SetTexture("_RemapTex", Main.escanor.LoadAsset<Texture2D>("texRampDivineAxeRhitta1.png"));
            newHitspark2Mat.SetFloat("_Boost", 20f);
            newHitspark2Mat.SetFloat("_AlphaBoost", 1.120074f);
            newHitspark2Mat.SetFloat("_AlphaBias", 0.1589945f);

            scaledHitspark2.material = newHitspark2Mat;

            var flashHard = trans.Find("Flash, Hard").GetComponent<ParticleSystem>().main;

            var flashHardStartColor = flashHard.startColor;
            var flashHardStartLifetime = flashHard.startLifetime;
            flashHardStartLifetime.constant = 0.3f;
            flashHardStartColor.color = new Color32(255, 206, 78, 255);

            var impactShockwave = trans.Find("Impact Shockwave").GetComponent<ParticleSystemRenderer>();
            impactShockwave.alignment = ParticleSystemRenderSpace.Facing;
            impactShockwave.transform.localScale = new Vector3(8f, 0.75f, 8f);
            var impactShockwavePS = impactShockwave.GetComponent<ParticleSystem>().sizeOverLifetime;
            impactShockwavePS.size = new ParticleSystem.MinMaxCurve(1f, new AnimationCurve(new Keyframe(0f, 0.85f), new Keyframe(0.85f, 1f), new Keyframe(1f, 0f)));
            var newImpactShockwaveMat = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/DLC1/Common/Void/matOmniRing1Void.mat").WaitForCompletion());
            newImpactShockwaveMat.SetTexture("_RemapTex", Main.escanor.LoadAsset<Texture2D>("texRampDivineAxeRhitta2.png"));
            newImpactShockwaveMat.SetFloat("_InvFade", 0f);
            newImpactShockwaveMat.SetFloat("_Boost", 20f);
            newImpactShockwaveMat.SetFloat("_AlphaBoost", 1.246044f);
            newImpactShockwaveMat.SetFloat("_AlphaBias", 0.0275989f);
            newImpactShockwaveMat.SetTexture("_MainTex", Addressables.LoadAssetAsync<Texture2D>("RoR2/Base/Common/VFX/texOmniHitspark2Mask.png").WaitForCompletion());

            impactShockwave.material = newImpactShockwaveMat;

            ContentAddition.AddEffect(prefab);

            groundPrefab = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidCamp/PurchaseLockVoid.prefab").WaitForCompletion(), "Divine Axe Rhitta Ground VFX", false);

            Object.Destroy(groundPrefab.GetComponent<NetworkIdentity>());

            var sphere2 = groundPrefab.transform.GetChild(0).GetComponent<MeshRenderer>();

            var newMat = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidCamp/matVoidCampLock.mat").WaitForCompletion());
            newMat.SetColor("_TintColor", new Color32(255, 213, 0, 133));

            sphere2.material = newMat;

            var effectComponent2 = groundPrefab.AddComponent<EffectComponent>();
            effectComponent2.applyScale = true;
            effectComponent2.positionAtReferencedTransform = true;
            effectComponent2.parentToReferencedTransform = true;

            effectComponent2.gameObject.AddComponent<VFXAttributes>();

            var destroyOnTimer = groundPrefab.AddComponent<DestroyOnTimer>();
            destroyOnTimer.duration = 0.5f;

            ContentAddition.AddEffect(groundPrefab);
        }
    }
}