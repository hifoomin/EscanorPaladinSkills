using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EscanorPaladinSkills.Overlays
{
    public static class FlameOfLife
    {
        public static Material prefab1;
        public static Material prefab2;

        public static void Init()
        {
            prefab1 = GameObject.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Huntress/matHuntressFlashBright.mat").WaitForCompletion());
            prefab1.SetColor("_TintColor", new Color32(191, 11, 3, 200));

            prefab2 = GameObject.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Huntress/matHuntressFlashExpanded.mat").WaitForCompletion());
            prefab2.SetColor("_TintColor", new Color32(148, 17, 0, 200));
        }
    }
}