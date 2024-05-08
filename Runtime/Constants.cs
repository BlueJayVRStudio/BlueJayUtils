using System;
using UnityEngine;
using Unity.Netcode;

namespace bluejayvrstudio
{
    public enum GrabberType
    {
        None,
        Left,
        Right
    }

    public enum ArcadeObjectType
    {
        Menu,
        Decor,
        PhysicsObject,
        Shape
    }

    public enum Axis
    {
        Y,
        X,
        Z
    }
}