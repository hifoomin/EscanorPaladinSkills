﻿/*
using PaladinMod.Modules;
using PaladinMod;
using R2API;
using RoR2;
using UnityEngine;
using RoR2.Projectile;
using PaladinMod.Misc;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.AddressableAssets;

namespace EscanorPaladinSkills.Projectiles
{
    public static class SunlightSpearUpgraded
    {
        public static GameObject prefab;
        public static GameObject boltPrefab;

        public static void Init()
        {
            prefab = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/MageLightningBombProjectile"), "Molten Spear", true);
            prefab.AddComponent<PaladinMod.Misc.ProjectileOverchargeOnImpact>();

            GameObject spearGhost = Assets.lightningSpear.InstantiateClone("Molten Spear Ghost", false);
            spearGhost.AddComponent<ProjectileGhostController>();

            //vfx
            foreach (ParticleSystemRenderer i in spearGhost.GetComponentsInChildren<ParticleSystemRenderer>())
            {
                if (i) i.trailMaterial = PaladinMod.Modules.Assets.matYellowLightningLong;
            }
            Light light = spearGhost.GetComponentInChildren<Light>();
            light.range = 16f;
            light.intensity = 32f;
            spearGhost.GetComponentInChildren<TrailRenderer>().material = PaladinMod.Modules.Assets.matYellowLightningLong;

            prefab.transform.localScale *= 2f;

            prefab.GetComponent<ProjectileController>().ghostPrefab = spearGhost;
            //lightningSpear.GetComponent<ProjectileOverlapAttack>().impactEffect = Assets.lightningImpactFX;
            prefab.GetComponent<ProjectileDamage>().damageType = DamageType.Shock5s;
            prefab.GetComponent<ProjectileImpactExplosion>().impactEffect = Assets.altLightningImpactFX;
            prefab.GetComponent<Rigidbody>().useGravity = false;

            PaladinPlugin.Destroy(prefab.GetComponent<AntiGravityForce>());
            PaladinPlugin.Destroy(prefab.GetComponent<ProjectileProximityBeamController>());

            prefab.AddComponent<FuckOnHit>();

            PrefabAPI.RegisterNetworkPrefab(prefab);
            ContentAddition.AddProjectile(prefab);

            boltPrefab = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ElectricWorm/ElectricWormSeekerProjectile.prefab").WaitForCompletion(), "Molten Spear Bolt");

            var projectileSimple = boltPrefab.GetComponent<ProjectileSimple>();
            projectileSimple.lifetime = 8f;
            projectileSimple.desiredForwardSpeed = 60f;

            var projectileImpactExplosion = boltPrefab.GetComponent<ProjectileImpactExplosion>();
            projectileImpactExplosion.blastRadius = 6f;

            PrefabAPI.RegisterNetworkPrefab(boltPrefab);
            ContentAddition.AddProjectile(boltPrefab);
        }

        public class FuckOnHit : MonoBehaviour, IProjectileImpactBehavior
        {
            public ProjectileController projectileController;
            public CharacterBody ownerBody;

            public void Start()
            {
                projectileController = GetComponent<ProjectileController>();
                var owner = projectileController.owner;
                if (owner)
                {
                    ownerBody = owner.GetComponent<CharacterBody>();
                    Main.logger.LogError("owner is " + owner);
                    Main.logger.LogError("owner body is " + ownerBody);
                }
            }

            public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
            {
                StartCoroutine(FireFuckingFuckingProjectiles(impactInfo));
            }

            public IEnumerator FireFuckingFuckingProjectiles(ProjectileImpactInfo impactInfo)
            {
                for (int i = 1; i < 8; i++)
                {
                    var fpi = new FireProjectileInfo()
                    {
                        crit = ownerBody.RollCrit(),
                        damage = ownerBody.damage * 2f,
                        force = 1000f,
                        owner = ownerBody.gameObject,
                        projectilePrefab = boltPrefab,
                        position = impactInfo.estimatedPointOfImpact,
                        rotation = Util.QuaternionSafeLookRotation(new Vector3(1f / i, 1f, 1f / i)),
                    };

                    ProjectileManager.instance.FireProjectile(fpi);

                    yield return new WaitForSeconds(0.5f / i);
                }
            }
        }
    }
}
*/