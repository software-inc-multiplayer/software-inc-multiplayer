using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Multiplayer.Extensions
{
    public static class UI
    {
        public static void SetTitle(this GUIWindow Window, string title)
        {
            Window.InitialTitle = Window.TitleText.text = Window.NonLocTitle = title;
        }
        public static Texture2D LoadPNG(string filePath, int width, int height)
        {
            Texture2D tex = null;
            byte[] fileData;
            if (File.Exists(filePath))
            {
                fileData = File.ReadAllBytes(filePath);
                tex = new Texture2D(width, height);
                tex.LoadImage(fileData);
            }
            return tex;
        }
        public static void AddBulk<T>(this List<T> list, params T[] bulkToAdd)
        {
            foreach(T a in bulkToAdd)
            {
                list.Add(a);
            }
        }
        public static void AddToElement(this GameObject toAdd, GameObject parent, Rect rect, Rect bounding)
        {
            WindowManager.AddElementToElement(toAdd, parent, rect, bounding);
        }
    }
}
