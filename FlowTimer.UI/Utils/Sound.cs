using System.Media;

namespace FlowTimer.UI.Utils
{
    public class Sound
    {
        private SoundPlayer _soundPlayer;

        public Sound()
        {
            _soundPlayer = new SoundPlayer();
        }

        public void Play(string sound)
        {
            _soundPlayer.SoundLocation = sound;
            _soundPlayer.Load();
            _soundPlayer.Play();
        }
    }
}
