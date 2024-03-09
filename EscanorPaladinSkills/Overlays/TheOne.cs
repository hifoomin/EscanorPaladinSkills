using UnityEngine.AddressableAssets;
using UnityEngine;

namespace EscanorPaladinSkills.Overlays
{
    public static class TheOne
    {
        public static Material prefab1;
        public static Material prefab2;

        public static void Init()
        {
            prefab1 = GameObject.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Huntress/matHuntressFlashBright.mat").WaitForCompletion());
            prefab1.SetColor("_TintColor", new Color32(198, 0, 0, 190));

            prefab2 = GameObject.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Huntress/matHuntressFlashExpanded.mat").WaitForCompletion());
            prefab2.SetColor("_TintColor", new Color32(43, 0, 0, 190));
        }
    }
}