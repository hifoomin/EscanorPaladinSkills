using EntityStates;
using RoR2;
using System;
using UnityEngine;
using RoR2.Skills;
using R2API;

namespace EscanorPaladinSkills.SkillDefs
{
    public abstract class SkillDefBase
    {
        public abstract string NameToken { get; }
        public abstract string NameText { get; }
        public abstract string ConfigNameText { get; }
        public abstract string DescriptionText { get; }
        public abstract SerializableEntityStateType ActivationState { get; }
        public abstract string ActivationStateMachineName { get; }
        public abstract int BaseMaxStock { get; }
        public abstract float BaseRechargeInterval { get; }
        public abstract bool BeginSkillCooldownOnSkillEnd { get; }
        public abstract bool CanceledFromSprinting { get; }
        public abstract bool CancelSprintingOnActivation { get; }
        public virtual bool FullRestockOnAssign { get; } = true;
        public abstract InterruptPriority SkillInterruptPriority { get; }
        public abstract bool IsCombatSkill { get; }
        public abstract bool MustKeyPress { get; }
        public abstract int RechargeStock { get; }
        public abstract Sprite Icon { get; }
        public abstract int StockToConsume { get; }
        public abstract string[] KeywordTokens { get; }
        public abstract bool ResetCooldownTimerOnUse { get; }
        public abstract int RequiredStock { get; }
        public abstract SkillSlot SkillSlot { get; }

        public virtual bool isEnabled { get; } = true;
        public virtual bool IsStepped { get; } = false;
        public virtual int StepCount { get; } = 1;
        public virtual float StepGraceDuration { get; } = 0.5f;

        public virtual bool Add { get; } = true;
        public abstract UnlockableDef UnlockableDef { get; }

        public SkillDef skillDef;
        public SteppedSkillDef steppedSkillDef;

        public SkillLocator paladinSkillLocator = PaladinMod.PaladinPlugin.characterPrefab.GetComponent<SkillLocator>();
        /*
        public T ConfigOption<T>(T value, string name, string description)
        {
            return Main.MandoGamingConfig.Bind<T>(NameText, name, value, description).Value;
        }
        */

        public string d(float f)
        {
            return (f * 100f).ToString() + "%";
        }

        public virtual void Init()
        {
            string nameToken = "PALADIN_" + NameToken.ToUpper() + "_NAME";
            string descriptionToken = "PALADIN_" + NameToken.ToUpper() + "_DESCRIPTION";
            LanguageAPI.Add(nameToken, NameText);
            LanguageAPI.Add(descriptionToken, DescriptionText);

            if (IsStepped)
            {
                steppedSkillDef = ScriptableObject.CreateInstance<SteppedSkillDef>();

                steppedSkillDef.skillNameToken = nameToken;
                steppedSkillDef.skillDescriptionToken = descriptionToken;
                steppedSkillDef.activationState = ActivationState;
                steppedSkillDef.activationStateMachineName = ActivationStateMachineName;
                steppedSkillDef.baseMaxStock = BaseMaxStock;
                steppedSkillDef.baseRechargeInterval = BaseRechargeInterval;
                steppedSkillDef.beginSkillCooldownOnSkillEnd = BeginSkillCooldownOnSkillEnd;
                steppedSkillDef.canceledFromSprinting = CanceledFromSprinting;
                steppedSkillDef.cancelSprintingOnActivation = CancelSprintingOnActivation;
                steppedSkillDef.fullRestockOnAssign = FullRestockOnAssign;
                steppedSkillDef.interruptPriority = SkillInterruptPriority;
                steppedSkillDef.isCombatSkill = IsCombatSkill;
                steppedSkillDef.mustKeyPress = MustKeyPress;
                steppedSkillDef.rechargeStock = RechargeStock;
                steppedSkillDef.icon = Icon;
                steppedSkillDef.stockToConsume = StockToConsume;
                steppedSkillDef.keywordTokens = KeywordTokens;
                steppedSkillDef.resetCooldownTimerOnUse = ResetCooldownTimerOnUse;
                steppedSkillDef.requiredStock = RequiredStock;
                steppedSkillDef.stepCount = StepCount;
                steppedSkillDef.stepGraceDuration = StepGraceDuration;

                ContentAddition.AddSkillDef(steppedSkillDef);
            }
            else
            {
                skillDef = ScriptableObject.CreateInstance<SkillDef>();

                skillDef.skillNameToken = nameToken;
                skillDef.skillDescriptionToken = descriptionToken;
                skillDef.activationState = ActivationState;
                skillDef.activationStateMachineName = ActivationStateMachineName;
                skillDef.baseMaxStock = BaseMaxStock;
                skillDef.baseRechargeInterval = BaseRechargeInterval;
                skillDef.beginSkillCooldownOnSkillEnd = BeginSkillCooldownOnSkillEnd;
                skillDef.canceledFromSprinting = CanceledFromSprinting;
                skillDef.cancelSprintingOnActivation = CancelSprintingOnActivation;
                skillDef.fullRestockOnAssign = FullRestockOnAssign;
                skillDef.interruptPriority = SkillInterruptPriority;
                skillDef.isCombatSkill = IsCombatSkill;
                skillDef.mustKeyPress = MustKeyPress;
                skillDef.rechargeStock = RechargeStock;
                skillDef.icon = Icon;
                skillDef.stockToConsume = StockToConsume;
                skillDef.keywordTokens = KeywordTokens;
                skillDef.resetCooldownTimerOnUse = ResetCooldownTimerOnUse;
                skillDef.requiredStock = RequiredStock;
                skillDef.skillName = "Paladin" + NameText.Replace(" ", "");

                ContentAddition.AddSkillDef(skillDef);
            }

            if (Add)
            {
                var skillFamily = paladinSkillLocator.GetSkill(SkillSlot).skillFamily;
                Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
                skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant
                {
                    skillDef = IsStepped ? steppedSkillDef : skillDef,
                    unlockableDef = UnlockableDef,
                    viewableNode = new ViewablesCatalog.Node(IsStepped ? steppedSkillDef.skillNameToken : skillDef.skillNameToken, false, null)
                };
            }

            Main.logger.LogInfo("Added " + NameText);
        }
    }
}