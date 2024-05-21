using System;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace UnityEngine.Rendering.Universal
{
    [Serializable, VolumeComponentMenu("Akilli Mum/SRP/Post Processing/Pixelisation")]
    public class PixelisationVolume : VolumeBase
    {
        public FloatParameter Pixelisation = new ClampedFloatParameter(0.01f, 0.01f,120f, false);
    }
}
