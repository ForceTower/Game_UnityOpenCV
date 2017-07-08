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
    public static extern int Init(ref int outCameraWidth, ref int outCameraHeight);

    [DllImport("OpenCV Unity Integration")]
    public static extern void Close();

    [DllImport("OpenCV Unity Integration")]
    public static extern void DetectionPipeline(ref int stateIn, ref int stateOut);

    [DllImport("OpenCV Unity Integration")]
    public static extern void SetState(int state);

    [DllImport("OpenCV Unity Integration")]
    public static extern void SetupBlackPlatforms(ref int ammountDetected);

    [DllImport("OpenCV Unity Integration")]
    public static extern void SetupRedPlatforms(ref int ammountDetected);

    [DllImport("OpenCV Unity Integration")]
    public static extern void SetupYellowPlatforms(ref int ammountDetected);

    [DllImport("OpenCV Unity Integration")]
    public static extern void SetupGreenPlatforms(ref int ammountDetected);

    [DllImport("OpenCV Unity Integration")]
    public static extern void SetupBluePlatforms(ref int ammountDetected);

    [DllImport("OpenCV Unity Integration")]
    public unsafe static extern void GetBlackPlatforms(LevelElement* platforms, int maxPlatformsCount, ref int platformsCount);

    [DllImport("OpenCV Unity Integration")]
    public unsafe static extern void GetRedPlatforms(LevelElement* platforms, int maxPlatformsCount, ref int platformsCount);

    [DllImport("OpenCV Unity Integration")]
    public unsafe static extern void GetYellowPlatforms(LevelElement* platforms, int maxPlatformsCount, ref int platformsCount);

    [DllImport("OpenCV Unity Integration")]
    public unsafe static extern void GetGreenPlatforms(LevelElement* platforms, int maxPlatformsCount, ref int platformsCount);

    [DllImport("OpenCV Unity Integration")]
    public unsafe static extern void GetBluePlatforms(LevelElement* platforms, int maxPlatformsCount, ref int platformsCount);


}
