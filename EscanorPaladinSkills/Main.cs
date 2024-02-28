using BepInEx;
using BepInEx.Logging;
using R2API;
using R2API.ContentManagement;
using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine;
using System.Linq;
using RoR2;
using EscanorPaladinSkills.States;
using EscanorPaladinSkills.VFX;
using EscanorPaladinSkills.SkillDefs;
using EscanorPaladinSkills.Components;
using RoR2.Skills;
using EscanorPaladinSkills.SkillDefs.Upgrades;
using EscanorPaladinSkills.States.Upgrades;
using UnityEngine.SceneManagement;
using RoR2.UI;
using UnityEngine.Networking;

namespace EscanorPaladinSkills
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(LanguageAPI.PluginGUID)]
    [BepInDependency(PrefabAPI.PluginGUID)]
    [BepInDependency(R2APIContentManager.PluginGUID)]
    [BepInDependency("com.rob.Paladin", BepInDependency.DependencyFlags.HardDependency)]
    public class Main : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;

        public const string PluginAuthor = "HIFU";
        public const string PluginName = "EscanorPaladinSkills";
        public const string PluginVersion = "1.0.0";

        public static AssetBundle escanor;

        public static ManualLogSource logger;

        public static Dictionary<SkillDef, SkillDef> originalPrimarySkillDefToPrimarySkillDefUpgradeMap = new();
        public static Dictionary<SkillDef, SkillDef> originalSecondarySkillDefToPrimarySkillDefUpgradeMap = new();
        public static Dictionary<SkillDef, SkillDef> originalUtilitySkillDefToPrimarySkillDefUpgradeMap = new();
        public static Dictionary<SkillDef, SkillDef> originalSpecialSkillDefToPrimarySkillDefUpgradeMap = new();

        public static BodyIndex paladinBodyIndex;

        public static Xoroshiro128Plus rng;

        public void Awake()
        {
            logger = base.Logger;

            On.RoR2.BodyCatalog.Init += BodyCatalog_Init;
            Run.onRunStartGlobal += Run_onRunStartGlobal;

            On.RoR2.Networking.NetworkManagerSystemSteam.OnClientConnect += (s, u, t) => { };

            escanor = AssetBundle.LoadFromFile(Assembly.GetExecutingAssembly().Location.Replace("EscanorPaladinSkills.dll", "escanorpaladinskills"));

            Buffs.All.Init();
            VFX.DivineAxeRhitta.Init();
            Overlays.FlameOfLife.Init();
            Projectiles.CruelSun.Init();
            Projectiles.CruelSunUpgraded.Init();
            VFX.Judgement.Init();
            Projectiles.Judgement.Init();
            Projectiles.Sunfall.Init();
        }

        private void Run_onRunStartGlobal(Run run)
        {
            rng = new Xoroshiro128Plus(RoR2Application.rng.nextUlong);
        }

        private void BodyCatalog_Init(On.RoR2.BodyCatalog.orig_Init orig)
        {
            orig();
            paladinBodyIndex = BodyCatalog.FindBodyIndex("RobPaladinBody");
        }

        public void Start()
        {
            TheOneSD.Init();
            DivineAxeRhittaJank.Init();

            var pally = PaladinMod.PaladinPlugin.characterPrefab;

            var networkStateMachine = pally.GetComponent<NetworkStateMachine>();

            var flameESM = pally.AddComponent<EntityStateMachine>();
            flameESM.customName = "Flame";
            flameESM.initialStateType = new(typeof(EntityStates.Idle));
            flameESM.mainStateType = new(typeof(EntityStates.Idle));

            Array.Resize(ref networkStateMachine.stateMachines, networkStateMachine.stateMachines.Length + 1);
            networkStateMachine.stateMachines[networkStateMachine.stateMachines.Length - 1] = flameESM;

            IEnumerable<Type> enumerable = from type in Assembly.GetExecutingAssembly().GetTypes()
                                           where !type.IsAbstract && type.IsSubclassOf(typeof(SkillDefBase))
                                           select type;

            logger.LogInfo("==+----------------==SKILLS==----------------+==");

            foreach (Type type in enumerable)
            {
                SkillDefBase based = (SkillDefBase)Activator.CreateInstance(type);
                if (ValidateSkillDef(based))
                {
                    based.Init();
                }
            }

            ContentAddition.AddEntityState(typeof(CruelSunState), out _);
            ContentAddition.AddEntityState(typeof(CruelSunUpgradedState), out _);
            ContentAddition.AddEntityState(typeof(FlameOfLifeState), out _);
            ContentAddition.AddEntityState(typeof(DivineAxeRhittaState), out _);
            ContentAddition.AddEntityState(typeof(SunfallState), out _);
            ContentAddition.AddEntityState(typeof(States.Upgrades.SpinningSlash.SpinningSlashAirborneAltUpgradedState), out _);
            ContentAddition.AddEntityState(typeof(States.Upgrades.SpinningSlash.SpinningSlashAirborneUpgradedState), out _);
            ContentAddition.AddEntityState(typeof(States.Upgrades.SpinningSlash.SpinningSlashEntryUpgradedState), out _);
            ContentAddition.AddEntityState(typeof(States.Upgrades.SpinningSlash.SpinningSlashGroundedAltUpgradedState), out _);
            ContentAddition.AddEntityState(typeof(States.Upgrades.SpinningSlash.SpinningSlashGroundedUpgradedState), out _);
            ContentAddition.AddEntityState(typeof(CruelSunUpgradedState), out _);
            ContentAddition.AddEntityState(typeof(DivineAxeRhittaUpgradedState), out _);

            // 1 = primary
            // 2 = spinning slash m2
            // 3 =

            originalSecondarySkillDefToPrimarySkillDefUpgradeMap.Add(SunshineCruelSunSD.instance.skillDef, SunshineCruelSunUpgradedSD.instance.skillDef);
            originalSecondarySkillDefToPrimarySkillDefUpgradeMap.Add(PaladinMod.Modules.Skills.skillFamilies[1].variants[0].skillDef, SpinningSlashUpgradedSD.instance.skillDef);

            CharacterBody.onBodyStartGlobal += CharacterBody_onBodyStartGlobal;
            HUD.shouldHudDisplay += HUD_shouldHudDisplay;
        }

        private void HUD_shouldHudDisplay(HUD hud, ref bool shouldDisplay)
        {
            if (hud.GetComponent<TheOneHUD>() == null)
                hud.gameObject.AddComponent<TheOneHUD>();
        }

        private void CharacterBody_onBodyStartGlobal(CharacterBody body)
        {
            if (body.bodyIndex != paladinBodyIndex)
            {
                return;
            }

            var passive = body.GetComponents<GenericSkill>().Where(x => x.skillDef.skillNameToken == "PALADIN_THEONE_NAME").FirstOrDefault();
            if (passive)
            {
                if (body.GetComponent<TheOneController>() == null)
                {
                    logger.LogError("adding the one controller");
                    var theOneController = body.gameObject.AddComponent<TheOneController>();
                    var sceneName = SceneManager.GetActiveScene().name;
                    var timeMultiplier = sceneName switch
                    {
                        "moon" => 1.5f,
                        "moon2" => 1.75f,
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

            if (NetworkServer.active && body.master)
            {
                body.master.Respawn(body.footPosition, body.transform.rotation);
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

            if (trans.Find("sesbian lex hitbox") != null)
            {
                return;
            }

            GameObject hitBox2 = new("sesbian lex hitbox");
            hitBox2.transform.parent = trans;
            hitBox2.AddComponent<HitBox>();
            hitBox2.transform.localPosition = new Vector3(0f, 0f, 22f);
            hitBox2.transform.localScale = new Vector3(8f, 4000f, 48f);
            hitBox2.transform.localEulerAngles = Vector3.zero;
            var hitBoxGroup2 = trans.gameObject.AddComponent<HitBoxGroup>();
            hitBoxGroup2.hitBoxes = new HitBox[] { hitBox2.GetComponent<HitBox>() };
            hitBoxGroup2.groupName = "SesbianLex";

            if (body.GetComponent<SkillLocator>().primary.skillDef.skillNameToken == "PALADIN_DIVINEAXERHITTA_NAME")
            {
                var childLocator = trans.GetComponent<ChildLocator>();
                var lowerArmR = childLocator.transformPairs[35].transform;
                // lowerArmR.localScale = Vector3.one * 1.5f;

                var swordBase = lowerArmR.Find("hand.R/swordBase");
                var stupid = swordBase.gameObject.AddComponent<StupidSwordController>();
                stupid.sword = swordBase;
            }
        }

        public bool ValidateSkillDef(SkillDefBase sdb)
        {
            if (sdb.isEnabled)
            {
                bool enabledfr = Config.Bind(sdb.ConfigNameText, "Enable?", true, "Vanilla is false").Value;
                if (enabledfr)
                {
                    return true;
                }
            }
            return false;
        }

        public class StupidSwordController : MonoBehaviour
        {
            public Vector3 idealSwordSize;
            public Transform sword;

            public void Start()
            {
                idealSwordSize = Vector3.one * 1.5f;
            }

            public void LateUpdate()
            {
                if (!sword)
                {
                    return;
                }

                sword.localScale = idealSwordSize;
            }
        }
    }
}