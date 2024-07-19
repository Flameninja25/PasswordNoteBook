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
        /// <summary>
        /// method to play wanted sound
        /// </summary>
        /// <param name="soundPath"></param>
        private void PlaySound(string soundPath)
        {
            //check if the sound path is filled
            if (soundPath == null) return;
            //open the file path load then play the sound
            using (FileStream stream = File.Open(soundPath, FileMode.Open))
            {
                SoundPlayer myNewSound = new SoundPlayer(stream);
                myNewSound.Load();
                myNewSound.Play();
            }
        }

        /// <summary>
        /// play the page trunging sound
        /// </summary>
        public void ChangePage()
        {
            PlaySound(@"audio/PageFlip.wav");
        }

        /// <summary>
        /// play the pencil writing sound
        /// </summary>
        public void WriteInfo()
        {
            PlaySound(@"audio/Writing.wav");
        }

        /// <summary>
        /// play the unlock page
        /// </summary>
        public void UnlockPage()
        {
            PlaySound(@"audio/Unlock.wav");
        }

    }
}
