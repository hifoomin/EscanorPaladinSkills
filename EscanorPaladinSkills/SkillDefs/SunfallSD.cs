using EntityStates;
using EscanorPaladinSkills.States;
using RoR2;
using UnityEngine;

namespace EscanorPaladinSkills.SkillDefs
{
    public class SunfallSD : SkillDefBase<SunfallSD>
    {
        public override string NameToken => "SUNFALL";

        public override string NameText => "Sunfall";

        public override string ConfigNameText => "Sunfall";

        public override string DescriptionText => "<style=cIsDamage>Fleeting</style>. <style=cDeath>Stun yourself</style> and call down your hand, summoning <style=cIsDamage>20</style> meteors for <style=cIsDamage>400% damage</style> each.";

        public override SerializableEntityStateType ActivationState => new(typeof(SunfallState));

        public override string ActivationStateMachineName => "Weapon";

        public override int BaseMaxStock => 5;

        public override float BaseRechargeInterval => 6f;

        public override bool BeginSkillCooldownOnSkillEnd => true;

        public override bool CanceledFromSprinting => false;

        public override bool CancelSprintingOnActivation => false;

        public override InterruptPriority SkillInterruptPriority => InterruptPriority.Any;

        public override bool IsCombatSkill => true;

        public override bool MustKeyPress => false;

        public override int RechargeStock => 1;

        public override Sprite Icon => Main.escanor.LoadAsset<Sprite>("texSunshinesCruelSun.png");

        public override int StockToConsume => 5;

        public override string[] KeywordTokens => new string[] { "KEYWORD_FLEETING" };

        public override bool ResetCooldownTimerOnUse => true;

        public override int RequiredStock => 5;

        public override SkillSlot SkillSlot => SkillSlot.Special;

        public override UnlockableDef UnlockableDef => null;
    }
}