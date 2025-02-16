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
using UnityEngine.Networking;
using UnityEngine;
using System.Collections;

namespace EscanorPaladinSkills.States.Upgrades.SpinningSlash
{
    public class SpinningSlashGroundedUpgradedState : BaseSkillState
    {
        public static float damageCoefficient = StaticValues.spinSlashDamageCoefficient;
        public float baseDuration = 1.5f;
        public static float attackRecoil = 3f;

        private float duration;
        private bool hasFired;
        private float hitPauseTimer;
        private OverlapAttack attack;
        private bool inHitPause;
        private float stopwatch;
        private Animator animator;
        private BaseState.HitStopCachedState hitStopCachedState;
        private PaladinSwordController swordController;
        private Vector3 storedVelocity;
        //private int startingJumpCount;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            hasFired = false;
            animator = GetModelAnimator();
            swordController = GetComponent<PaladinSwordController>();
            StartAimMode(0.5f + duration, false);
            characterBody.isSprinting = false;
            //this.startingJumpCount = base.characterMotor.jumpCount;
            //base.characterMotor.jumpCount = base.characterBody.maxJumpCount;

            if (swordController) swordController.attacking = true;

            HitBoxGroup hitBoxGroup = null;
            Transform modelTransform = GetModelTransform();

            if (NetworkServer.active) characterBody.AddBuff(RoR2Content.Buffs.Slow50);

            string hitboxString = "SpinSlash";
            if (swordController && swordController.swordActive) hitboxString = "SpinSlashLarge";

            if (modelTransform)
            {
                hitBoxGroup = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == hitboxString);
            }

            PlayAnimation("FullBody, Override", "GroundSweep", "Whirlwind.playbackRate", duration * 1.1f);
            Util.PlaySound(PaladinMod.Modules.Sounds.Cloth3, gameObject);

            attack = new OverlapAttack
            {
                damageType = DamageType.Stun1s,
                attacker = gameObject,
                inflictor = gameObject,
                teamIndex = GetTeam(),
                damage = damageCoefficient * damageStat,
                procCoefficient = 1,
                hitEffectPrefab = swordController.hitEffect,
                forceVector = Vector3.up * 1600f,
                pushAwayForce = -1500f,
                hitBoxGroup = hitBoxGroup,
                isCrit = RollCrit(),
                impactSound = PaladinMod.Modules.Assets.swordHitSoundEventM.index
            };
            if (swordController.isBlunt) attack.impactSound = PaladinMod.Modules.Assets.batHitSoundEventM.index;
        }

        public override void OnExit()
        {
            base.OnExit();
            //base.characterMotor.jumpCount = this.startingJumpCount;
            if (NetworkServer.active) characterBody.RemoveBuff(RoR2Content.Buffs.Slow50);

            if (swordController) swordController.attacking = false;
        }

        public IEnumerator FireAttack()
        {
            if (!hasFired)
            {
                hasFired = true;
                for (int i = 0; i < 2; i++)
                {
                    swordController.PlaySwingSound();

                    if (isAuthority)
                    {
                        AddRecoil(-1f * attackRecoil, -2f * attackRecoil, -0.5f * attackRecoil, 0.5f * attackRecoil);
                        if (swordController.swordActive) EffectManager.SimpleMuzzleFlash(swordController.empoweredSpinSlashEffect, gameObject, "SwingCenter", true);
                        else EffectManager.SimpleMuzzleFlash(swordController.spinSlashEffect, gameObject, "SwingCenter", true);

                        Ray aimRay = GetAimRay();

                        Vector3 forwardDirection = characterDirection.forward;
                        if (PaladinPlugin.IsLocalVRPlayer(characterBody))
                        {
                            forwardDirection = Camera.main.transform.forward;
                            forwardDirection.y = 0;
                            forwardDirection = forwardDirection.normalized;
                        }

                        characterMotor.velocity += forwardDirection * 25f;
                    }
                    yield return new WaitForSeconds(duration * 0.3f);
                }
            }

            for (int i = 0; i < 2; i++)
            {
                if (attack.Fire())
                {
                    if (!inHitPause)
                    {
                        if (characterMotor.velocity != Vector3.zero) storedVelocity = characterMotor.velocity;
                        hitStopCachedState = CreateHitStopCachedState(characterMotor, animator, "Whirlwind.playbackRate");
                        hitPauseTimer = (4f * EntityStates.Merc.GroundLight.hitPauseDuration) / attackSpeedStat;
                        inHitPause = true;
                    }
                }
                yield return new WaitForSeconds(duration * 0.3f);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            hitPauseTimer -= Time.fixedDeltaTime;

            if (hitPauseTimer <= 0f && inHitPause)
            {
                ConsumeHitStopCachedState(hitStopCachedState, characterMotor, animator);
                inHitPause = false;
                if (storedVelocity != Vector3.zero) characterMotor.velocity = storedVelocity;
            }

            if (!inHitPause)
            {
                stopwatch += Time.fixedDeltaTime;
            }
            else
            {
                if (characterMotor) characterMotor.velocity = Vector3.zero;
                if (animator) animator.SetFloat("Whirlwind.playbackRate", 0f);
            }

            if (characterMotor && stopwatch < duration * 0.6f)
            {
                characterMotor.moveDirection /= 2f;
            }

            bool skillStarted = stopwatch >= duration * 0.45f;
            bool skillEnded = stopwatch > duration * 0.7f;

            if ((skillStarted && !skillEnded) || (skillStarted && skillEnded && !hasFired))
            {
                outer.StartCoroutine(FireAttack());
            }

            if (stopwatch >= duration * 0.6f)
            {
                if (isAuthority && inputBank.skill2.down)
                {
                    if (skillLocator.secondary.stock > 0)
                    {
                        EntityState nextState = new SpinningSlashGroundedAltUpgradedState();
                        outer.SetNextState(nextState);
                        return;
                    }
                }
            }

            if (stopwatch >= duration && isAuthority)
            {
                outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (stopwatch >= duration * 0.41f) return InterruptPriority.PrioritySkill;
            else return InterruptPriority.Frozen;
        }
    }
}
*/