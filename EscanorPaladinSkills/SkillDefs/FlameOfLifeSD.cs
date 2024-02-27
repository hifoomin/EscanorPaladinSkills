using EntityStates;
using EscanorPaladinSkills.States;
using RoR2;
using UnityEngine;

namespace EscanorPaladinSkills.SkillDefs
{
    public class FlameOfLifeSD : SkillDefBase<FlameOfLifeSD>
    {
        public override string NameToken => "FLAMEOFLIFE";

        public override string NameText => "Flame of Life";

        public override string ConfigNameText => "Flame of Life";

        public override string DescriptionText => "<style=cIsUtility>Agile</style>. For the next <style=cIsUtility>7 seconds</style>, become <style=cIsHealing>blessed</style>. All attacks <style=cIsDamage>ignite</style> and deal <style=cIsDamage>75%</style> more damage. Increase <style=cIsUtility>movement speed</style> by <style=cIsUtility>20%</style>. <style=cDeath>Disable all healing and incinerate yourself</style>.";

        public override SerializableEntityStateType ActivationState => new(typeof(FlameOfLifeState));

        public override string ActivationStateMachineName => "Flame";

        public override int BaseMaxStock => 1;

        public override float BaseRechargeInterval => 10f;

        public override bool BeginSkillCooldownOnSkillEnd => true;

        public override bool CanceledFromSprinting => false;

        public override bool CancelSprintingOnActivation => false;

        public override InterruptPriority SkillInterruptPriority => InterruptPriority.Any;

        public override bool IsCombatSkill => false;

        public override bool MustKeyPress => true;

        public override int RechargeStock => 1;

        public override Sprite Icon => null;

        public override int StockToConsume => 1;

        public override string[] KeywordTokens => new string[] { "KEYWORD_AGILE", "KEYWORD_IGNITE" };

        public override bool ResetCooldownTimerOnUse => true;

        public override int RequiredStock => 1;

        public override SkillSlot SkillSlot => SkillSlot.Utility;

        public override UnlockableDef UnlockableDef => null;
    }
}