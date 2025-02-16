using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EscanorPaladinSkills.VFX
{
    public static class DivineAxeRhittaJank
    {
        public static GameObject swingPrefab;

        public static void Init()
        {
            var paladinSwing = PaladinMod.Modules.Asset.mainAssetBundle.LoadAsset<GameObject>("PaladinSwing");
            // Main.logger.LogError("paladin swing name is " + paladinSwing.name);

            swingPrefab = PrefabAPI.InstantiateClone(paladinSwing, "Divine Axe Rhitta Swing VFX", false);

            var vfxAttributes = swingPrefab.AddComponent<VFXAttributes>();
            vfxAttributes.DoNotPool = true;

            Object.Destroy(swingPrefab.GetComponent<NetworkIdentity>());
            swingPrefab.GetComponent<DestroyOnTimer>().duration = 5;

            var effect = swingPrefab.AddComponent<EffectComponent>();
            effect.applyScale = false;
            effect.parentToReferencedTransform = true;
            effect.positionAtReferencedTransform = true;

            var trans = swingPrefab.transform;
            var swingTrail = trans.Find("SwingTrail");
            swingTrail.localScale = Vector3.one * 4f;

            var newMat = Object.Instantiate(PaladinMod.Modules.Asset.mainAssetBundle.LoadAsset<Material>("matPaladinSwing3"));
            newMat.SetColor("_Color", new Color32(255, 0, 74, 255));
            newMat.SetColor("_EmissionColor", new Color32(255, 144, 0, 255));
            newMat.SetFloat("_Cutoff", 0.12f);
            newMat.SetFloat("_BlendOp", 4);
            newMat.SetTexture("_MainTex", Main.escanor.LoadAsset<Texture2D>("texMaskSwing.png"));
            newMat.SetTexture("_EmissionMap", Main.escanor.LoadAsset<Texture2D>("texMaskSwing.png"));

            swingTrail.GetComponent<ParticleSystemRenderer>().material = newMat;

            var swingTrailDistortion = swingTrail.Find("SwingTrail2");
            swingTrailDistortion.localScale = Vector3.one * 2f;

            var light = swingTrail.gameObject.AddComponent<Light>();
            light.color = new Color32(255, 144, 0, 255);
            light.range = 13f;
            light.intensity = 14f;

            var lightIntensityCurve = swingTrail.gameObject.AddComponent<LightIntensityCurve>();
            lightIntensityCurve.light = light;
            lightIntensityCurve.maxIntensity = 14f;
            lightIntensityCurve.curve = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 0f));
            lightIntensityCurve.timeMax = 0.5f;

            ContentAddition.AddEffect(swingPrefab);
        }
    }
}