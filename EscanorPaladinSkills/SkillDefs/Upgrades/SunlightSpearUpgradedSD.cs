/*
using EntityStates;
using EscanorPaladinSkills.States.Upgrades.SunlightSpear;
using RoR2;
using UnityEngine;

namespace EscanorPaladinSkills.SkillDefs.Upgrades
{
    public class SunlightSpearUpgradedSD : SkillDefBase<SunlightSpearUpgradedSD>
    {
        public override string NameToken => "SUNLIGHTSPEARUPGRADED";

        public override string NameText => "Molten Spear";

        public override string ConfigNameText => "Molten Spear";

        public override string DescriptionText => "<style=cIsUtility>Shocking</style>. <style=cIsUtility>Agile</style>. Charge up and throw a <style=cIsUtility>lightning bolt</style>, dealing up to <style=cIsDamage>800% damage</style> and firing out <style=cIsDamage>7</style> firebolts for <style=cIsDamage>200%</style> damage each. <style=cIsDamage>Throw at your feet</style> to coat your blade in <style=cIsDamage>lightning</style> for <style=cIsUtility>4 seconds</style>.";

        public override SerializableEntityStateType ActivationState => new(typeof(ChargeSunlightSpearUpgradedState));

        public override string ActivationStateMachineName => "Weapon";

        public override int BaseMaxStock => 1;

        public override float BaseRechargeInterval => 8f;

        public override bool BeginSkillCooldownOnSkillEnd => true;

        public override bool CanceledFromSprinting => false;

        public override bool CancelSprintingOnActivation => true;

        public override InterruptPriority SkillInterruptPriority => InterruptPriority.Any;

        public override bool IsCombatSkill => true;

        public override bool MustKeyPress => false;

        public override int RechargeStock => 1;

        public override Sprite Icon => Main.escanor.LoadAsset<Sprite>("texSunlightSpearUpgraded.png");

        public override int StockToConsume => 1;

        public override string[] KeywordTokens => new string[] { "KEYWORD_SHOCKING", "KEYWORD_AGILE" };

        public override bool ResetCooldownTimerOnUse => true;

        public override int RequiredStock => 1;

        public override SkillSlot SkillSlot => SkillSlot.Secondary;

        public override UnlockableDef UnlockableDef => null;

        public override bool Add => false;
    }
}
*/