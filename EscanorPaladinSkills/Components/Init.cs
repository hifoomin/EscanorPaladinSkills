using UnityEngine.SceneManagement;
using UnityEngine;
using RoR2;
using RoR2.UI;
using System.Linq;

namespace EscanorPaladinSkills.Components
{
    public class Init
    {
        public static void SetUpComponents()
        {
            // Main.logger.LogError("setting up components");
            CharacterBody.onBodyStartGlobal += CharacterBody_onBodyStartGlobal;
            HUD.shouldHudDisplay += HUD_shouldHudDisplay;
        }

        public static void HUD_shouldHudDisplay(HUD hud, ref bool shouldDisplay)
        {
            if (hud.GetComponent<TheOneHUD>() == null)
                hud.gameObject.AddComponent<TheOneHUD>();
        }

        public static void CharacterBody_onBodyStartGlobal(CharacterBody body)
        {
            if (body.name != "RobPaladinBody(Clone)")
            {
                return;
            }

            // Main.logger.LogError("found paladin body");

            var passive = body.GetComponents<GenericSkill>().Where(x => x.skillDef.skillNameToken == "PALADIN_THEONE_NAME").FirstOrDefault();
            if (passive)
            {
                if (body.GetComponent<TheOneController>() == null)
                {
                    // Main.logger.LogError("adding the one controller");
                    var theOneController = body.gameObject.AddComponent<TheOneController>();
                    var sceneName = SceneManager.GetActiveScene().name;
                    var timeMultiplier = sceneName switch
                    {
                        "moon" => 2f,
                        "moon2" => 2.5f,
                        "voidstage" => 0.5f,
                        "limbo" => 0.1f,
                        "arena" => 0.75f,
                        _ => 1f
                    };
                    var speed = body.moveSpeed / 7f;
                    timeMultiplier /= Mathf.Sqrt(speed);
                    theOneController.GetTransTime(timeMultiplier);
                }
            }

            var modelLocator = body.modelLocator;
            if (!modelLocator)
            {
                return;
            }

            var trans = modelLocator.modelTransform;
            if (!trans)
            {
                return;
            }

            if (trans.Find("say gex hitbox") != null)
            {
                return;
            }

            GameObject hitBox = new("say gex hitbox");
            hitBox.transform.parent = trans;
            hitBox.AddComponent<HitBox>();
            hitBox.transform.localPosition = new Vector3(0f, 0f, 7f);
            hitBox.transform.localScale = new Vector3(16f, 20f, 24f);
            hitBox.transform.localEulerAngles = Vector3.zero;
            var hitBoxGroup = trans.gameObject.AddComponent<HitBoxGroup>();
            hitBoxGroup.hitBoxes = new HitBox[] { hitBox.GetComponent<HitBox>() };
            hitBoxGroup.groupName = "SayGex";

            if (body.GetComponent<SkillLocator>().primary.skillDef.skillNameToken == "PALADIN_DIVINEAXERHITTA_NAME")
            {
                // Main.logger.LogError("m1 is divine axe rhitta");
                var childLocator = trans.GetComponent<ChildLocator>();
                var transformPairs = childLocator.transformPairs;
                if (transformPairs.Length > 36)
                {
                    // Main.logger.LogError("transform pairs has more than 36 elements");
                    var lowerArmR = childLocator.transformPairs[35].transform;
                    if (lowerArmR)
                    {
                        // Main.logger.LogError("lower arm r exists");
                        var swordBase = lowerArmR.Find("hand.R/swordBase");
                        if (swordBase)
                        {
                            // Main.logger.LogError("sword base exists");
                            var swordSizeController = swordBase.gameObject.AddComponent<SwordSizeController>();
                            swordSizeController.sword = swordBase;
                        }
                    }
                }
            }
        }
    }
}