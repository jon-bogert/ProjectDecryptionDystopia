using System;

public class GameSettings
{
    private static GameSettings _inst = null;
    public static GameSettings instance
    { 
        get
        {
            if (_inst == null)
                _inst = new GameSettings();
            return _inst;
        }
    }

    public float musicVolume = 1f;
    public float sfxVolume = 1f;
    public bool hmdRelativeMovement = true;
}
