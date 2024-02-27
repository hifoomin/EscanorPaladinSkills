using EntityStates;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using Rewired.Data.Mapping;
using RoR2;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EscanorPaladinSkills.Buffs
{
    public static class All
    {
        public static BuffDef healingRemoval;
        public static BuffDef enrage;
        public static BuffDef slow;
        public static BuffDef theOne;
        public static BuffDef curse;

        public static void Init()
        {
            healingRemoval = ScriptableObject.CreateInstance<BuffDef>();
            healingRemoval.isDebuff = false;
            healingRemoval.isCooldown = false;
            healingRemoval.canStack = false;
            healingRemoval.buffColor = new Color32(246, 119, 32, 255);
            healingRemoval.iconSprite = Main.escanor.LoadAsset<Sprite>("texBuffFucked.png");
            healingRemoval.isHidden = false;
            healingRemoval.name = "Flame of Life Healing Removal";

            ContentAddition.AddBuffDef(healingRemoval);

            enrage = ScriptableObject.CreateInstance<BuffDef>();
            enrage.isDebuff = false;
            enrage.isCooldown = false;
            enrage.canStack = false;
            enrage.buffColor = new Color32(246, 119, 32, 255);
            enrage.iconSprite = Main.escanor.LoadAsset<Sprite>("texBuffEnraged.png");
            enrage.isHidden = false;
            enrage.name = "Flame of Life Enrage";

            ContentAddition.AddBuffDef(enrage);

            slow = ScriptableObject.CreateInstance<BuffDef>();
            slow.isDebuff = false;
            slow.isCooldown = false;
            slow.canStack = false;
            slow.buffColor = new Color32(246, 119, 32, 255);
            slow.iconSprite = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/Common/bdSlow50.asset").WaitForCompletion().iconSprite;
            slow.isHidden = false;
            slow.name = "Sunfall Slow";

            ContentAddition.AddBuffDef(slow);

            theOne = ScriptableObject.CreateInstance<BuffDef>();
            theOne.isDebuff = false;
            theOne.isCooldown = false;
            theOne.canStack = false;
            theOne.buffColor = new Color32(255, 0, 0, 255);
            theOne.iconSprite = Main.escanor.LoadAsset<Sprite>("texBuffEnraged.png");
            theOne.isHidden = false;
            theOne.name = "The One Blessing";

            ContentAddition.AddBuffDef(theOne);

            curse = ScriptableObject.CreateInstance<BuffDef>();
            curse.isDebuff = false;
            curse.isCooldown = false;
            curse.canStack = false;
            curse.buffColor = Color.gray;
            curse.iconSprite = Main.escanor.LoadAsset<Sprite>("texBuffFucked.png");
            curse.isHidden = false;
            curse.name = "The One Curse";

            ContentAddition.AddBuffDef(curse);

            LanguageAPI.Add("KEYWORD_FLEETING", "<style=cKeywordName>Fleeting</style><style=cSub><style=cIsDamage>Attack speed</style> increases <style=cIsDamage>skill damage</style> instead, at <style=cIsDamage66%</style> effectiveness.</style>");
            LanguageAPI.Add("KEYWORD_CURSED", "<style=cKeywordName>Cursed</style><style=cSub>While not <style=cIsDamage>The One</style>, <style=cDeath>reduce all armor</style> by <style=cDeath>50%</style>, <style=cDeath>maximum health</style> by <style=cDeath>20%</style>, and <style=cDeath>all healing</style> by <style=cDeath>33%</style>.</style>");
            LanguageAPI.Add("KEYWORD_BESTOWED", "<style=cKeywordName>Bestowed</style><style=cSub><style=cIsHealing>Increase armor</style> by <style=cIsHealing>30</style>, <style=cIsHealing>health regeneration</style> by <style=cIsHealing>100%</style>, and <style=cIsDamage>upgrade skills</style> in unique ways.</style>");

            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            GlobalEventManager.onServerDamageDealt += GlobalEventManager_onServerDamageDealt;
            On.RoR2.HealthComponent.Heal += HealthComponent_Heal;
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
            On.RoR2.HealthComponent.FixedUpdate += HealthComponent_FixedUpdate;
            On.RoR2.CharacterBody.FixedUpdate += CharacterBody_FixedUpdate;
            IL.EntityStates.GenericCharacterMain.ProcessJump += GenericCharacterMain_ProcessJump;
        }

        private static void CharacterBody_FixedUpdate(On.RoR2.CharacterBody.orig_FixedUpdate orig, CharacterBody self)
        {
            if (self.HasBuff(enrage))
            {
                self.healthComponent.barrier = Mathf.Max(Mathf.Epsilon, self.healthComponent.barrier);
            }

            orig(self);

            if (self.HasBuff(enrage))
            {
                self.healthComponent.barrier = Mathf.Max(Mathf.Epsilon, self.healthComponent.barrier);
            }
        }

        private static void HealthComponent_FixedUpdate(On.RoR2.HealthComponent.orig_FixedUpdate orig, HealthComponent self)
        {
            orig(self);
        }

        private static void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);

            if (self.HasBuff(curse))
            {
                self.armor *= 0.5f;
            }
        }

        private static void GenericCharacterMain_ProcessJump(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<GenericCharacterMain>("jumpInputReceived")))
            {
                c.Index += 2;
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<bool, GenericCharacterMain, bool>>((orig, self) =>
                {
                    var body = self.characterBody;
                    if (body && body.HasBuff(slow))
                    {
                        return false;
                    }

                    return orig;
                });
            }
            else
            {
                Main.logger.LogError("Failed to apply Jump Hook");
            }
        }

        private static float HealthComponent_Heal(On.RoR2.HealthComponent.orig_Heal orig, HealthComponent self, float amount, ProcChainMask procChainMask, bool nonRegen)
        {
            if (self.body.HasBuff(healingRemoval))
            {
                amount = 0;
            }
            if (self.body.HasBuff(curse))
            {
                amount *= 0.66f;
            }
            return orig(self, amount, procChainMask, nonRegen);
        }

        private static void GlobalEventManager_onServerDamageDealt(DamageReport report)
        {
            var attackerBody = report.attackerBody;
            if (!attackerBody)
            {
                return;
            }

            if (attackerBody.HasBuff(enrage))
            {
                report.damageInfo.damageType |= DamageType.IgniteOnHit;
            }
        }

        private static void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(enrage))
            {
                args.damageMultAdd += 0.75f;
                args.moveSpeedMultAdd += 0.2f;
            }
            if (sender.HasBuff(slow))
            {
                args.moveSpeedRootCount += 1; // 1 - (1/(1+'1')) = 0.5
            }
            if (sender.HasBuff(theOne))
            {
                args.armorAdd += 30f;
                args.regenMultAdd += 1f;
            }
            if (sender.HasBuff(curse))
            {
                args.baseCurseAdd += 0.2f;
            }
        }
    }
}