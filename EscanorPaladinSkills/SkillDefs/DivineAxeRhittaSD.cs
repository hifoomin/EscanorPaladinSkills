using EntityStates;
using EscanorPaladinSkills.States;
using RoR2;
using UnityEngine;

namespace EscanorPaladinSkills.SkillDefs
{
    public class DivineAxeRhittaSD : SkillDefBase<DivineAxeRhittaSD>
    {
        public override string NameToken => "DIVINEAXERHITTA";

        public override string NameText => "Divine Axe Rhitta";

        public override string ConfigNameText => "Divine Axe Rhitta";

        public override string DescriptionText => "<style=cIsDamage>Fleeting</style>. Raise your axe for <style=cIsDamage>700% damage</style>. <style=cIsUtility>Grounds nearby enemies</style> if the Paladin is <style=cIsHealing>blessed</style>.";

        public override SerializableEntityStateType ActivationState => new(typeof(DivineAxeRhittaState));

        public override string ActivationStateMachineName => "Weapon";

        public override int BaseMaxStock => 1;

        public override float BaseRechargeInterval => 0f;

        public override bool BeginSkillCooldownOnSkillEnd => true;

        public override bool CanceledFromSprinting => false;

        public override bool CancelSprintingOnActivation => false;

        public override InterruptPriority SkillInterruptPriority => InterruptPriority.Any;

        public override bool IsCombatSkill => true;

        public override bool MustKeyPress => false;

        public override int RechargeStock => 0;

        public override Sprite Icon => Main.escanor.LoadAsset<Sprite>("texDivineAxeRhitta4.png");

        public override int StockToConsume => 0;

        public override string[] KeywordTokens => new string[] { "KEYWORD_FLEETING" };

        public override bool ResetCooldownTimerOnUse => true;

        public override int RequiredStock => 1;

        public override SkillSlot SkillSlot => SkillSlot.Primary;

        public override UnlockableDef UnlockableDef => null;

        public override bool IsStepped => true;

        public override int StepCount => 4;
    }
}