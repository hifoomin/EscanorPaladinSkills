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
        public static Dictionary<SkillDef, SkillDef> originalSecondarySkillDefToSecondarySkillDefUpgradeMap = new();
        public static Dictionary<SkillDef, SkillDef> originalUtilitySkillDefToUtilitySkillDefUpgradeMap = new();
        public static Dictionary<SkillDef, SkillDef> originalSpecialSkillDefToSpecialSkillDefUpgradeMap = new();

        public static BodyIndex paladinBodyIndex;

        public static Xoroshiro128Plus rng;

        public void Awake()
        {
            logger = base.Logger;

            On.RoR2.BodyCatalog.Init += BodyCatalog_Init;
            Run.onRunStartGlobal += Run_onRunStartGlobal;

            // On.RoR2.Networking.NetworkManagerSystemSteam.OnClientConnect += (s, u, t) => { };

            escanor = AssetBundle.LoadFromFile(Assembly.GetExecutingAssembly().Location.Replace("EscanorPaladinSkills.dll", "escanorpaladinskills"));

            Buffs.All.Init();
            VFX.DivineAxeRhitta.Init();
            Overlays.FlameOfLife.Init();
            Overlays.TheOne.Init();
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

        public void Start() // paladin realification happens in start for whatever reason
        {
            TheOneSD.Init();
            DivineAxeRhittaJank.Init();

            AddESM();

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

            AddStates();
            AddSkillUpgrades();

            // 1 = primary
            // 2 = spinning slash m2
            // 3 =

            Init.SetUpComponents();
        }

        public void AddESM()
        {
            var pally = PaladinMod.PaladinPlugin.characterPrefab;

            var networkStateMachine = pally.GetComponent<NetworkStateMachine>();

            var flameESM = pally.AddComponent<EntityStateMachine>();
            flameESM.customName = "Flame";
            flameESM.initialStateType = new(typeof(EntityStates.Idle));
            flameESM.mainStateType = new(typeof(EntityStates.Idle));

            Array.Resize(ref networkStateMachine.stateMachines, networkStateMachine.stateMachines.Length + 1);
            networkStateMachine.stateMachines[networkStateMachine.stateMachines.Length - 1] = flameESM;
        }

        public void AddStates()
        {
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
        }

        public void AddSkillUpgrades()
        {
            originalSecondarySkillDefToSecondarySkillDefUpgradeMap.Add(SunshineCruelSunSD.instance.skillDef, SunshineCruelSunUpgradedSD.instance.skillDef);
            originalSecondarySkillDefToSecondarySkillDefUpgradeMap.Add(PaladinMod.Modules.Skills.skillFamilies[1].variants[0].skillDef, SpinningSlashUpgradedSD.instance.skillDef);
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
    }
}