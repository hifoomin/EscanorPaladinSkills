using EntityStates;
using RoR2;
using RoR2.Projectile;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace EscanorPaladinSkills.States
{
    public class SunfallState : BaseSkillState
    {
        public static float baseDuration = 3f;
        public static float duration;
        public bool hasFired = false;
        public bool hasPlayedSound = false;
        public Ray aimRay;

        public override void OnEnter()
        {
            base.OnEnter();

            StartAimMode(5f);

            Util.PlaySound("PaladinCloth1", gameObject);
            Util.PlayAttackSpeedSound("Play_loader_m1_swing", gameObject, 0.5f);

            PlayAnimation("FullBody, Override", "PointDown", "Emote.playbackRate", 3f);

            if (characterBody && NetworkServer.active)
            {
                characterBody.AddBuff(Buffs.All.sunfallStunDebuff);
            }
        }

        public IEnumerator Fire()
        {
            Util.PlaySound("Play_grandParent_attack3_sun_spawn", gameObject);
            AddRecoil(-4f, -4.5f, -2.75f, 2.75f);
            for (int i = 0; i < 20; i++)
            {
                yield return new WaitForSeconds(0.06f);
                aimRay = new Ray(inputBank.aimOrigin + (inputBank.aimDirection * 1f), inputBank.aimDirection);

                if (Physics.Raycast(aimRay, out var raycastInfo, 1000f, LayerIndex.CommonMasks.bullet, QueryTriggerInteraction.Ignore))
                {
                    AddRecoil(-0.5f, 0.5f, -0.75f, 0.75f);
                    var fpi = new FireProjectileInfo()
                    {
                        crit = RollCrit(),
                        damage = damageStat * (3.5f + ((attackSpeedStat - 1f) * (2f + 1f / 3f))),
                        damageTypeOverride = DamageType.Generic,
                        owner = gameObject,
                        position = raycastInfo.point + new Vector3(Main.rng.RangeFloat(-1.5f * i, 1.5f * i), 45f + Main.rng.RangeFloat(-1.1f * i, 1.1f * i), Main.rng.RangeFloat(-1.51f * i, 1.51f * i)),
                        rotation = Util.QuaternionSafeLookRotation(new Vector3(Main.rng.RangeFloat(0f, 0.02f * i), -1f, Main.rng.RangeFloat(0f, 0.021f * i))),
                        projectilePrefab = Projectiles.Sunfall.prefab,
                    };
                    if (isAuthority)
                    {
                        ProjectileManager.instance.FireProjectile(fpi);
                    }
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

            characterBody.isSprinting = false;

            if (fixedAge >= 0.5f && !hasPlayedSound)
            {
                Util.PlayAttackSpeedSound("Play_grandParent_spawn", gameObject, 0.5f);
                hasPlayedSound = true;
            }

            if (fixedAge >= 1.5f && !hasFired)
            {
                outer.StartCoroutine(Fire());

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

            PlayAnimation("FullBody, Override", "BufferEmpty");

            if (characterBody && NetworkServer.active)
            {
                characterBody.RemoveBuff(Buffs.All.sunfallStunDebuff);
            }
        }
    }
}