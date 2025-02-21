﻿using EntityStates;
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
            PlayAnimation("Gesture, Override", "LimitBreak", "Rage.playbackRate", 1f);
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
            // timer += Time.fixedDeltaTime;
            if (fixedAge >= castAnimationDuration)
            {
                if (!hasAddedStuff)
                {
                    if (modelTransform)
                    {
                        var overlay1 = TemporaryOverlayManager.AddOverlay(modelTransform.gameObject);
                        overlay1.duration = baseDuration + 0.5f;
                        overlay1.animateShaderAlpha = true;
                        overlay1.alphaCurve = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(0.07f, 1f), new Keyframe(0.9f, 1f), new Keyframe(1f, 0f));
                        overlay1.destroyComponentOnEnd = true;
                        overlay1.originalMaterial = Overlays.FlameOfLife.prefab1;
                        overlay1.inspectorCharacterModel = modelTransform.GetComponent<CharacterModel>();

                        var overlay2 = TemporaryOverlayManager.AddOverlay(modelTransform.gameObject);
                        overlay2.duration = baseDuration + 1.5f;
                        overlay2.animateShaderAlpha = true;
                        overlay2.alphaCurve = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(0.1f, 1f), new Keyframe(0.85f, 1f), new Keyframe(1f, 0f));
                        overlay2.destroyComponentOnEnd = true;
                        overlay2.originalMaterial = Overlays.FlameOfLife.prefab2;
                        overlay2.inspectorCharacterModel = modelTransform.GetComponent<CharacterModel>();

                        var light = modelTransform.GetComponent<Light>();
                        if (light)
                        {
                            light.enabled = true;
                        }

                        var lightIntensityCurve = modelTransform.GetComponent<LightIntensityCurve>();
                        if (lightIntensityCurve)
                        {
                            lightIntensityCurve.enabled = true;
                        }
                    }

                    if (characterBody && NetworkServer.active)
                    {
                        if (characterBody.HasBuff(Buffs.All.flameOfLifeHealingDebuff))
                            characterBody.RemoveBuff(Buffs.All.flameOfLifeHealingDebuff);
                        characterBody.AddBuff(Buffs.All.flameOfLifeHealingDebuff);
                        characterBody.AddBuff(Buffs.All.flameOfLifeBuff);
                    }
                    hasAddedStuff = true;
                }
                /*
                if (timer >= interval)
                {
                    DamageInfo info = new()
                    {
                        attacker = null,
                        procCoefficient = 0,
                        damage = healthComponent.fullCombinedHealth * 0.007f,
                        crit = false,
                        position = transform.position,
                        damageColorIndex = DamageColorIndex.Fragile,
                        damageType = DamageType.BypassArmor | DamageType.BypassBlock | DamageType.NonLethal,
                        inflictor = null
                    };

                    if (NetworkServer.active)
                    {
                        healthComponent.TakeDamage(info);
                    }

                    timer = 0f;
                }
                */
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
            if (characterBody && NetworkServer.active)
            {
                characterBody.RemoveBuff(Buffs.All.flameOfLifeHealingDebuff);
                characterBody.RemoveBuff(Buffs.All.flameOfLifeBuff);
                characterBody.AddTimedBuff(Buffs.All.flameOfLifeHealingDebuff, 2f);
            }

            if (modelTransform)
            {
                var light = modelTransform.GetComponent<Light>();
                if (light)
                {
                    light.enabled = false;
                }

                var lightIntensityCurve = modelTransform.GetComponent<LightIntensityCurve>();
                if (lightIntensityCurve)
                {
                    lightIntensityCurve.enabled = false;
                }
            }
        }
    }
}