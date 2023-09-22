using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StringUtils
{
    public static string FindCommonLinesStarting(List<string> lines, params char[] lastRequiredChars)
    {
        if (null == lines) return "";
        if (0 == lines.Count) return "";

        string firstLine = lines[0];
        int firstLineLength = firstLine.Length;

        int index;

        for (index = 0; index < firstLineLength; index++)
        {
            char chr = firstLine[index];
            if (!IsLinesHasChar(lines, chr, index)) break;
        }

        string resStr = firstLine.Substring(0, index);
        int lastRequiredCharIndex = -1;
        foreach (char chr in lastRequiredChars)
        {
            lastRequiredCharIndex = Mathf.Max(lastRequiredCharIndex, resStr.LastIndexOf(chr));
        }

        lastRequiredCharIndex++;

        resStr = firstLine.Substring(0, lastRequiredCharIndex);

        return resStr;
    }

    public static List<string> CutCommonStartingLines(List<string> lines, out string commonLinesPart, params char[] lastRequiredChars)
    {
        commonLinesPart = FindCommonLinesStarting(lines, lastRequiredChars);

        if (string.IsNullOrEmpty(commonLinesPart)) return new List<string>(lines);

        int commonLinesPartLength = commonLinesPart.Length;
        List<string> resList = new List<string>(lines.Count);

        foreach (string line in lines)
        {
            string subString = line.Substring(commonLinesPartLength, line.Length - commonLinesPartLength);
            resList.Add(subString);
        }

        return resList;
    }

    public static bool IsLinesHasChar(List<string> lines, char chr, int index)
    {
        foreach (string line in lines)
        {
            if (line.Length <= index) return false;
            if (line[index] != chr) return false;
        }

        return true;
    }

    public static List<string> SeparatePath(string fullPath, bool removeFileName)
    {
        string[] elements = fullPath.Split(new[] { System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar }, System.StringSplitOptions.RemoveEmptyEntries);
        List<string> resList = new List<string>(elements);

        if (removeFileName)
        {
            resList.RemoveAt(resList.Count - 1);
        }

        return resList;
    }

    public static void SeparateParentChildPaths(
        string currentDirectory, 
        List<string> allFiles, 
        out List<string> childrenFiles,
        out List<string> parentFiles,
        out List<string> externalFiles,
        out string ownFile,
        out string nearestParentFile)
    {
        childrenFiles = new List<string>();
        parentFiles = new List<string>();
        externalFiles = new List<string>();
        ownFile = null;
        nearestParentFile = null;

        List<string> currentDirectoryElements = SeparatePath(currentDirectory, false);

        foreach (string filePath in allFiles)
        {
            List<string> separatedFilePath = SeparatePath(filePath, true);
            CommonUtils.ElementsEntry elementsEntry = CommonUtils.GetListsElementsEntry(currentDirectoryElements, separatedFilePath);

            switch (elementsEntry)
            {
                case CommonUtils.ElementsEntry.Equals:
                    ownFile = filePath;
                    break;

                case CommonUtils.ElementsEntry.FirstIsAPartOfSecond:
                    childrenFiles.Add(filePath);
                    break;

                case CommonUtils.ElementsEntry.SecondIsAPartOfFirst:
                    parentFiles.Add(filePath);

                    if (string.IsNullOrEmpty(nearestParentFile))
                    {
                        nearestParentFile = filePath;
                    }
                    else
                    {
                        if (nearestParentFile.Length > filePath.Length)
                        {
                            nearestParentFile = filePath;
                        }
                    }

                    break;

                case CommonUtils.ElementsEntry.None:
                    externalFiles.Add(filePath);
                    break;
            }
        }
    }
}
