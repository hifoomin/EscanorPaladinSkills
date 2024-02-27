using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using RoR2.Skills;
using RoR2.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using static RoR2.UI.CharacterSelectController;

namespace EscanorPaladinSkills.SkillDefs
{
    public static class TheOneSD
    {
        public static GenericSkill skill;
        public static SkillFamily family => skill?.skillFamily;
        public static SkillDef theOneDef;

        public static void Init()
        {
            var bodyPrefab = PaladinMod.PaladinPlugin.characterPrefab;

            foreach (var comp in bodyPrefab.GetComponents<GenericSkill>())
            {
                if ((comp.skillFamily as ScriptableObject).name.ToLower().Contains("passive"))
                {
                    skill = comp;
                    return;
                }
            }
            skill = bodyPrefab.AddComponent<GenericSkill>();
            SkillLocator locator = bodyPrefab.GetComponent<SkillLocator>();
            skill._skillFamily = ScriptableObject.CreateInstance<SkillFamily>();
            (skill.skillFamily as ScriptableObject).name = bodyPrefab.name + "Passive";
            skill.skillFamily.variants = new SkillFamily.Variant[2];
            skill.skillName = bodyPrefab.name + "Passive";
            locator.passiveSkill.enabled = false;

            var passiveDef = ScriptableObject.CreateInstance<SkillDef>();
            passiveDef.skillNameToken = locator.passiveSkill.skillNameToken;
            (passiveDef as ScriptableObject).name = passiveDef.skillNameToken;
            passiveDef.skillDescriptionToken = locator.passiveSkill.skillDescriptionToken;
            passiveDef.icon = locator.passiveSkill.icon;
            passiveDef.keywordTokens = locator.passiveSkill.keywordToken.Length > 0 ? new string[] { locator.passiveSkill.keywordToken } : null;
            passiveDef.baseRechargeInterval = 0f;
            passiveDef.activationStateMachineName = "Body";
            passiveDef.activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.GenericCharacterMain));

            ContentAddition.AddSkillDef(passiveDef);

            var theOneDef = ScriptableObject.CreateInstance<SkillDef>();
            theOneDef.skillNameToken = "PALADIN_THEONE_NAME";
            (theOneDef as ScriptableObject).name = "PALADIN_THEONE_NAME";
            theOneDef.skillDescriptionToken = "PALADIN_THEONE_DESCRIPTION";
            theOneDef.icon = null;
            theOneDef.keywordTokens = new string[] { "KEYWORD_CURSED", "KEYWORD_BESTOWED" };
            theOneDef.baseRechargeInterval = 0f;
            theOneDef.activationStateMachineName = "Body";
            theOneDef.activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.GenericCharacterMain));

            ContentAddition.AddSkillDef(theOneDef);

            skill.skillFamily.variants[0] = new SkillFamily.Variant { skillDef = passiveDef, viewableNode = new ViewablesCatalog.Node(passiveDef.skillNameToken, false, null) };
            skill.skillFamily.variants[1] = new SkillFamily.Variant { skillDef = theOneDef, viewableNode = new ViewablesCatalog.Node(theOneDef.skillNameToken, false, null) };

            ContentAddition.AddSkillFamily(skill.skillFamily);

            LanguageAPI.Add("PALADIN_THEONE_NAME", "The One");
            LanguageAPI.Add("PALADIN_THEONE_DESCRIPTION", "<style=cDeath>Cursed</style>. While above <style=cIsHealing>90% health</style> or while having <style=cIsHealing>active barrier</style>, the Paladin is <style=cIsHealing>blessed</style>, <style=cIsDamage>empowering</style> all sword skills. Every stage, <style=cIsUtility>run a timer</style>. Once the timer runs out, become <style=cIsDamage>bestowed</style> for <style=cIsDamage>one minute</style>.");

            IL.RoR2.UI.CharacterSelectController.BuildSkillStripDisplayData += CharacterSelectController_BuildSkillStripDisplayData;
            IL.RoR2.UI.LoadoutPanelController.Rebuild += LoadoutPanelController_Rebuild;
        }

        private static void LoadoutPanelController_Rebuild(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(MoveType.After, x => x.MatchCallOrCallvirt(typeof(LoadoutPanelController.Row).GetMethod(nameof(LoadoutPanelController.Row.FromSkillSlot), (System.Reflection.BindingFlags)(-1)))))
            {
                c.EmitDelegate<System.Func<LoadoutPanelController.Row, LoadoutPanelController.Row>>((orig) =>
                {
                    var label = orig.rowPanelTransform.Find("SlotLabel") ?? orig.rowPanelTransform.Find("LabelContainer").Find("SlotLabel");
                    if (label && label.GetComponent<LanguageTextMeshController>().token == "Passive")
                    {
                        orig.rowPanelTransform.SetSiblingIndex(0);
                    }
                    return orig;
                });
            }
        }

        private static void CharacterSelectController_BuildSkillStripDisplayData(ILContext il)
        {
            ILCursor c = new(il);
            int skillIndex = -1;
            int defIndex = -1;
            var label = c.DefineLabel();
            if (c.TryGotoNext(x => x.MatchLdloc(out skillIndex), x => x.MatchLdfld(typeof(GenericSkill).GetField("hideInCharacterSelect")), x => x.MatchBrtrue(out label)) && skillIndex != (-1) && c.TryGotoNext(MoveType.After, x => x.MatchLdfld(typeof(SkillFamily.Variant).GetField("skillDef")), x => x.MatchStloc(out defIndex)))
            {
                // c.Emit(OpCodes.Ldloc, defIndex);
                // c.EmitDelegate<Func<SkillDef, bool>>((def) => def == NoneDef);
                // c.Emit(OpCodes.Brtrue, label);
                if (c.TryGotoNext(x => x.MatchCallOrCallvirt(typeof(List<StripDisplayData>).GetMethod("Add"))))
                {
                    c.Remove();
                    c.Emit(OpCodes.Ldloc, skillIndex);
                    c.EmitDelegate<Action<List<StripDisplayData>, StripDisplayData, GenericSkill>>((list, disp, ski) =>
                    {
                        if ((ski.skillFamily as ScriptableObject).name.Contains("Passive"))
                        {
                            list.Insert(0, disp);
                        }
                        else
                        {
                            list.Add(disp);
                        }
                    });
                }
            }
        }
    }
}