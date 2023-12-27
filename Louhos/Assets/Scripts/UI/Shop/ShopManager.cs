using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private Animator shopAnimator;

    [Header("Shop Pages")]
    [SerializeField] private GameObject toolsPage;
    [SerializeField] private GameObject upgradesPage;
    [SerializeField] private GameObject consumablesPage;
    [SerializeField] private GameObject nextDayPage;

    [Header("Shop pagination")]
    [SerializeField] private GameObject nextPageButton;
    [SerializeField] private GameObject previousPageButton;
    [SerializeField] private Sprite PaginationNormalSprite;
    [SerializeField] private Sprite PaginationHoverSprite;
    [SerializeField] private float pageTurnDuration = 0.25f;

    public UnityAction OnPageChange;

    // Unity event called from shop next button
    public void CancelHoverPaginationButton()
    {
        // We only have 2 pages and only 1 pagination arrow is ever visible, so this is a quick and dirty way to handle this, will need to be redone if 3rd shop page is added
        nextPageButton.GetComponent<Image>().sprite = PaginationNormalSprite;
        previousPageButton.GetComponent<Image>().sprite = PaginationNormalSprite;
    }


    // Unity event called from shop next button
    public void HoverPaginationButton()
    {
        // We only have 2 pages and only 1 pagination arrow is ever visible, so this is a quick and dirty way to handle this, will need to be redone if 3rd shop page is added
        nextPageButton.GetComponent<Image>().sprite = PaginationHoverSprite;
        previousPageButton.GetComponent<Image>().sprite = PaginationHoverSprite;
    }


    // Unity event called from shop next button
    public void TurnNextPage() => StartCoroutine(TurnNextPageCoroutine());
    private IEnumerator TurnNextPageCoroutine()
    {
        nextPageButton.SetActive(false);
        toolsPage.SetActive(false);
        upgradesPage.SetActive(false);
        CancelHoverPaginationButton();
        shopAnimator.SetTrigger("Turn Next Page");
        OnPageChange?.Invoke();

        yield return new WaitForSeconds(pageTurnDuration);

        previousPageButton.SetActive(true);
        consumablesPage.SetActive(true);
        nextDayPage.SetActive(true);
    }


    // Unity event called from shop previous button
    public void TurnPreviousPage() => StartCoroutine(TurnPreviousPageCoroutine());
    private IEnumerator TurnPreviousPageCoroutine()
    {
        previousPageButton.SetActive(false);
        consumablesPage.SetActive(false);
        nextDayPage.SetActive(false);
        CancelHoverPaginationButton();
        shopAnimator.SetTrigger("Turn Previous Page");
        OnPageChange?.Invoke();

        yield return new WaitForSeconds(pageTurnDuration);

        nextPageButton.SetActive(true);
        toolsPage.SetActive(true);
        upgradesPage.SetActive(true);
    }


    public void ResetShopUI()
    {
        previousPageButton.SetActive(false);
        consumablesPage.SetActive(false);
        nextDayPage.SetActive(false);
        CancelHoverPaginationButton();
        shopAnimator.SetTrigger("Turn Previous Page");
        nextPageButton.SetActive(true);
        toolsPage.SetActive(true);
        upgradesPage.SetActive(true);
    }
}