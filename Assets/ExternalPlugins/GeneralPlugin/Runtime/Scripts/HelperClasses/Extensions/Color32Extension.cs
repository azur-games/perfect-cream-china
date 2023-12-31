﻿using UnityEngine;


public static class Color32Extension
{
    public static Color32 RandomColor
    {
        get
        {
            return new Color32((byte)Random.Range(0, 256), (byte)Random.Range(0, 256), (byte)Random.Range(0, 256), 255);
        }
    }


    public static Color32 Set(this Color32 c, byte r, byte g, byte b, byte a)
    {
        c.r = r;
        c.g = g;
        c.b = b;
        c.a = a;
        
        return c;
    }


    public static Color32 SetR(this Color32 c, byte r)
    {
        c.r = r;
        
        return c;
    }


    public static Color32 SetG(this Color32 c, byte g)
    {
        c.g = g;
        
        return c;
    }


    public static Color32 SetB(this Color32 c, byte b)
    {
        c.b = b;
        
        return c;
    }


    public static Color32 SetA(this Color32 c, byte a)
    {
        c.a = a;
        
        return c;
    }


    public static uint ToUInt(this Color32 color)
    {
        return (uint)ToInt(color);
    }
    
    
    public static int ToInt(this Color32 color)
    {
        return (color.a << 24) | (color.r << 16) |
               (color.g << 8) | (color.b << 0);
    }


    public static Color32 ToColor32(this uint color)
    {
        byte a = (byte)(color >> 24);
        byte r = (byte)(color >> 16);
        byte g = (byte)(color >> 8);
        byte b = (byte)(color >> 0);

        return new Color32(r, g, b, a);
    }


    public static float GetBrightness(this Color32 color)
    {
        Color c = color;
        return c.GetBrightness();
    }


    public static float GetHue(this Color32 color)
    {
        Color c = color;
        return c.GetHue();
    }


    public static float GetSaturation(this Color32 color)
    {
        Color c = color;
        return c.GetSaturation();
    }


    /// <summary>
    /// Shift RGB color channels
    /// </summary>
    public static Color32 Shift(this Color32 c, byte shift)
    {
        return new Color32((byte)(c.r + shift).Clamp(0, 255), 
                           (byte)(c.g + shift).Clamp(0, 255), 
                           (byte)(c.b + shift).Clamp(0, 255), c.a);
    }


    public static string ToHex(this Color32 c)
    {
        return string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", c.r, c.g, c.b, c.a);
    }
}
