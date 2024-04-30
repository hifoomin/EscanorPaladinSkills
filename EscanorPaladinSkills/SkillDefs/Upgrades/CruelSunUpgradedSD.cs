using EntityStates;
using EscanorPaladinSkills.States;
using EscanorPaladinSkills.States.Upgrades;
using RoR2;
using UnityEngine;

namespace EscanorPaladinSkills.SkillDefs.Upgrades
{
    public class CruelSunUpgradedSD : SkillDefBase<CruelSunUpgradedSD>
    {
        public override string NameToken => "SUNSHINECRUELSUNUPGRADED";

        public override string NameText => "Sunshine's Cruel Quasar";

        public override string ConfigNameText => "Sunshines Cruel Quasar";

        public override string DescriptionText => "<style=cIsDamage>Ignite</style>. Hurl a slow, homing <style=cIsDamage>miniature quasar</style> that explodes in an enormous radius for <style=cIsDamage>1000%</style> damage. Gain <style=cIsHealing>5% barrier</style> for each enemy hit.";

        public override SerializableEntityStateType ActivationState => new(typeof(CruelSunUpgradedState));

        public override string ActivationStateMachineName => "Weapon";

        public override int BaseMaxStock => 1;

        public override float BaseRechargeInterval => 8f;

        public override bool BeginSkillCooldownOnSkillEnd => true;

        public override bool CanceledFromSprinting => false;

        public override bool CancelSprintingOnActivation => true;

        public override InterruptPriority SkillInterruptPriority => InterruptPriority.Any;

        public override bool IsCombatSkill => true;

        public override bool MustKeyPress => true;

        public override int RechargeStock => 1;

        public override Sprite Icon => Main.escanor.LoadAsset<Sprite>("texSunshinesCruelSun3Upgraded.png");

        public override int StockToConsume => 1;

        public override string[] KeywordTokens => new string[] { "KEYWORD_IGNITE" };

        public override bool ResetCooldownTimerOnUse => true;

        public override int RequiredStock => 1;

        public override SkillSlot SkillSlot => SkillSlot.Secondary;

        public override UnlockableDef UnlockableDef => null;

        public override bool Add => false;
    }
}