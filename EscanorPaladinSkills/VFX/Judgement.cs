using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EscanorPaladinSkills.VFX
{
    public static class Judgement
    {
        public static GameObject prefab;

        public static void Init()
        {
            prefab = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/OmniImpactVFXSlashMerc.prefab").WaitForCompletion(), "Judgement VFX", false);

            var effectComponent = prefab.GetComponent<EffectComponent>();
            effectComponent.soundName = string.Empty;
            effectComponent.applyScale = true;

            ContentAddition.AddEffect(prefab);
        }
    }
}