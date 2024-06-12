using PaladinMod.States;

namespace EscanorPaladinSkills.States.Upgrades.SunlightSpear
{
    public class ThrowSunlightSpearUpgradedState : BaseThrowSpellState
    {
        public override void OnEnter()
        {
            baseDuration = 0.8f;
            force = 4000f;
            minDamageCoefficient = PaladinMod.StaticValues.lightningSpearMinDamageCoefficient;
            maxDamageCoefficient = PaladinMod.StaticValues.lightningSpearMaxDamageCoefficient;
            muzzleflashEffectPrefab = EntityStates.Commando.CommandoWeapon.FirePistol2.muzzleEffectPrefab;
            projectilePrefab = Projectiles.SunlightSpearUpgraded.prefab;
            selfForce = 2500f;

            base.OnEnter();

            /*ChildLocator childLocator = base.GetModelChildLocator();
            if (childLocator)
            {
                childLocator.FindChild("SpearThrowEffect").gameObject.SetActive(true);
            }*/
        }
    }
}