using EntityStates;
using EscanorPaladinSkills.Projectiles;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EscanorPaladinSkills.States
{
    public class CruelSunState : BaseState
    {
        public static float baseDuration = 0.75f;
        public float duration;
        public static string sound;
        public Ray aimRay;
        public bool hasFired = false;
        public bool hasPlayedSound = false;
        public static float barrierGain = 0.06f;

        public override void OnEnter()
        {
            base.OnEnter();

            duration = baseDuration / attackSpeedStat;
            if (characterBody)
            {
                characterBody.SetAimTimer(1f);
            }

            // PlayAnimation("Gesture, Override", "ChargeSpell", "Spell.playbackRate", duration);
            // PlayAnimation("Gesture, Override", "CastSpell", "Spell.playbackRate", duration);
            PlayAnimation("Gesture, Override", "ChannelSpell", "Spell.playbackRate", duration * 0.5f);
            // PlayAnimation("Gesture, Override", "ThrowSpell", "ChargeSpell.playbackRate", duration);
            Util.PlaySound(PaladinMod.Modules.Sounds.Cloth1, gameObject);
            Util.PlaySound(PaladinMod.Modules.Sounds.Cloth1, gameObject);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            /*
            if (fixedAge >= duration / 2f && !hasPlayedSound)
            {
                Util.PlaySound(PaladinMod.Modules.Sounds.MenuSound, gameObject);
                hasPlayedSound = true;
            }
            */

            if (fixedAge < duration || !isAuthority)
            {
                return;
            }

            if (!hasFired)
            {
                aimRay = GetAimRay();
                FireProjectile();
                hasFired = true;
            }

            outer.SetNextStateToMain();
        }

        public void FireProjectile()
        {
            AddRecoil(7f, 7f, -2.5f, 2.5f);
            PlayAnimation("Gesture, Override", "ThrowSpell", "Spell.playbackRate", duration * 0.5f);
            var fpi = new FireProjectileInfo()
            {
                crit = RollCrit(),
                damage = damageStat * 7f,
                damageTypeOverride = DamageType.IgniteOnHit,
                owner = gameObject,
                position = characterBody.corePosition + new Vector3(0f, 4f, 0f),
                projectilePrefab = CruelSun.prefab,
                rotation = Util.QuaternionSafeLookRotation(aimRay.direction)
            };
            ProjectileManager.instance.FireProjectile(fpi);
            Util.PlaySound("Play_grandParent_attack3_sun_spawn", gameObject);
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}