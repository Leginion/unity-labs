using UnityEngine;

public class ParticleEffectConfig : MonoBehaviour
{
    [Header("Hit Effect Settings")]
    [SerializeField] private ParticleSystem hitParticles;
    [SerializeField] private Color hitColor = new Color(1f, 0.6f, 0f, 1f);
    [SerializeField] private int hitParticleCount = 10;
    [SerializeField] private float hitParticleSize = 0.2f;
    [SerializeField] private float hitParticleSpeed = 3f;

    [Header("Death Effect Settings")]
    [SerializeField] private ParticleSystem deathParticles;
    [SerializeField] private Color deathColor = new Color(1f, 0.3f, 0f, 1f);
    [SerializeField] private int deathParticleCount = 50;
    [SerializeField] private float deathParticleSize = 0.5f;
    [SerializeField] private float deathParticleSpeed = 8f;

    void Start()
    {
        ConfigureHitEffect();
        ConfigureDeathEffect();
    }

    void ConfigureHitEffect()
    {
        if (hitParticles == null) return;

        var main = hitParticles.main;
        main.startColor = hitColor;
        main.startSize = hitParticleSize;
        main.startSpeed = hitParticleSpeed;
        main.startLifetime = 0.5f;
        main.maxParticles = hitParticleCount;

        var emission = hitParticles.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] {
            new ParticleSystem.Burst(0f, hitParticleCount)
        });

        var shape = hitParticles.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.5f;
    }

    void ConfigureDeathEffect()
    {
        if (deathParticles == null) return;

        var main = deathParticles.main;
        main.startColor = deathColor;
        main.startSize = deathParticleSize;
        main.startSpeed = deathParticleSpeed;
        main.startLifetime = 1f;
        main.maxParticles = deathParticleCount;

        var emission = deathParticles.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] {
            new ParticleSystem.Burst(0f, deathParticleCount)
        });

        var shape = deathParticles.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 1f;
    }
}
