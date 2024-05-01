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
        public float transitionTime;
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
        public ModelLocator modelLocator;
        public Transform modelTransform;

        public float GetTransTime(float timeScalar)
        {
            body = GetComponent<CharacterBody>();
            body.levelArmor = 0f;

            skillLocator = GetComponent<SkillLocator>();
            modelLocator = GetComponent<ModelLocator>();
            modelTransform = modelLocator.modelTransform;

            theOneBuff = Buffs.All.theOneBuff;
            curseBuff = Buffs.All.theOneCurseDebuff;
            chosenTransitionTimer = 240f * timeScalar;
            timeMultiplier = timeScalar;

            if (Run.instance)
            {
                if (!body.HasBuff(curseBuff) && NetworkServer.active)
                {
                    body.AddBuff(curseBuff);
                }

                transitionTimer = chosenTransitionTimer;
                shouldRunTransitionTimer = true;

                // Main.logger.LogError("the one controller chosen transition timer is " + chosenTransitionTimer);

                if (modelTransform)
                {
                    var overlay1PreTransition = chosenTransitionTimer * 0.01f / chosenTransitionTimer;
                    var overlay1JustTransitioning = chosenTransitionTimer / (chosenTransitionTimer + 50f);
                    var overlay1StoppingTransition = (chosenTransitionTimer + 45f) / (chosenTransitionTimer + 50f);
                    var overlay2PreTransition = chosenTransitionTimer * 0.02f / chosenTransitionTimer;
                    var overlay2JustTransitioning = chosenTransitionTimer / (chosenTransitionTimer + 60f);
                    var overlay2StoppingTransition = (chosenTransitionTimer + 55f) / (chosenTransitionTimer + 60f);

                    var overlay1 = modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                    overlay1.duration = chosenTransitionTimer + 50f;
                    overlay1.animateShaderAlpha = true;
                    overlay1.alphaCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(overlay1PreTransition, 0.1f), new Keyframe(overlay1JustTransitioning, 0.4f), new Keyframe(overlay1JustTransitioning + Mathf.Epsilon, 1f), new Keyframe(overlay1StoppingTransition, 1f), new Keyframe(1f, 0f));
                    overlay1.destroyComponentOnEnd = true;
                    overlay1.originalMaterial = Overlays.TheOne.prefab1;
                    overlay1.AddToCharacerModel(modelTransform.GetComponent<CharacterModel>());

                    var overlay2 = modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                    overlay2.duration = chosenTransitionTimer + 60f;
                    overlay2.animateShaderAlpha = true;
                    overlay2.alphaCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(overlay2PreTransition, 0.1f), new Keyframe(overlay2JustTransitioning, 0.4f), new Keyframe(overlay2JustTransitioning + Mathf.Epsilon, 1f), new Keyframe(overlay2StoppingTransition, 1f), new Keyframe(1f, 0f));
                    overlay2.destroyComponentOnEnd = true;
                    overlay2.originalMaterial = Overlays.TheOne.prefab2;
                    overlay2.AddToCharacerModel(modelTransform.GetComponent<CharacterModel>());
                }

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
                        UpgradeSkills();

                        Util.PlaySound("Play_lemurian_fireball_impact", gameObject);
                        Util.PlaySound("Play_lemurian_fireball_impact", gameObject);
                        Util.PlaySound("Play_lemurian_fireball_impact", gameObject);
                        Util.PlaySound("Play_moonBrother_orb_slam_impact", gameObject);

                        // EffectManager.SpawnEffect()

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
                        RevertSkills();

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

        public void UpgradeSkills()
        {
            if (body.hasAuthority)
            {
                if (Main.originalPrimarySkillDefToPrimarySkillDefUpgradeMap.TryGetValue(skillLocator.primary.baseSkill, out var upgradedPrimary))
                {
                    skillLocator.primary.SetSkillOverride(gameObject, upgradedPrimary, GenericSkill.SkillOverridePriority.Contextual);
                }
                else
                {
                    Main.logger.LogError("The One: No suitable primary skill upgrade was found");
                }

                if (Main.originalSecondarySkillDefToSecondarySkillDefUpgradeMap.TryGetValue(skillLocator.secondary.baseSkill, out var upgradedSecondary))
                {
                    skillLocator.secondary.SetSkillOverride(gameObject, upgradedSecondary, GenericSkill.SkillOverridePriority.Contextual);
                }
                else
                {
                    Main.logger.LogError("The One: No suitable secondary skill upgrade was found");
                }

                if (Main.originalUtilitySkillDefToUtilitySkillDefUpgradeMap.TryGetValue(skillLocator.utility.baseSkill, out var upgradedUtility))
                {
                    skillLocator.utility.SetSkillOverride(gameObject, upgradedUtility, GenericSkill.SkillOverridePriority.Contextual);
                }
                else
                {
                    Main.logger.LogError("The One: No suitable utility skill upgrade was found");
                }

                if (Main.originalSpecialSkillDefToSpecialSkillDefUpgradeMap.TryGetValue(skillLocator.special.baseSkill, out var upgradedSpecial))
                {
                    skillLocator.special.SetSkillOverride(gameObject, upgradedSpecial, GenericSkill.SkillOverridePriority.Contextual);
                }
                else
                {
                    Main.logger.LogError("The One: No suitable special skill upgrade was found");
                }
            }
        }

        public void RevertSkills()
        {
            if (body.hasAuthority)
            {
                if (Main.originalPrimarySkillDefToPrimarySkillDefUpgradeMap.TryGetValue(skillLocator.primary.baseSkill, out var upgradedPrimary))
                {
                    skillLocator.primary.UnsetSkillOverride(gameObject, upgradedPrimary, GenericSkill.SkillOverridePriority.Contextual);
                }

                if (Main.originalSecondarySkillDefToSecondarySkillDefUpgradeMap.TryGetValue(skillLocator.secondary.baseSkill, out var upgradedSecondary))
                {
                    skillLocator.secondary.UnsetSkillOverride(gameObject, upgradedSecondary, GenericSkill.SkillOverridePriority.Contextual);
                }

                if (Main.originalUtilitySkillDefToUtilitySkillDefUpgradeMap.TryGetValue(skillLocator.utility.baseSkill, out var upgradedUtility))
                {
                    skillLocator.utility.UnsetSkillOverride(gameObject, upgradedUtility, GenericSkill.SkillOverridePriority.Contextual);
                }

                if (Main.originalSpecialSkillDefToSpecialSkillDefUpgradeMap.TryGetValue(skillLocator.special.baseSkill, out var upgradedSpecial))
                {
                    skillLocator.special.UnsetSkillOverride(gameObject, upgradedSpecial, GenericSkill.SkillOverridePriority.Contextual);
                }
            }
        }
    }
}