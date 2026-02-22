using UnityEngine;

public class FXInstance : MonoBehaviour
{
    private FXSystem fxSystem;
    public ParticleSystem Particle { get; private set; }
    public ParticleSystem ParticleKey { get; private set; }

    private void Awake()
    {
        Particle = GetComponent<ParticleSystem>();
    }

    public void Initialize(FXSystem fxSystem, ParticleSystem key, bool isLoop)
    {
        this.fxSystem = fxSystem;
        ParticleKey = key;

        var main = Particle.main;
        main.loop = isLoop;
        main.stopAction = (!isLoop) ? ParticleSystemStopAction.Callback : ParticleSystemStopAction.None;
    }

    public void Play()
    {
        Particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        Particle.Play(true);
    }

    public void Stop()
    {
        Particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    private void OnParticleSystemStopped()
    {
        if (Particle != null && ParticleKey != null)
            fxSystem.Return(ParticleKey, Particle);
    }
}