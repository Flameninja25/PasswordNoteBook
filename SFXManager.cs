using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace PasswordNoteBook
{
    class SFXManager
    {
        //method to play wanted sound
        private void PlaySound(string soundPath)
        {
            if (soundPath == null) return;
            using (FileStream stream = File.Open(soundPath, FileMode.Open))
            {
                SoundPlayer myNewSound = new SoundPlayer(stream);
                myNewSound.Load();
                myNewSound.Play();
            }
        }

        public void ChangePage()
        {
            PlaySound(@"audio/PageFlip.wav");
        }

    }
}
