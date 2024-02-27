using EntityStates;
using EscanorPaladinSkills.States;
using RoR2;
using UnityEngine;

namespace EscanorPaladinSkills.SkillDefs
{
    public class SunshineCruelSunSD : SkillDefBase<SunshineCruelSunSD>
    {
        public override string NameToken => "SUNSHINECRUELSUN";

        public override string NameText => "Sunshine's Cruel Sun";

        public override string ConfigNameText => "Sunshines Cruel Sun";

        public override string DescriptionText => "<style=cIsDamage>Ignite</style>. Hurl a slow, homing <style=cIsDamage>miniature sun</style> that explodes in a large radius for <style=cIsDamage>800%</style> damage. Gain <style=cIsHealing>7.5% barrier</style> for each enemy hit.";

        public override SerializableEntityStateType ActivationState => new(typeof(CruelSunState));

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

        public override Sprite Icon => null;

        public override int StockToConsume => 1;

        public override string[] KeywordTokens => new string[] { "KEYWORD_IGNITE" };

        public override bool ResetCooldownTimerOnUse => true;

        public override int RequiredStock => 1;

        public override SkillSlot SkillSlot => SkillSlot.Secondary;

        public override UnlockableDef UnlockableDef => null;
    }
}