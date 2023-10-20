# com.undefined.wwise-recorder

![Unofficial](https://img.shields.io/badge/unofficial-8A2BE2)

An unofficial [Wwise](https://www.audiokinetic.com/fr/products/wwise/) Recorder module for Unity's Recorder.

This adds a new recorder type to the Unity Recorder. It comes with its own input and enables storing output as a .wav file.

I am not affiliated with either Unity or Audiokinetic.

***All copyrights, trademarks, logos, and assets are the property of their respective owners.***

## Installing

Requires Unity 2021.3 LTS or higher.

### Via Unity Package Manager and Git url

- Open your Unity Package Manager
- Add package from git url: `https://github.com/nehvaleem/com.undefined.wwise-recorder.git`

---

### Known Issues

It seems that Wwise starts producing output before Unity does. This leads to the situation where captured audio output is a bit longer than video for example.
For now, I recommend manually synchronizing tracks at the end (at least it seems to work in my case).
