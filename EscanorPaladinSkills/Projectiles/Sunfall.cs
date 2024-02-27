using EscanorPaladinSkills.States;
using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EscanorPaladinSkills.Projectiles
{
    public static class Sunfall
    {
        public static GameObject prefab;
        public static DamageAPI.ModdedDamageType barrierOnHit = DamageAPI.ReserveDamageType();

        public static void Init()
        {
            prefab = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mage/MageLightningBombProjectile.prefab").WaitForCompletion(), "Sunfall");

            var rigidBody = prefab.GetComponent<Rigidbody>();
            rigidBody.useGravity = false;
            rigidBody.freezeRotation = false;

            var projectileSimple = prefab.GetComponent<ProjectileSimple>();
            projectileSimple.lifetime = 10f;
            projectileSimple.desiredForwardSpeed = 50f;
            projectileSimple.enableVelocityOverLifetime = true;
            projectileSimple.velocityOverLifetime = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.025f, 1f), new Keyframe(0.026f, 1f), new Keyframe(1f, 3f));

            var projectileDamage = prefab.GetComponent<ProjectileDamage>();
            projectileDamage.damageType = DamageType.Generic;

            var newImpact = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Grandparent/GrandparentSpawnImpact.prefab").WaitForCompletion(), "Sunfall Impact", false);

            var effectComponent = newImpact.GetComponent<EffectComponent>();
            effectComponent.soundName = "PaladinExplosion";

            ContentAddition.AddEffect(newImpact);

            var projectileImpactExplosion = prefab.GetComponent<ProjectileImpactExplosion>();
            projectileImpactExplosion.impactEffect = newImpact;
            projectileImpactExplosion.blastRadius = 18f;

            var projectileController = prefab.GetComponent<ProjectileController>();
            projectileController.flightSoundLoop = Addressables.LoadAssetAsync<RoR2.Audio.LoopSoundDef>("RoR2/DLC1/VoidMegaCrab/lsdMegaCrabBlackCannonProjectile.asset").WaitForCompletion();

            var newGhost = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/LemurianBruiser/MegaFireballGhost.prefab").WaitForCompletion(), "Sunfall Ghost", false);

            var trans = newGhost.transform;
            for (int i = 0; i < trans.childCount; i++)
            {
                var child = trans.GetChild(i);
                child.localScale *= 0.7333333333f;
            }

            var pointLight = trans.Find("Point Light");
            pointLight.GetComponent<FlickerLight>().enabled = false;
            var light = pointLight.GetComponent<Light>();
            light.range = 24f;
            light.intensity = 50f;

            var icoSphere = Addressables.LoadAssetAsync<Mesh>("RoR2/Base/Common/VFX/mdlVFXIcosphere.fbx").WaitForCompletion();

            var flameTrailsRenderer = trans.Find("FlameTrails, World").GetComponent<ParticleSystemRenderer>();
            flameTrailsRenderer.mesh = icoSphere;
            flameTrailsRenderer.renderMode = ParticleSystemRenderMode.Mesh;
            flameTrailsRenderer.gameObject.transform.localScale = Vector3.one * 0.566666666f;

            var newMat = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matOmniExplosion1.mat").WaitForCompletion());
            newMat.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture2D>("RoR2/DLC1/Common/ColorRamps/texRampConstructLaserTypeB.png").WaitForCompletion());
            newMat.SetFloat("_AlphaBoost", 1f);

            flameTrailsRenderer.material = newMat;

            var flameBillboardsRenderer = trans.Find("FlameBillboards, Local").GetComponent<ParticleSystemRenderer>();
            flameBillboardsRenderer.mesh = icoSphere;
            flameBillboardsRenderer.renderMode = ParticleSystemRenderMode.Mesh;
            flameBillboardsRenderer.gameObject.transform.localScale = Vector3.one;

            flameBillboardsRenderer.material = newMat;

            projectileController.ghostPrefab = newGhost;

            Object.DestroyImmediate(prefab.GetComponent<AkEvent>());
            Object.DestroyImmediate(prefab.GetComponent<AkGameObj>());
            Object.Destroy(prefab.GetComponent<ProjectileProximityBeamController>());
            Object.Destroy(prefab.GetComponent<AntiGravityForce>());

            var sphereCollider = prefab.GetComponent<SphereCollider>();
            sphereCollider.radius = 0.5f;

            prefab.AddComponent<SunfallController>();

            PrefabAPI.RegisterNetworkPrefab(prefab);
            ContentAddition.AddProjectile(prefab);

            GlobalEventManager.onServerDamageDealt += GlobalEventManager_onServerDamageDealt;
        }

        private static void GlobalEventManager_onServerDamageDealt(DamageReport report)
        {
            var attackerBody = report.attackerBody;
            if (!attackerBody)
            {
                return;
            }

            if (DamageAPI.HasModdedDamageType(report.damageInfo, barrierOnHit))
            {
                attackerBody.healthComponent.AddBarrierAuthority(attackerBody.healthComponent.fullCombinedHealth * CruelSunState.barrierGain);
            }
        }
    }

    public class SunfallController : MonoBehaviour
    {
        public SphereCollider sphereCollider;
        public float timer;
        public float delay = 0.33f;

        public void Start()
        {
            sphereCollider = GetComponent<SphereCollider>();
            sphereCollider.enabled = false;
        }

        public void FixedUpdate()
        {
            timer += Time.fixedDeltaTime;
            if (timer >= delay)
            {
                sphereCollider.enabled = true;
            }
        }
    }
}