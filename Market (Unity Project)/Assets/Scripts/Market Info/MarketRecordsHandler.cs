using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarketRecordsHandler : MonoBehaviour
{
    public static MarketRecordsHandler instance;

    public List<MarketRowWithNames> marketRows;
    [Header("Ui Components")]
    public List<MaskableGraphic> backgrounds;
    public Color turnedOnBackgroundColor;
    public Color turnedOffBackgroundColor;
    [Space(15)]
    public List<MaskableGraphic> texts;
    public Color turnedOnTextColor;
    public Color turnedOffTextColor;
    [Space(15)]
    public Image ascendingDescendingImage;
    public Sprite ascendingSprite;
    public Sprite descendingSprite;
    [Space(15)]
    public Text infoText;
    public string beforeRowCountText;
    public string afterRowCountBeforeInfoText;
    public List<string> columnSortingTexts;
    public string byAscendingInfoText;
    public string byDescendingInfoText;
    [Space(15)]
    public Text allowAllRecordsText;
    public string allRecordsAllowed;
    public string onlyOwnedRecordsAllowed;

    private int backgroundsCount;
    private int sortingColumn = 0;
    private bool byAscending = true;
    private bool allowAllRecords = true;
    private int ownedCount = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            marketRows = new List<MarketRowWithNames>();
            backgroundsCount = backgrounds.Count;
            RefreshUIComponents();
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying component");
            Destroy(this);
        }
    }

    public void SortAndShow()
    {
        List<MarketRowWithNames> sortedList;
        switch (sortingColumn)
        {
            case 0: // По имени предмета
                {
                    if (byAscending) sortedList = marketRows.OrderBy(s => s.itemName).ToList();
                    else sortedList = marketRows.OrderByDescending(s => s.itemName).ToList();
                    break;
                }
            case 1: // По типу предмета
                {
                    if (byAscending) sortedList = marketRows.OrderBy(s => s.typeName).ToList();
                    else sortedList = marketRows.OrderByDescending(s => s.typeName).ToList();
                    break;
                }
            case 2: // По имени продавца
                {
                    if (byAscending) sortedList = marketRows.OrderBy(s => s.nickname).ToList();
                    else sortedList = marketRows.OrderByDescending(s => s.nickname).ToList();
                    break;
                }
            case 3: // По количеству
                {
                    if (byAscending) sortedList = marketRows.OrderBy(s => s.quantity).ToList();
                    else sortedList = marketRows.OrderByDescending(s => s.quantity).ToList();
                    break;
                }
            case 4: // По цене за штуку
                {
                    if (byAscending) sortedList = marketRows.OrderBy(s => s.priceForUnit).ToList();
                    else sortedList = marketRows.OrderByDescending(s => s.priceForUnit).ToList();
                    break;
                }
            default:
                {
                    sortedList = new List<MarketRowWithNames>();
                    break;
                }
        }

        UiManager.instance.ClearChildren(UiManager.instance.marketRowScrollViewContent);
        int playerId = CurrentPlayer.instance.playerId;
        ownedCount = 0;
        foreach (MarketRowWithNames s in sortedList)
        {
            if (s.playerId == playerId)
            {
                UiManager.instance.AddMarketRowToScrollView(s, true);
                ownedCount++;
            }
            else
            {
                if (allowAllRecords) UiManager.instance.AddMarketRowToScrollView(s, false);
            }
        }
        RefreshInfoText();
    }

    public void SetSortingColumn(int value)
    {
        if (value != sortingColumn)
        {
            sortingColumn = value;
            byAscending = true;
        }
        else
        {
            byAscending = !byAscending;
        }
        RefreshUIComponents();
        SortAndShow();
    }

    private void RefreshUIComponents()
    {
        for (int i = 0; i < backgroundsCount; ++i)
        {
            if (i != sortingColumn)
            {
                backgrounds[i].color = turnedOffBackgroundColor;
                texts[i].color = turnedOffTextColor;
            }
            else
            {
                backgrounds[i].color = turnedOnBackgroundColor;
                texts[i].color = turnedOnTextColor;
            }
        }
        RefreshAscendingDescendingImage();
        RefreshAllowedRecordsButtonText();
    }

    public void ChangeSortingOrder()
    {
        byAscending = !byAscending;
        RefreshAscendingDescendingImage();
        SortAndShow();
    }

    private void RefreshAscendingDescendingImage()
    {
        if (byAscending) ascendingDescendingImage.sprite = ascendingSprite;
        else ascendingDescendingImage.sprite = descendingSprite;
    }

    private void RefreshInfoText()
    {
        infoText.text = beforeRowCountText +
            ' ' + (allowAllRecords ? marketRows.Count : ownedCount) +
            ' ' + afterRowCountBeforeInfoText +
            ' ' + columnSortingTexts[sortingColumn] +
            ' ' + (byAscending ? byAscendingInfoText : byDescendingInfoText) +
            '.';
    }

    public void ChangeMarketRecordShowed()
    {
        allowAllRecords = !allowAllRecords;
        RefreshAllowedRecordsButtonText();
        SortAndShow();
    }

    private void RefreshAllowedRecordsButtonText()
    {
        if (allowAllRecords) allowAllRecordsText.text = allRecordsAllowed;
        else allowAllRecordsText.text = onlyOwnedRecordsAllowed;
    }
}
