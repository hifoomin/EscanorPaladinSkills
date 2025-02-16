/*
using PaladinMod.States;
using UnityEngine;

namespace EscanorPaladinSkills.States.Upgrades.SunlightSpear
{
    public class ChargeSunlightSpearUpgradedState : BaseChargeSpellState
    {
        private GameObject chargeEffect;
        private Vector3 originalScale;

        public override void OnEnter()
        {
            baseDuration = PaladinMod.StaticValues.lightningSpearChargeTime;
            chargeEffectPrefab = null;
            chargeSoundString = "Play_mage_m2_charge";
            crosshairOverridePrefab = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Crosshair/ToolbotGrenadeLauncherCrosshair");
            maxBloomRadius = 0.2f;
            minBloomRadius = 2;

            base.OnEnter();

            ChildLocator childLocator = GetModelChildLocator();
            if (childLocator)
            {
                chargeEffect = childLocator.FindChild("SpearChargeEffect").gameObject;
                chargeEffect.SetActive(true);
                originalScale = chargeEffect.transform.localScale;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            chargeEffect.transform.localScale = originalScale * 6 * CalcCharge();
        }

        public override void OnExit()
        {
            base.OnExit();

            if (chargeEffect)
            {
                chargeEffect.transform.localScale = originalScale;
                chargeEffect.SetActive(false);
            }
        }

        public override BaseThrowSpellState GetNextState()
        {
            return new ThrowSunlightSpearUpgradedState();
        }
    }
}
*/