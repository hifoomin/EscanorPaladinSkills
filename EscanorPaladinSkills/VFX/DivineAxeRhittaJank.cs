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
            swingPrefab = PrefabAPI.InstantiateClone(PaladinMod.Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("PaladinSwing"), "Divine Axe Rhitta Swing VFX", false);
            Object.Destroy(swingPrefab.GetComponent<NetworkIdentity>());
            swingPrefab.GetComponent<DestroyOnTimer>().duration = 5;

            var effect = swingPrefab.GetComponent<EffectComponent>();
            effect.applyScale = false;
            effect.parentToReferencedTransform = true;
            effect.positionAtReferencedTransform = true;

            var trans = swingPrefab.transform;
            var swingTrail = trans.Find("SwingTrail");
            swingTrail.localScale = Vector3.one * 4f;

            var newMat = Object.Instantiate(PaladinMod.Modules.Assets.mainAssetBundle.LoadAsset<Material>("matPaladinSwing3"));
            newMat.SetColor("_Color", new Color32(255, 0, 74, 255));
            newMat.SetColor("_EmissionColor", new Color32(255, 144, 0, 255));
            newMat.SetFloat("_Cutoff", 0.12f);
            newMat.SetFloat("_BlendOp", 4);
            newMat.SetTexture("_MainTex", Main.escanor.LoadAsset<Texture2D>("texMaskSwing.png"));
            newMat.SetTexture("_EmissionMap", Main.escanor.LoadAsset<Texture2D>("texMaskSwing.png"));

            swingTrail.GetComponent<ParticleSystemRenderer>().material = newMat;

            var swingTrailDistortion = swingTrail.Find("SwingTrail2");
            swingTrailDistortion.localScale = Vector3.one * 2f;

            ContentAddition.AddEffect(swingPrefab);
        }
    }
}