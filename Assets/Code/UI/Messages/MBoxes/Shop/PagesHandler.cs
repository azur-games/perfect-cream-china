using System;
using System.Collections.Generic;
using UnityEngine;


public class PagesHandler : MonoBehaviour
{
    #region Fields

    [SerializeField] GameObject pageDotPrefab;


    List<PageDot> activePages = new List<PageDot>();
    Stack<PageDot> dotsPool = new Stack<PageDot>();


    PageDot currentPage = null;
    int currentPageIndex = -1;

    #endregion 



    #region Properties

    public int CurrentPage
    {
        get => currentPageIndex;
        set
        {
            SwitchPage(value);
        }
    }


    public int PageMin => 0;


    public int PageMax => activePages.Count - 1;

    #endregion



    #region Unity lifecycle

    void OnDestroy()
    {
        foreach (var activePage in activePages)
        {
            Destroy(activePage.gameObject);
        }

        while (dotsPool.Count > 0)
        {
            PageDot dot = dotsPool.Pop();
            Destroy(dot.gameObject);
        }
    }

    #endregion



    #region Initialization

    public void Init(int pagesNum, int currentPage)
    {
        while (activePages.Count < pagesNum)
        {
            activePages.Add(GetPageDotFromPool());
        }

        while (activePages.Count > 0 && activePages.Count > pagesNum)
        {
            PageDot pageDot = activePages.First();
            activePages.RemoveAt(0);
            MovePageToPool(pageDot);
        }

        foreach (var page in activePages)
        {
            page.State = PageDotState.Inactive;
        }

        CurrentPage = currentPage;

        bool isMultiplePages = (activePages.Count > 1);
        gameObject.SetActive(isMultiplePages);
    }

    #endregion


    #region Pages switching

    void SwitchPage(int pageIndex)
    {
        int activePagesCount = activePages.Count;

        if (currentPage != null)
        {
            currentPage.State = PageDotState.Inactive;
        }

        currentPageIndex = pageIndex;
        currentPageIndex = (currentPageIndex < activePagesCount) ? currentPageIndex : activePagesCount - 1;
        currentPageIndex = (currentPageIndex >= 0) ? currentPageIndex : 0;

        if (activePagesCount > 0)
        {
            currentPage = activePages[currentPageIndex];
            currentPage.State = PageDotState.Active;
        }
        else
        {
            currentPage = null;
        }
    }

    #endregion



    #region Pool managment

    PageDot GetPageDotFromPool()
    {
        PageDot result = null;

        if (dotsPool.Count > 0)
        {
            result = dotsPool.Pop();
            result.gameObject.SetActive(true);
            result.gameObject.transform.SetParent(gameObject.transform);
        }
        else
        {
            result = Instantiate(pageDotPrefab, gameObject.transform).GetComponent<PageDot>();
        }

        return result;
    }


    void MovePageToPool(PageDot pageDot)
    {
        pageDot.gameObject.transform.SetParent(null);
        pageDot.gameObject.SetActive(false);
        pageDot.State = PageDotState.Inactive;

        dotsPool.Push(pageDot);
    }

    #endregion
}