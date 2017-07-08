using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public static class LevelDetectionPipeline {
    //Each int32 is 4 bytes... So it's 8 bits
    [StructLayout(LayoutKind.Sequential, Size = 8)]
    public struct LevelElement {
        public int X, Y;
    }

    [DllImport("OpenCV Unity Integration")]
    static extern int Init(ref int outCameraWidth, ref int outCameraHeight);

    [DllImport("OpenCV Unity Integration")]
    static extern int Close();

    [DllImport("OpenCV Unity Integration")]
    static extern int DetectionPipeline(ref int stateIn, ref int stateOut);

    [DllImport("OpenCV Unity Integration")]
    static extern int SetupBlackPlatforms(ref int ammountDetected);

    [DllImport("OpenCV Unity Integration")]
    static extern int SetupRedPlatforms(ref int ammountDetected);

    [DllImport("OpenCV Unity Integration")]
    static extern int SetupYellowPlatforms(ref int ammountDetected);

    [DllImport("OpenCV Unity Integration")]
    static extern int SetupGreenPlatforms(ref int ammountDetected);

    [DllImport("OpenCV Unity Integration")]
    static extern int SetupBluePlatforms(ref int ammountDetected);

    [DllImport("OpenCV Unity Integration")]
    unsafe static extern int GetBlackPlatforms(LevelElement* platforms, int maxPlatformsCount, ref int platformsCount);

    [DllImport("OpenCV Unity Integration")]
    unsafe static extern int GetRedPlatforms(LevelElement* platforms, int maxPlatformsCount, ref int platformsCount);

    [DllImport("OpenCV Unity Integration")]
    unsafe static extern int GetYellowPlatforms(LevelElement* platforms, int maxPlatformsCount, ref int platformsCount);

    [DllImport("OpenCV Unity Integration")]
    unsafe static extern int GetGreenPlatforms(LevelElement* platforms, int maxPlatformsCount, ref int platformsCount);

    [DllImport("OpenCV Unity Integration")]
    unsafe static extern int GetBluePlatforms(LevelElement* platforms, int maxPlatformsCount, ref int platformsCount);


}
