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
        public static BuffDef flameOfLifeHealingDebuff;
        public static BuffDef flameOfLifeBuff;
        public static BuffDef sunfallStunDebuff;
        public static BuffDef theOneBuff;
        public static BuffDef theOneCurseDebuff;

        public static void Init()
        {
            flameOfLifeHealingDebuff = ScriptableObject.CreateInstance<BuffDef>();
            flameOfLifeHealingDebuff.isDebuff = false;
            flameOfLifeHealingDebuff.isCooldown = false;
            flameOfLifeHealingDebuff.canStack = false;
            flameOfLifeHealingDebuff.buffColor = new Color32(246, 119, 32, 255);
            flameOfLifeHealingDebuff.iconSprite = Main.escanor.LoadAsset<Sprite>("texBuffFucked.png");
            flameOfLifeHealingDebuff.isHidden = false;
            flameOfLifeHealingDebuff.name = "Flame of Life Healing Removal";

            ContentAddition.AddBuffDef(flameOfLifeHealingDebuff);

            flameOfLifeBuff = ScriptableObject.CreateInstance<BuffDef>();
            flameOfLifeBuff.isDebuff = false;
            flameOfLifeBuff.isCooldown = false;
            flameOfLifeBuff.canStack = false;
            flameOfLifeBuff.buffColor = new Color32(246, 119, 32, 255);
            flameOfLifeBuff.iconSprite = Main.escanor.LoadAsset<Sprite>("texBuffEnraged.png");
            flameOfLifeBuff.isHidden = false;
            flameOfLifeBuff.name = "Flame of Life Enrage";

            ContentAddition.AddBuffDef(flameOfLifeBuff);

            sunfallStunDebuff = ScriptableObject.CreateInstance<BuffDef>();
            sunfallStunDebuff.isDebuff = false;
            sunfallStunDebuff.isCooldown = false;
            sunfallStunDebuff.canStack = false;
            sunfallStunDebuff.buffColor = new Color32(246, 119, 32, 255);
            sunfallStunDebuff.iconSprite = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/Common/bdSlow50.asset").WaitForCompletion().iconSprite;
            sunfallStunDebuff.isHidden = false;
            sunfallStunDebuff.name = "Sunfall Slow";

            ContentAddition.AddBuffDef(sunfallStunDebuff);

            theOneBuff = ScriptableObject.CreateInstance<BuffDef>();
            theOneBuff.isDebuff = false;
            theOneBuff.isCooldown = false;
            theOneBuff.canStack = false;
            theOneBuff.buffColor = new Color32(255, 0, 0, 255);
            theOneBuff.iconSprite = Main.escanor.LoadAsset<Sprite>("texBuffEnraged.png");
            theOneBuff.isHidden = false;
            theOneBuff.name = "The One Blessing";

            ContentAddition.AddBuffDef(theOneBuff);

            theOneCurseDebuff = ScriptableObject.CreateInstance<BuffDef>();
            theOneCurseDebuff.isDebuff = false;
            theOneCurseDebuff.isCooldown = false;
            theOneCurseDebuff.canStack = false;
            theOneCurseDebuff.buffColor = Color.gray;
            theOneCurseDebuff.iconSprite = Main.escanor.LoadAsset<Sprite>("texBuffFucked.png");
            theOneCurseDebuff.isHidden = false;
            theOneCurseDebuff.name = "The One Curse";

            ContentAddition.AddBuffDef(theOneCurseDebuff);

            LanguageAPI.Add("KEYWORD_FLEETING", "<style=cKeywordName>Fleeting</style><style=cSub><style=cIsDamage>Attack speed</style> increases <style=cIsDamage>skill damage</style> instead, at <style=cIsDamage>66%</style> effectiveness.</style>");
            LanguageAPI.Add("KEYWORD_CURSED", "<style=cKeywordName>Cursed</style><style=cSub>While not <style=cIsDamage>The One</style>, <style=cDeath>reduce all armor</style> by <style=cDeath>40%</style>, <style=cDeath>maximum health</style> by <style=cDeath>20%</style>, and <style=cDeath>all healing</style> by <style=cDeath>25%</style>.</style>");
            LanguageAPI.Add("KEYWORD_THEONE", "<style=cKeywordName>The One</style><style=cSub><style=cIsHealing>Increase armor</style> by <style=cIsHealing>30</style>, <style=cIsHealing>health regeneration</style> by <style=cIsHealing>100%</style>, and <style=cIsDamage>upgrade skills</style> in unique ways.</style>");

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
            if (self.HasBuff(flameOfLifeBuff))
            {
                self.healthComponent.barrier = Mathf.Max(Mathf.Epsilon, self.healthComponent.barrier);
            }

            orig(self);

            if (self.HasBuff(flameOfLifeBuff))
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

            if (self.HasBuff(theOneCurseDebuff))
            {
                self.armor *= 0.6f;
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
                    if (body && body.HasBuff(sunfallStunDebuff))
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
            if (self.body.HasBuff(flameOfLifeHealingDebuff))
            {
                amount = 0;
            }
            if (self.body.HasBuff(theOneCurseDebuff))
            {
                amount *= 0.75f;
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

            if (attackerBody.HasBuff(flameOfLifeBuff))
            {
                report.damageInfo.damageType |= DamageType.IgniteOnHit;
            }
        }

        private static void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(flameOfLifeBuff))
            {
                args.damageMultAdd += 0.6f;
                args.moveSpeedMultAdd += 0.25f;
            }
            if (sender.HasBuff(sunfallStunDebuff))
            {
                args.moveSpeedRootCount += 1; // 1 - (1/(1+'1')) = 0.5
            }
            if (sender.HasBuff(theOneBuff))
            {
                args.armorAdd += 30f;
                args.regenMultAdd += 1f;
            }
            if (sender.HasBuff(theOneCurseDebuff))
            {
                args.baseCurseAdd += 0.2f;
            }
        }
    }
}