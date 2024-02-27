using EntityStates;
using EscanorPaladinSkills.States;
using EscanorPaladinSkills.States.Upgrades.SpinningSlash;
using RoR2;
using UnityEngine;

namespace EscanorPaladinSkills.SkillDefs.Upgrades
{
    public class SpinningSlashUpgradedSD : SkillDefBase<SpinningSlashUpgradedSD>
    {
        public override string NameToken => "SPINNINGSLASHUPGRADEDSD";

        public override string NameText => "Whirlwind";

        public override string ConfigNameText => "Whirlwind";

        public override string DescriptionText => "Perform a <style=cIsDamage>wide stunning whirlwind</style> for <style=cIsDamage>2x1000% damage</style>, gaining range if <style=cIsHealing>blessed</style>. If airborne, perform a vault strike, firing a <style=cIsDamage>shockwave</style> if <style=cIsHealing>blessed</style>.";

        public override SerializableEntityStateType ActivationState => new(typeof(SpinningSlashEntryUpgradedState));

        public override string ActivationStateMachineName => "Weapon";

        public override int BaseMaxStock => 1;

        public override float BaseRechargeInterval => 6f;

        public override bool BeginSkillCooldownOnSkillEnd => true;

        public override bool CanceledFromSprinting => false;

        public override bool CancelSprintingOnActivation => true;

        public override InterruptPriority SkillInterruptPriority => InterruptPriority.Any;

        public override bool IsCombatSkill => true;

        public override bool MustKeyPress => false;

        public override int RechargeStock => 1;

        public override Sprite Icon => Main.escanor.LoadAsset<Sprite>("texSpinningSlashUpgraded.png");

        public override int StockToConsume => 1;

        public override string[] KeywordTokens => new string[] { "KEYWORD_STUNNING" };

        public override bool ResetCooldownTimerOnUse => false;

        public override int RequiredStock => 1;

        public override SkillSlot SkillSlot => SkillSlot.Secondary;

        public override UnlockableDef UnlockableDef => null;

        public override bool Add => false;
    }
}