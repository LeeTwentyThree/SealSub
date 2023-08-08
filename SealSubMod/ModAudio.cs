using FMOD;

namespace SealSubMod;

internal static class ModAudio
{
    public const MODE k3DSoundModes = MODE.DEFAULT | MODE._3D | MODE.ACCURATETIME | MODE._3D_LINEARSQUAREROLLOFF;
    public const MODE k2DSoundModes = MODE.DEFAULT | MODE._2D | MODE.ACCURATETIME;
    public const MODE kStreamSoundModes = k2DSoundModes | MODE.CREATESTREAM;

    public static void RegisterAudio(AssetBundle bundle)
    {
        AddSubVoiceLine(bundle.LoadAsset<AudioClip>("SealWelcomeAboard"), "SealWelcomeAboard");
    }

    private static void AddSubVoiceLine(AudioClip clip, string soundPath)
    {
        var sound = AudioUtils.CreateSound(clip, kStreamSoundModes);
        CustomSoundHandler.RegisterCustomSound(soundPath, sound, AudioUtils.BusPaths.VoiceOvers);
    }

    private static void AddWorldSoundEffect(AudioClip clip, string soundPath, float minDistance = 1f, float maxDistance = 100f, string overrideBus = null)
    {
        var sound = AudioUtils.CreateSound(clip, k3DSoundModes);
        if (maxDistance > 0f)
        {
            sound.set3DMinMaxDistance(minDistance, maxDistance);
        }
        CustomSoundHandler.RegisterCustomSound(soundPath, sound, string.IsNullOrEmpty(overrideBus) ? AudioUtils.BusPaths.PlayerSFXs : overrideBus);
    }

    private static void AddInterfaceSoundEffect(AudioClip clip, string soundPath)
    {
        var sound = AudioUtils.CreateSound(clip, k2DSoundModes);
        CustomSoundHandler.RegisterCustomSound(soundPath, sound, AudioUtils.BusPaths.PlayerSFXs);
    }
}