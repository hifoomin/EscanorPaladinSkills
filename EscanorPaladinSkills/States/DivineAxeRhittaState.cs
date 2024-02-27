using EntityStates;
using EntityStates.Merc;
using PaladinMod.Misc;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace EscanorPaladinSkills.States
{
    public class DivineAxeRhittaState : BaseSkillState, SteppedSkillDef.IStepSetter
    {
        public static float baseDuration = 1.25f;
        public static float minimumSprintDuration = 0.4f;
        public SphereSearch sphereSearch;
        public List<HurtBox> hurtBoxes;
        public PaladinSwordController swordController;
        public bool hasFired = false;
        public OverlapAttack attack;
        public HitBoxGroup hitBoxGroup;
        public Transform modelTransform;
        public GameObject impact = VFX.DivineAxeRhitta.prefab;

        public int swingIndex;
        public bool inCombo;
        public Animator animator;
        public bool inHitPause;
        public bool hasHopped;
        public HitStopCachedState hitStopCachedState;
        public float hitPauseTimer;
        public Vector3 storedVelocity;

        public override void OnEnter()
        {
            base.OnEnter();
            swordController = GetComponent<PaladinSwordController>();
            hitBoxGroup = null;
            modelTransform = GetModelTransform();
            animator = GetModelAnimator();
            StartAimMode(2.5f);
            if (modelTransform)
            {
                hitBoxGroup = Array.Find(modelTransform.GetComponents<HitBoxGroup>(), (x) => x.groupName == "SayGex");
            }
            /*
            var swingTrail = swordController.swingEffect.transform.GetChild(0);
            swingTrail.localScale = new Vector3(3f, 1.5249f, 3f);
            var swingTrailDistortion = swingTrail.GetChild(0);
            swingTrailDistortion.localScale = new Vector3(3f, 1.3392f, 3f);
            */
            Util.PlaySound("PaladinCloth2", gameObject);

            var swing = "Slash" + (1 + swingIndex);
            if (inCombo)
            {
                var standingStill = !animator.GetBool("isMoving") && animator.GetBool("isGrounded");

                if (standingStill)
                {
                    PlayCrossfade("FullBody, Override", "SlashCombo1", "Slash.playbackRate", baseDuration, 0.05f);
                }
                PlayCrossfade("Gesture, Override", "SlashCombo1", "Slash.playbackRate", baseDuration, 0.05f);
            }
            else
            {
                var movingGrounded = !animator.GetBool("isMoving") && animator.GetBool("isGrounded");
                if (movingGrounded)
                {
                    PlayCrossfade("FullBody, Override", swing, "Slash.playbackRate", baseDuration, 0.05f);
                }
                PlayCrossfade("Gesture, Override", swing, "Slash.playbackRate", baseDuration, 0.05f);
            }
        }

        public void Fire()
        {
            AddRecoil(-3f, -6f, -1.5f, 1.5f);
            Util.PlayAttackSpeedSound("Play_loader_m1_swing", gameObject, 0.5f);
            Util.PlaySound("Play_item_proc_armorReduction_hit", gameObject);
            swordController.attacking = true;
            swordController.PlaySwingSound();
            if (isAuthority)
            {
                string swing;
                if (swingIndex == 0)
                {
                    swing = "SwingRight";
                }
                else
                {
                    swing = "SwingLeft";
                }

                // EffectManager.SimpleMuzzleFlash(swordController.swingEffect, base.gameObject, swing, true);
                EffectManager.SimpleMuzzleFlash(VFX.DivineAxeRhittaJank.swingPrefab, base.gameObject, swing, true);
            }

            attack = new()
            {
                attacker = gameObject,
                inflictor = gameObject,
                attackerFiltering = AttackerFiltering.NeverHitSelf,
                pushAwayForce = 1500f,
                damage = damageStat * (7f + ((attackSpeedStat - 1f) * (4f + 2f / 3f))),
                // damageType = DamageType.Stun1s,
                procCoefficient = 1f,
                isCrit = RollCrit(),
                procChainMask = default,
                teamIndex = TeamComponent.GetObjectTeam(gameObject),
                forceVector = Vector3.zero,
                hitBoxGroup = hitBoxGroup,
                hitEffectPrefab = impact
            };
            if (isAuthority)
            {
                if (attack.Fire(null))
                {
                    OnHit();
                }
            }
        }

        public void OnHit()
        {
            Util.PlaySound("Play_item_proc_armorReduction_shatter", gameObject);
            if (!hasHopped)
            {
                var airborne = characterMotor && !characterMotor.isGrounded;
                if (airborne)
                {
                    SmallHop(characterMotor, 6f);
                }

                if (skillLocator.utility.skillDef.skillNameToken == "PALADIN_UTILITY_DASH_NAME") skillLocator.utility.RunRecharge(1f);

                hasHopped = true;
            }
            if (!inHitPause)
            {
                if (characterMotor.velocity != Vector3.zero)
                {
                    storedVelocity = characterMotor.velocity;
                }
                hitStopCachedState = CreateHitStopCachedState(characterMotor, animator, "Slash.playbackRate");
                hitPauseTimer = 2f * GroundLight.hitPauseDuration;
                inHitPause = true;
            }
        }

        public void GroundNearby()
        {
            if (characterBody)
            {
                hurtBoxes = new();
                sphereSearch = new()
                {
                    origin = characterBody.corePosition,
                    mask = LayerIndex.entityPrecise.mask,
                    radius = 24f,
                };
                sphereSearch.RefreshCandidates();
                sphereSearch.FilterCandidatesByHurtBoxTeam(TeamMask.GetUnprotectedTeams(teamComponent.teamIndex));
                sphereSearch.FilterCandidatesByDistinctHurtBoxEntities();
                sphereSearch.OrderCandidatesByDistance();
                sphereSearch.GetHurtBoxes(hurtBoxes);
                sphereSearch.ClearCandidates();
                if (hurtBoxes.Count > 0)
                {
                    Util.PlaySound("Play_item_proc_fireRingTornado_end", gameObject);
                }
                for (int i = 0; i < hurtBoxes.Count; i++)
                {
                    var hurtBox = hurtBoxes[i];
                    var hc = hurtBox.healthComponent;
                    var body = hc.body;
                    var motor = body.GetComponent<RigidbodyMotor>();
                    var rb = body.rigidbody;
                    var mass = 1f;
                    if (motor) mass = motor.mass;
                    else if (rb) mass = rb.mass;
                    if (mass < 100) mass = 100;

                    DamageInfo info = new()
                    {
                        attacker = gameObject,
                        canRejectForce = false,
                        crit = false,
                        damage = 0,
                        damageType = DamageType.Silent,
                        procCoefficient = 0,
                        force = new Vector3(0f, -25f * mass, 0f),
                        inflictor = gameObject,
                    };
                    if (NetworkServer.active)
                    {
                        hc.TakeDamage(info);
                    }

                    var effectData = new EffectData { origin = body.corePosition, rotation = Quaternion.identity, scale = body.radius * 1.5f };
                    effectData.SetNetworkedObjectReference(body.gameObject);
                    EffectManager.SpawnEffect(VFX.DivineAxeRhitta.groundPrefab, effectData, true);
                }
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            hitPauseTimer -= Time.fixedDeltaTime;
            if (hitPauseTimer <= 0f && inHitPause)
            {
                ConsumeHitStopCachedState(hitStopCachedState, characterMotor, animator);
                inHitPause = false;
                if (storedVelocity != Vector3.zero)
                {
                    characterMotor.velocity = storedVelocity;
                }
            }

            if (fixedAge <= minimumSprintDuration)
            {
                characterBody.isSprinting = false;
            }
            if (fixedAge >= 0.25f && !hasFired)
            {
                Fire();
                if (characterBody)
                {
                    if (swordController && swordController.swordActive)
                    {
                        GroundNearby();
                    }
                }
                hasFired = true;
            }

            if (fixedAge < baseDuration || !isAuthority)
            {
                return;
            }

            outer.SetNextStateToMain();
        }

        public override void OnExit()
        {
            base.OnExit();
            if (inHitPause)
            {
                ConsumeHitStopCachedState(hitStopCachedState, characterMotor, animator);
                inHitPause = false;
            }
            if (swordController)
            {
                swordController.attacking = false;
            }
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write((byte)swingIndex);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            swingIndex = reader.ReadByte();
        }

        public void SetStep(int i)
        {
            swingIndex = i;
            if (swingIndex > 1)
            {
                ((SteppedSkillDef.InstanceData)activatorSkillSlot.skillInstanceData).step = 0;
                swingIndex = 0;
                inCombo = true;
            }
        }
    }
}