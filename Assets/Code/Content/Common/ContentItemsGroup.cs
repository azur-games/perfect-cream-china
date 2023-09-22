using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ContentItemsGroup", menuName = "Configs/ContentItemsGroup")]
public class ContentItemsGroup : ScriptableObject
{
    [System.Serializable]
    public class Element
    {
        [SerializeField] string tag = null;
        [SerializeField] ContentItemsGroup subgroup = null;

        public string Tag
        {
            get
            {
                return tag;
            }
        }

        public ContentItemsGroup Subgroup
        {
            get
            {
                return subgroup;
            }
        }

        public Element(string tag)
        {
            this.tag = tag;
        }

        public Element(ContentItemsGroup subgroup)
        {
            this.subgroup = subgroup;
        }

        public bool IsYou(string tag)
        {
            return (this.tag == tag);
        }

        public bool IsYou(ContentItemsGroup subgroup)
        {
            return (this.subgroup == subgroup);
        }

        public bool IsSubgroup
        {
            get
            {
                return (null != subgroup);
            }
        }

        public bool IsTag
        {
            get
            {
                return (!string.IsNullOrEmpty(tag));
            }
        }
    }

    #region Fields

    //[SerializeField] TypeOfField fieldType = TypeOfField.None;
    [SerializeField] string groupName = "";
    [SerializeField] Texture2D icon = null;
    [SerializeField] List<Element> elements = new List<Element>();
    
    #endregion


    #region Properties

    public Texture2D Icon
    {
        get
        {
            return icon;
        }

        set
        {
            icon = value;
        }
    }

    public string GroupName
    {
        get
        {
            return groupName;
        }

        set
        {
            groupName = value;
        }
    }

    /*public TypeOfField FieldType
    {
        get
        {
            return fieldType;
        }

        set
        {
            fieldType = value;
        }
    }*/

    public List<Element> Elements
    {
        get
        {
            return elements;
        }
    }
    
    #endregion

    public void AddTag(string tag)
    {
        if (null == elements)
            elements = new List<Element>();

        Element el = new Element(tag);
        elements.Add(el);
    }

    public void AddSubgroup(ContentItemsGroup subgroup)
    {
        if (null == elements)
            elements = new List<Element>();

        Element el = new Element(subgroup);
        elements.Add(el);
    }

    public bool ContainsTag(string tag, bool recursively)
    {
        if (null == elements) return false;

        // check subgroups
        foreach (Element element in elements)
        {
            if (element.IsYou(tag)) return true;

            if (recursively && element.IsSubgroup)
            {
                element.Subgroup.ContainsTag(tag, true);
            }
        }

        return false;
    }

    public bool ContainsSubgroup(ContentItemsGroup subgroup, bool recursively)
    {
        if (null == elements) return false;

        foreach (Element element in elements)
        {
            if (!element.IsSubgroup) continue;

            if (element.IsYou(subgroup)) return true;

            if (recursively)
            {
                element.Subgroup.ContainsSubgroup(subgroup, true);
            }
        }

        return false;
    }

    public bool ContainsTags(bool recursively)
    {
        foreach (Element element in elements)
        {
            if (element.IsTag) return true;

            if (recursively && element.IsSubgroup)
            {
                element.Subgroup.ContainsTags(true);
            }
        }

        return false;
    }

    public bool ContainsSubgroups()
    {
        foreach (Element element in elements)
        {
            if (element.IsSubgroup) return true;
        }

        return false;
    }

    public List<ContentItemsGroup> GetAllSubgroups(bool recursively)
    {
        List<ContentItemsGroup> resList = new List<ContentItemsGroup>();
        foreach (Element element in elements)
        {
            if (!element.IsSubgroup) continue;

            resList.Add(element.Subgroup);

            if (recursively)
            {
                resList.AddRange(element.Subgroup.GetAllSubgroups(true));
            }
        }

        return resList;
    }

    public List<string> GetAllTags(bool recursively)
    {
        List<string> resList = new List<string>();
        foreach (Element element in elements)
        {
            if (element.IsSubgroup)
            {
                if (recursively)
                {
                    resList.AddRange(element.Subgroup.GetAllTags(true));
                }
            }
            else
            {
                resList.Add(element.Tag);
            }
        }

        return resList;
    }
}
