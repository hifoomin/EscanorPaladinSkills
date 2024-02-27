using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EscanorPaladinSkills.Projectiles
{
    public static class Judgement
    {
        public static GameObject prefab;

        public static void Init()
        {
            prefab = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ElementalRings/FireTornado.prefab").WaitForCompletion(), "WallOfInfernoPillar");
            prefab.transform.eulerAngles = new Vector3(0, 0, 90);

            Object.Destroy(prefab.GetComponent<SphereCollider>());

            var cc = prefab.AddComponent<CapsuleCollider>();
            cc.isTrigger = false;
            cc.center = new Vector3(0f, 0f, 0f);
            cc.radius = 1f;
            cc.height = 1f;

            // add collider for gravity

            var hitbox = prefab.transform.GetChild(0);
            hitbox.transform.localScale = new Vector3(4000f, 4000f, 10f);
            hitbox.transform.localPosition = Vector3.zero; // 0, 0, 8
            hitbox.transform.localEulerAngles = new Vector3(0f, 90f, 0f);

            // add hitbox

            var rb = prefab.GetComponent<Rigidbody>();
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.useGravity = true;
            rb.freezeRotation = true;

            // add rb for gravity

            var cf = prefab.AddComponent<ConstantForce>();
            cf.force = new Vector3(0f, -4500f, 0f);

            // add gravity real

            var psoi = prefab.AddComponent<ProjectileStickOnImpact>();
            psoi.ignoreCharacters = true;
            psoi.ignoreWorld = false;
            psoi.alignNormals = false;

            var ps = prefab.GetComponent<ProjectileSimple>();
            ps.lifetime = 15f;

            ProjectileDamage pd = prefab.GetComponent<ProjectileDamage>();
            pd.damageType = DamageType.IgniteOnHit;

            ProjectileOverlapAttack overlap = prefab.GetComponent<ProjectileOverlapAttack>();
            overlap.damageCoefficient = 0.1f;
            overlap.resetInterval = 0.1f;
            overlap.overlapProcCoefficient = 0.75f;

            ProjectileController projectileController = prefab.GetComponent<ProjectileController>();
            var ghostPrefab = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Junk/Mage/MageFirePillarGhost.prefab").WaitForCompletion(), "WallOfInfernoPillarGhost", false);
            // Main.HATLogger.LogError("ghost prefab is " + ghostPrefab);
            ghostPrefab.transform.GetChild(1).gameObject.SetActive(false);

            var pillar = ghostPrefab.transform.GetChild(0);
            pillar.localScale = new Vector3(4000f, 4000f, 10f);

            var pillarParticleSystem = pillar.GetComponent<ParticleSystem>();
            var pillarPSR = pillarParticleSystem.GetComponent<ParticleSystemRenderer>();
            pillarPSR.maxParticleSize = 1000f;
            pillarPSR.alignment = ParticleSystemRenderSpace.Facing;

            var newMat = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matFirePillarParticle.mat").WaitForCompletion());
            newMat.SetInt("_FadeCloseOn", 1);
            newMat.shaderKeywords = new string[] { "CLOUDOFFSET", "FADECLOSE", "USE_CLOUDS", "_EMISSION" };
            newMat.SetFloat("_InvFade", 0.03742486f);
            newMat.SetFloat("_Boost", 9.221877f);
            newMat.SetFloat("_AlphaBoost", 3.994069f);
            newMat.SetFloat("_AlphaBias", 0.4729744f);
            newMat.SetFloat("_FadeCloseDistance", 0.02f);
            newMat.SetFloat("_DistortionStrength", 0.3714001f);
            newMat.SetTextureScale("_MainTex", new Vector2(4000f, 1f));
            newMat.SetTextureScale("_Cloud1Tex", new Vector2(4000f, 1000f));
            newMat.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture2D>("RoR2/Base/Common/ColorRamps/texRampBeetleQueen.png").WaitForCompletion());
            newMat.SetInt("_SrcBlend", 8);
            newMat.SetColor("_TintColor", new Color32(255, 62, 62, 255));

            pillarPSR.material = newMat;

            var pillarVelocity = pillarParticleSystem.velocityOverLifetime;
            // pillarVelocity.speedModifier = 0.005f;
            // pillarVelocity.speedModifier = 0.05f;
            // pillarVelocity.speedModifier = 0f;

            var gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(0f, 0.0f), new GradientAlphaKey(1f, 0.7f), new GradientAlphaKey(1f, 0.9f), new GradientAlphaKey(0f, 1.0f) });

            var pillarColor = pillarParticleSystem.colorOverLifetime;
            pillarColor.color = gradient;

            var pillarMain = pillarParticleSystem.main;
            var pillarStartSpeed = pillarMain.startSpeed;
            // pillarStartSpeed.constant = 0.005f;
            // pillarStartSpeed.constant = 0.0005f;

            ghostPrefab.transform.eulerAngles = new Vector3(0, 0, 90);
            var destroyOnTimer = ghostPrefab.AddComponent<DestroyOnTimer>();
            destroyOnTimer.duration = 15f;

            projectileController.ghostPrefab = ghostPrefab;

            ContentAddition.AddProjectile(prefab);
        }
    }
}