using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public AudioSource[] myMusic;   // List of all music
    public bool isMusic;

    private int currentSong = -1;    // Keep track of the current song playing
    private float reducedVolume = 0.3f;

    public void PlayMusic(int index)
    {
        // Stop any music currently playing
        //StopMusic();
        
        // Start playing the new song
        currentSong = index;
        if (index >= 0) {
            myMusic[index].enabled = true;
            myMusic[index].Play();
        }
    }


     //if current song is playing, stop and disable
     public void StopMusic()
     {
         if (currentSong >= 0) {
             myMusic[currentSong].Pause();
             myMusic[currentSong].enabled = false;
         }
     }


     public void SetVolume(float vol) {

         // Special case for white noise and background music
         if (isMusic && (currentSong == 3 || currentSong == 2)) {
             if (vol == 1f) {
                 myMusic[currentSong].volume = reducedVolume;
             }
             else {
                 if (vol >= 0f) {
                     myMusic[currentSong].volume = vol;
                 }
             }
         }
         else {
             if (currentSong >= 0) {
                 if (vol >= 0f) {
                     myMusic[currentSong].volume = vol;
                 }
             }
         }

     } // end of SetVolume()

} // end of MusicPlayer.cs
