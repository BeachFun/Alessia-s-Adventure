using UnityEngine;

public class CharacterSoundController : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)] private float attackSoundVolume = 0.3f;
    [SerializeField, Range(0f, 1f)] private float throwSoundVolume = 0.3f;
    [SerializeField, Range(0f, 1f)] private float hitSoundVolume = 0.3f;
    [SerializeField, Range(0f, 1f)] private float stepSoundVolume = 0.3f;
    [SerializeField, Range(0f, 1f)] private float healSoundVolume = 0.3f;
    [SerializeField, Range(0f, 1f)] private float jumpSoundVolume = 0.3f;
    [SerializeField, Range(0f, 1f)] private float landingSoundVolume = 0.3f;
    [SerializeField, Range(0f, 1f)] private float slidingSoundVolume = 0.3f;

    public void AttackSoundPlay()
    {
        GameManagers.Audio?.PlaySound(AudioClipPool.Instance["Attack"], attackSoundVolume);
    }

    public void ThrowSoundPlay()
    {
        GameManagers.Audio?.PlaySound(AudioClipPool.Instance["Throw"], throwSoundVolume);
    }

    public void HitSoundPlay()
    {
        GameManagers.Audio?.PlaySound(AudioClipPool.Instance["Hit"], hitSoundVolume);
    }

    public void StepSoundPlay()
    {
        GameManagers.Audio?.PlaySound(AudioClipPool.Instance["Step"], stepSoundVolume);
    }

    public void HealSoundPlay()
    {
        GameManagers.Audio?.PlaySound(AudioClipPool.Instance["Hit"], healSoundVolume);
    }

    public void JumpSoundPlay()
    {
        GameManagers.Audio?.PlaySound(AudioClipPool.Instance["Jump"], jumpSoundVolume);
    }

    public void LandingSoundPlay()
    {
        GameManagers.Audio?.PlaySound(AudioClipPool.Instance["Landing"], landingSoundVolume);
    }
    public void SlidingSoundPlay()
    {
        GameManagers.Audio?.PlaySound(AudioClipPool.Instance["Sliding"], slidingSoundVolume);
    }

    public void EnergyBallThrowSoundPlay()
    {
        GameManagers.Audio?.PlaySound(AudioClipPool.Instance["Energy Ball"]);
    }
}
