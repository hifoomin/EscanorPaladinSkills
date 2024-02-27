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

        private void HUD_shouldHudDisplay(HUD hud, ref bool shouldDisplay)
        {
            if (initializedHUD)
            {
                return;
            }

            hud.gameObject.AddComponent<TheOneHUD>();
            initializedHUD = true;
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
            HUD.shouldHudDisplay += HUD_shouldHudDisplay;

            if (Run.instance)
            {
                if (!body.HasBuff(curseBuff) && NetworkServer.active)
                {
                    body.AddBuff(curseBuff);
                }

                chosenTransitionTimer = transitionTimes[Run.instance.treasureRng.RangeInt(0, transitionTimes.Length)];
                transitionTimer = chosenTransitionTimer;
                shouldRunTransitionTimer = true;

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
                        if (Main.originalPrimarySkillDefToPrimarySkillDefUpgradeMap.TryGetValue(skillLocator.primary.baseSkill, out var upgradedPrimary))
                        {
                            skillLocator.primary.SetSkillOverride(gameObject, upgradedPrimary, GenericSkill.SkillOverridePriority.Contextual);
                        }
                        else
                        {
                            Main.logger.LogError("The One: No suitable primary skill upgrade was found");
                        }

                        if (Main.originalSecondarySkillDefToPrimarySkillDefUpgradeMap.TryGetValue(skillLocator.secondary.baseSkill, out var upgradedSecondary))
                        {
                            skillLocator.secondary.SetSkillOverride(gameObject, upgradedSecondary, GenericSkill.SkillOverridePriority.Contextual);
                        }
                        else
                        {
                            Main.logger.LogError("The One: No suitable secondary skill upgrade was found");
                        }

                        if (Main.originalUtilitySkillDefToPrimarySkillDefUpgradeMap.TryGetValue(skillLocator.utility.baseSkill, out var upgradedUtility))
                        {
                            skillLocator.utility.SetSkillOverride(gameObject, upgradedUtility, GenericSkill.SkillOverridePriority.Contextual);
                        }
                        else
                        {
                            Main.logger.LogError("The One: No suitable utility skill upgrade was found");
                        }

                        if (Main.originalSpecialSkillDefToPrimarySkillDefUpgradeMap.TryGetValue(skillLocator.special.baseSkill, out var upgradedSpecial))
                        {
                            skillLocator.special.SetSkillOverride(gameObject, upgradedSpecial, GenericSkill.SkillOverridePriority.Contextual);
                        }
                        else
                        {
                            Main.logger.LogError("The One: No suitable special skill upgrade was found");
                        }
                        if (NetworkServer.active)
                            body.AddBuff(theOneBuff);
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
                        if (Main.originalPrimarySkillDefToPrimarySkillDefUpgradeMap.TryGetValue(skillLocator.primary.baseSkill, out var upgradedPrimary))
                        {
                            skillLocator.primary.UnsetSkillOverride(gameObject, upgradedPrimary, GenericSkill.SkillOverridePriority.Contextual);
                        }

                        if (Main.originalSecondarySkillDefToPrimarySkillDefUpgradeMap.TryGetValue(skillLocator.secondary.baseSkill, out var upgradedSecondary))
                        {
                            skillLocator.secondary.UnsetSkillOverride(gameObject, upgradedSecondary, GenericSkill.SkillOverridePriority.Contextual);
                        }

                        if (Main.originalUtilitySkillDefToPrimarySkillDefUpgradeMap.TryGetValue(skillLocator.utility.baseSkill, out var upgradedUtility))
                        {
                            skillLocator.utility.UnsetSkillOverride(gameObject, upgradedUtility, GenericSkill.SkillOverridePriority.Contextual);
                        }

                        if (Main.originalSpecialSkillDefToPrimarySkillDefUpgradeMap.TryGetValue(skillLocator.special.baseSkill, out var upgradedSpecial))
                        {
                            skillLocator.special.UnsetSkillOverride(gameObject, upgradedSpecial, GenericSkill.SkillOverridePriority.Contextual);
                        }
                        if (NetworkServer.active)
                            body.RemoveBuff(theOneBuff);
                    }
                    theOneTimer = -999f;
                    shouldRunTheOneTimer = false;
                }
            }
        }

        public void OnDestroy()
        {
            HUD.shouldHudDisplay -= HUD_shouldHudDisplay;
        }
    }

    [DisallowMultipleComponent]
    public class TheOneHUD : MonoBehaviour
    {
        public HUD hud;
        public HGTextMeshProUGUI textMesh;
        public CharacterBody body;
        public TheOneController theOneController;

        public IEnumerator Init()
        {
            if (!Run.instance)
            {
                yield break;
            }

            if (!hud.targetMaster)
            {
                yield break;
            }

            body = hud.targetMaster.GetBody();

            if (body && body.bodyIndex == Main.paladinBodyIndex)
            {
                Main.logger.LogError("finally initialized holy shit");
                theOneController = body.GetComponent<TheOneController>();
            }

            yield return null;
        }

        public void Start()
        {
            hud = GetComponent<HUD>();

            var theOneContainer = new GameObject("TheOneContainer");
            // Main.logger.LogError("the one container is " + theOneContainer);

            var locator = hud.GetComponent<ChildLocator>();
            var upperRightCluster = locator.FindChild("TopCenterCluster").parent.Find("UpperRightCluster");
            // Main.logger.LogError("upper right cluster is " + upperRightCluster);
            theOneContainer.transform.SetParent(upperRightCluster);
            var rect = theOneContainer.AddComponent<RectTransform>();
            rect.localPosition = Vector3.zero;
            rect.localScale = Vector3.one;
            rect.localEulerAngles = Vector3.zero;
            var elem = theOneContainer.AddComponent<LayoutElement>();

            elem.minHeight = 120;
            elem.preferredWidth = 500;

            rect.pivot = new Vector2(1.15f, 0);
            rect.anchoredPosition = rect.pivot;

            //var image = uiContainer.AddComponent<Image>();
            //image.material = _hud.itemInventoryDisplay.GetComponentInChildren<Image>().material;

            //var textContainer = new GameObject("TextContainer");
            //textContainer.transform.SetParent(uiContainer.transform);
            //var textMesh = textContainer.AddComponent<HGTextMeshProUGUI>();
            textMesh = theOneContainer.AddComponent<HGTextMeshProUGUI>();
            textMesh.fontSize = 32;
            textMesh.alignment = TextAlignmentOptions.Center;
            textMesh.text = "";
        }

        public void Update()
        {
            if (!body || !theOneController)
            {
                StartCoroutine(Init());
                return;
            }

            var transitionTimer = theOneController.transitionTimer;
            var theOneTimer = theOneController.theOneTimer;

            if (transitionTimer >= 60f * theOneController.timeMultiplier || theOneTimer <= 30f)
            {
                textMesh.color = new Color32(255, 105, 34, 255);
            }
            else if (transitionTimer <= 30f * theOneController.timeMultiplier || theOneTimer >= 50f)
            {
                textMesh.color = new Color32(204, 34, 34, 255);
            }
            else
            {
                textMesh.color = new Color32(204, 71, 34, 255);
            }

            var transIntegerPart = ((int)transitionTimer).ToString("#,0");
            var transDecimalPart = (transitionTimer - (int)transitionTimer).ToString("0.00").Substring(1);

            var integerPart = ((int)theOneTimer).ToString("#,0");
            var decimalPart = (theOneTimer - (int)theOneTimer).ToString("0.00").Substring(1);

            if (transitionTimer > 0f)
            {
                textMesh.text = "<mspace=0.5em>The One: " + transIntegerPart + "<sup>" + transDecimalPart + "</sup></mspace>";
            }
            else if (theOneTimer < -900f)
            {
                textMesh.text = string.Empty;
            }
            else
            {
                textMesh.text = "<mspace=0.5em>The One: " + integerPart + "<sup>" + decimalPart + "</sup></mspace>";
            }
        }
    }
}