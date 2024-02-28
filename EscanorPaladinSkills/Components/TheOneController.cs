using EscanorPaladinSkills.SkillDefs;
using EscanorPaladinSkills.SkillDefs.Upgrades;
using RoR2;
using RoR2.Skills;
using RoR2.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace EscanorPaladinSkills.Components
{
    [DisallowMultipleComponent]
    public class TheOneController : MonoBehaviour
    {
        public float[] transitionTimes;
        public float transitionTimer = 0f;
        public float chosenTransitionTimer = 0f;
        public bool shouldRunTransitionTimer = false;
        public bool shouldRunTheOneTimer = false;
        public BuffDef theOneBuff;
        public BuffDef curseBuff;
        public CharacterBody body;
        public bool initializedHUD = false;
        public SkillLocator skillLocator;
        public float theOneTimer = 0f;
        public float timeMultiplier;

        public enum SwitchType
        {
            Upgrade,
            Standard
        }

        public float GetTransTime(float timeScalar)
        {
            body = GetComponent<CharacterBody>();
            body.levelArmor = 0f;

            skillLocator = GetComponent<SkillLocator>();

            theOneBuff = Buffs.All.theOne;
            curseBuff = Buffs.All.curse;
            transitionTimes = new float[4];
            transitionTimes[0] = 160f * timeScalar;
            transitionTimes[1] = 180f * timeScalar;
            transitionTimes[2] = 200f * timeScalar;
            transitionTimes[3] = 215f * timeScalar;
            timeMultiplier = timeScalar;

            if (Run.instance)
            {
                if (!body.HasBuff(curseBuff) && NetworkServer.active)
                {
                    body.AddBuff(curseBuff);
                }

                chosenTransitionTimer = transitionTimes[Main.rng.RangeInt(0, transitionTimes.Length)];
                transitionTimer = chosenTransitionTimer;
                shouldRunTransitionTimer = true;

                Main.logger.LogError("the one controller chosen transition timer is " + chosenTransitionTimer);

                return chosenTransitionTimer;
            }
            return float.MaxValue;
        }

        public void FixedUpdate()
        {
            if (shouldRunTransitionTimer)
            {
                transitionTimer -= Time.fixedDeltaTime;
                if (transitionTimer <= 0f)
                {
                    if (body.HasBuff(curseBuff) && NetworkServer.active)
                    {
                        body.RemoveBuff(curseBuff);
                    }
                    if (!body.HasBuff(theOneBuff))
                    {
                        SwitchSkills(SwitchType.Upgrade);

                        if (NetworkServer.active)
                        {
                            body.AddBuff(theOneBuff);
                        }
                    }
                    shouldRunTheOneTimer = true;
                    shouldRunTransitionTimer = false;
                }
            }
            if (shouldRunTheOneTimer)
            {
                theOneTimer += Time.fixedDeltaTime;
                if (theOneTimer >= 60f)
                {
                    if (!body.HasBuff(curseBuff) && NetworkServer.active)
                    {
                        body.AddBuff(curseBuff);
                    }
                    if (body.HasBuff(theOneBuff))
                    {
                        SwitchSkills(SwitchType.Standard);

                        if (NetworkServer.active)
                        {
                            body.RemoveBuff(theOneBuff);
                        }
                    }
                    theOneTimer = -999f;
                    shouldRunTheOneTimer = false;
                }
            }
        }

        public void SwitchSkills(SwitchType switchType)
        {
            if (body.hasAuthority)
            {
                if (Main.originalPrimarySkillDefToPrimarySkillDefUpgradeMap.TryGetValue(skillLocator.primary.baseSkill, out var upgradedPrimary))
                {
                    if (switchType == SwitchType.Upgrade)
                    {
                        skillLocator.primary.SetSkillOverride(gameObject, upgradedPrimary, GenericSkill.SkillOverridePriority.Contextual);
                    }
                    else
                    {
                        skillLocator.primary.UnsetSkillOverride(gameObject, upgradedPrimary, GenericSkill.SkillOverridePriority.Contextual);
                    }
                }
                else
                {
                    Main.logger.LogError("The One: No suitable primary skill upgrade was found");
                }

                if (Main.originalSecondarySkillDefToSecondarySkillDefUpgradeMap.TryGetValue(skillLocator.secondary.baseSkill, out var upgradedSecondary))
                {
                    if (switchType == SwitchType.Upgrade)
                    {
                        skillLocator.secondary.SetSkillOverride(gameObject, upgradedSecondary, GenericSkill.SkillOverridePriority.Contextual);
                    }
                    else
                    {
                        skillLocator.secondary.UnsetSkillOverride(gameObject, upgradedSecondary, GenericSkill.SkillOverridePriority.Contextual);
                    }
                }
                else
                {
                    Main.logger.LogError("The One: No suitable secondary skill upgrade was found");
                }

                if (Main.originalUtilitySkillDefToUtilitySkillDefUpgradeMap.TryGetValue(skillLocator.utility.baseSkill, out var upgradedUtility))
                {
                    if (switchType == SwitchType.Upgrade)
                    {
                        skillLocator.utility.SetSkillOverride(gameObject, upgradedUtility, GenericSkill.SkillOverridePriority.Contextual);
                    }
                    else
                    {
                        skillLocator.utility.UnsetSkillOverride(gameObject, upgradedUtility, GenericSkill.SkillOverridePriority.Contextual);
                    }
                }
                else
                {
                    Main.logger.LogError("The One: No suitable secondary skill upgrade was found");
                }

                if (Main.originalSpecialSkillDefToSpecialSkillDefUpgradeMap.TryGetValue(skillLocator.special.baseSkill, out var upgradedSpecial))
                {
                    if (switchType == SwitchType.Upgrade)
                    {
                        skillLocator.special.SetSkillOverride(gameObject, upgradedSpecial, GenericSkill.SkillOverridePriority.Contextual);
                    }
                    else
                    {
                        skillLocator.special.UnsetSkillOverride(gameObject, upgradedSpecial, GenericSkill.SkillOverridePriority.Contextual);
                    }
                }
                else
                {
                    Main.logger.LogError("The One: No suitable secondary skill upgrade was found");
                }
            }
        }
    }
}