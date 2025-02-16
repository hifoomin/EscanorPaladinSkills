/*
using EntityStates;
using PaladinMod.Misc;
using PaladinMod.States;
using PaladinMod;
using RoR2;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using RoR2.Projectile;

namespace EscanorPaladinSkills.States.Upgrades.SpinningSlash
{
    public class SpinningSlashAirborneAltUpgradedState : BaseSkillState
    {
        public static float damageCoefficient = StaticValues.spinSlashDamageCoefficient;
        public static float leapDuration = 0.5f;
        public static float dropVelocity = 60f;
        public static float hopVelocity = 30f;

        private float duration;
        private bool hasFired;
        private bool hasLanded;
        private float hitPauseTimer;
        private OverlapAttack attack;
        private bool inHitPause;
        private float stopwatch;
        private Animator animator;
        private BaseState.HitStopCachedState hitStopCachedState;
        private PaladinSwordController swordController;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = leapDuration / (0.5f + (0.5f * attackSpeedStat));
            hasFired = false;
            hasLanded = false;
            animator = GetModelAnimator();
            swordController = GetComponent<PaladinSwordController>();

            skillLocator.secondary.DeductStock(1);

            Vector3 direction = GetAimRay().direction;

            if (PaladinPlugin.IsLocalVRPlayer(characterBody))
            {
                direction = Camera.main.transform.forward;
            }

            if (isAuthority)
            {
                characterBody.isSprinting = true;

                characterMotor.velocity *= 0.1f;
                SmallHop(characterMotor, hopVelocity);
            }

            characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;

            HitBoxGroup hitBoxGroup = null;
            Transform modelTransform = GetModelTransform();

            string hitboxString = "LeapStrike";

            if (modelTransform)
            {
                hitBoxGroup = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == hitboxString);
            }

            PlayAnimation("FullBody, Override", "LeapSlam2", "Whirlwind.playbackRate", duration * 1.5f);
            Util.PlaySound(PaladinMod.Modules.Sounds.Lunge, gameObject);
            Util.PlaySound(PaladinMod.Modules.Sounds.Cloth2, gameObject);

            swordController.airSlamStacks++;

            float dmg = damageCoefficient;

            attack = new OverlapAttack
            {
                damageType = DamageType.Stun1s,
                attacker = gameObject,
                inflictor = gameObject,
                teamIndex = GetTeam(),
                damage = ((0.5f + (0.5f * swordController.airSlamStacks)) * dmg) * damageStat,
                procCoefficient = 1,
                hitEffectPrefab = swordController.hitEffect,
                forceVector = -Vector3.up * 6000f,
                pushAwayForce = 500f,
                hitBoxGroup = hitBoxGroup,
                isCrit = RollCrit(),
                impactSound = PaladinMod.Modules.Assets.swordHitSoundEventL.index
            };
            if (swordController.isBlunt) attack.impactSound = PaladinMod.Modules.Assets.batHitSoundEventL.index;
        }

        public override void OnExit()
        {
            base.OnExit();

            base.PlayAnimation("FullBody, Override", "BufferEmpty");
            characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
        }

        public void FireAttack()
        {
            if (!hasFired)
            {
                hasFired = true;
                swordController.PlaySwingSound();

                if (isAuthority)
                {
                    AddRecoil(-1f * GroundSweep.attackRecoil, -2f * GroundSweep.attackRecoil, -0.5f * GroundSweep.attackRecoil, 0.5f * GroundSweep.attackRecoil);
                    EffectManager.SimpleMuzzleFlash(swordController.swingEffect, gameObject, "SwingDown", true);

                    characterMotor.velocity *= 0.1f;
                    characterMotor.velocity += Vector3.up * -AirSlamAlt.dropVelocity;
                }
            }

            if (isAuthority)
            {
                Ray aimRay = GetAimRay();

                if (attack.Fire())
                {
                    swordController.airSlamStacks = 1;

                    if (!inHitPause)
                    {
                        hitStopCachedState = CreateHitStopCachedState(characterMotor, animator, "Whirlwind.playbackRate");
                        hitPauseTimer = (4f * EntityStates.Merc.GroundLight.hitPauseDuration) / attackSpeedStat;
                        inHitPause = true;
                    }
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            StartAimMode(0.5f, false);
            stopwatch += Time.fixedDeltaTime;

            if (stopwatch >= duration)
            {
                FireAttack();

                if (isAuthority && inputBank.skill2.down)
                {
                    if (skillLocator.secondary.stock > 0)
                    {
                        EntityState nextState = new AirSlamAlt();
                        outer.SetNextState(nextState);
                        return;
                    }
                }
            }

            if (stopwatch >= duration && isAuthority && characterMotor.isGrounded)
            {
                GroundImpact();
                outer.SetNextStateToMain();
                return;
            }
        }

        private void FireShockwave()
        {
            Transform shockwaveTransform = FindModelChild("SwingCenter");
            Vector3 shockwavePosition = characterBody.footPosition;
            Vector3 forward = characterDirection.forward;

            ProjectileManager.instance.FireProjectile(PaladinMod.Modules.Projectiles.shockwave, shockwavePosition, Util.QuaternionSafeLookRotation(forward), gameObject, characterBody.damage * StaticValues.beamDamageCoefficient, EntityStates.BrotherMonster.WeaponSlam.waveProjectileForce, RollCrit(), DamageColorIndex.Default, null, -1f);
        }

        private void GroundImpact()
        {
            if (!hasLanded)
            {
                hasLanded = true;

                if (swordController && swordController.swordActive)
                {
                    FireShockwave();
                }
                swordController.airSlamStacks = 1;

                Util.PlaySound(PaladinMod.Modules.Sounds.GroundImpact, gameObject);
                Util.PlaySound(PaladinMod.Modules.Sounds.LeapSlam, gameObject);

                EffectData effectData = new()
                {
                    origin = characterBody.footPosition,
                    scale = 2f
                };

                EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/ImpactEffects/ParentSlamEffect"), effectData, true);
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
*/