// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.CompilerServices;

unsafe class Runtime_71831
{
    private static int Main()
    {
        return Problem(100) ? 101 : 100;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static bool Problem(int i)
    {
        StructWithFloats s = default;

        s.FloatOne = BitConverter.Int32BitsToSingle(i);

        return s.FloatOne != *(float*)&i;
    }

    struct StructWithFloats
    {
        public float FloatOne;
        public float FloatTwo;
    }
}
