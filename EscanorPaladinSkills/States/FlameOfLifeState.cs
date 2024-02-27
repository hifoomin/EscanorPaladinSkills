using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EscanorPaladinSkills.States
{
    public class FlameOfLifeState : BaseState
    {
        public Transform modelTransform;
        public static float castAnimationDuration = 0.33f;
        public static float baseDuration = castAnimationDuration + 7f;
        public static float interval = 0.2f;
        public float timer;
        public bool hasAddedStuff;

        public override void OnEnter()
        {
            base.OnEnter();

            modelTransform = GetModelTransform();

            // PlayAnimation("Gesture, Override", "ChargeSpell", "Spell.playbackRate", castAnimationDuration);
            PlayAnimation("FullBody, Override", "RageEnter", "Rage.playbackRate", 1.75f);
            Util.PlaySound("PaladinCastTorpor", gameObject);
            Util.PlaySound("Play_item_use_hellfire_start", gameObject);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            timer += Time.fixedDeltaTime;
            if (fixedAge >= castAnimationDuration)
            {
                if (!hasAddedStuff)
                {
                    if (modelTransform)
                    {
                        var overlay1 = modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                        overlay1.duration = baseDuration + 0.5f;
                        overlay1.animateShaderAlpha = true;
                        overlay1.alphaCurve = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(0.07f, 1f), new Keyframe(0.9f, 1f), new Keyframe(1f, 0f));
                        overlay1.destroyComponentOnEnd = true;
                        overlay1.originalMaterial = Overlays.FlameOfLife.prefab1;
                        overlay1.AddToCharacerModel(modelTransform.GetComponent<CharacterModel>());

                        var overlay2 = modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                        overlay2.duration = baseDuration + 1.5f;
                        overlay2.animateShaderAlpha = true;
                        overlay2.alphaCurve = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(0.1f, 1f), new Keyframe(0.85f, 1f), new Keyframe(1f, 0f));
                        overlay2.destroyComponentOnEnd = true;
                        overlay2.originalMaterial = Overlays.FlameOfLife.prefab2;
                        overlay2.AddToCharacerModel(modelTransform.GetComponent<CharacterModel>());
                    }

                    if (characterBody)
                    {
                        characterBody.AddBuff(Buffs.All.healingRemoval);
                        characterBody.AddBuff(Buffs.All.enrage);
                    }
                    hasAddedStuff = true;
                }

                if (timer >= interval)
                {
                    DamageInfo info = new()
                    {
                        attacker = gameObject,
                        procCoefficient = 0,
                        damage = healthComponent.fullCombinedHealth * 0.0085714285714f,
                        crit = false,
                        position = transform.position,
                        damageColorIndex = DamageColorIndex.Fragile,
                        damageType = DamageType.BypassArmor | DamageType.BypassBlock,
                        inflictor = gameObject
                    };

                    if (NetworkServer.active)
                    {
                        healthComponent.TakeDamage(info);
                    }

                    timer = 0f;
                }
            }
            if (fixedAge < baseDuration + castAnimationDuration || !isAuthority)
            {
                return;
            }
            outer.SetNextStateToMain();
        }

        public override void OnExit()
        {
            base.OnExit();
            // PlayAnimation("Gesture, Override", "ChargeSpell", "Spell.playbackRate", castAnimationDuration);
            Util.PlaySound("Play_mage_R_end", gameObject);
            Util.PlaySound("Stop_item_use_hellfire_loop", gameObject);
            if (characterBody)
            {
                characterBody.RemoveBuff(Buffs.All.healingRemoval);
                characterBody.RemoveBuff(Buffs.All.enrage);
            }
        }
    }
}