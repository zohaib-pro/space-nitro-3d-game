using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject shipContainer;


    public GameObject nextBtn, prevBtn, selectBtn, watchAdBtn, coinAdditionAnim;

    public Text selectBtnText, coinText, msgText, adMsgText;

    public AudioSource shipBuySound, coinAddSound;

    public GameObject msgObject;

    private int shipsCount;
    private int pos = 0; //current displaying prefab
    private Transform shipContainerTransform;

    private int[] shipPrices =
    {
        0, 250, 500, 1250, 4500
    };

    void Start()
    {
        shipContainerTransform =  shipContainer.transform;
        shipsCount = shipContainerTransform.childCount;
        pos = getSelectedShip();
     
        refresh();
    }


    public void prev()
    {
        pos--;
        refresh();
    }

    public void next() {
        pos++;
        refresh();
    }

    public void select()
    {
        //if ship already owned by player, then just select it
        if (PrefManager.isShipOwned(pos))
        {
            PrefManager.SetSelectedShip(pos);

        }
        else
        {
            /*
             * Reaches here if ship isn't owned by player
             * here are two options, either player has enough money to buy it or now
             * if money is enough then buy the current ship
             * otherwise, just show a message about not enough money
             */
    
            int shipPrice = shipPrices[pos];
            if (shipPrice <= PrefManager.GetPlayerCoins())
            {
                PrefManager.SetShipOwned(pos);
                PrefManager.DeductPlayerCoins(shipPrice);
                shipBuySound.Play();
            }
            else
            {
                //not enough money here, but user pressed to buy
                OnUserPressedCoinAddBtn(); //same functionality

            }
        }
        
        refresh();

    }

    public void refresh()
    {
        for (int i = 0; i < shipsCount; i++)
        {
            GameObject ship = shipContainerTransform.GetChild(i).gameObject;
            ship.SetActive(i == pos);
        }

        prevBtn.SetActive(pos != 0);
        nextBtn.SetActive(pos != shipsCount - 1);

        if (pos == getSelectedShip())
        {
            selectBtnText.text = "selected";
        }
        else if (!PrefManager.isShipOwned(pos))
        {
            selectBtnText.text = "buy $" + shipPrices[pos];
        }
        else
        {
            selectBtnText.text = "select";
        }
        
        coinText.text = "" + PrefManager.GetPlayerCoins();
    }

    public void OnUserPressedCoinAddBtn()
    {
        selectBtn.SetActive(false);
        bool canWatch = PrefManager.CanWatchAd();
        watchAdBtn.GetComponent<Button>().interactable = canWatch;
        msgObject.SetActive(true);
        if (!canWatch)
        {
            adMsgText.text = "You already watched ads! try tomorrow!";
        }
    }

    public void onUserPressedWatchAds()
    {
        //don't allow to press again until fist ad is loaded
        watchAdBtn.GetComponent<Button>().interactable = false;
        adMsgText.text = "Loading Ad...";
        AdManager adManager = GetComponent<AdManager>();
        adManager.LoadRewardedAd((isSuccess) =>
        {
            if (isSuccess)
            {
                adManager.ShowRewardedAd(() =>
                {
                    coinAdditionAnim.SetActive(true);
                    coinAddSound.Play();
                    
                    Invoke(nameof(OnUserRewarded), 1f);
                    onUserPressedSkip();
                });
                watchAdBtn.GetComponent<Button>().interactable = true;
            }
            else
            {
                adMsgText.text = "Failed to load ad! Try again later";
            }
                
        });
    }

    private void OnUserRewarded()
    {
        selectBtn.SetActive(true);
        PrefManager.AddPlayerCoins(25);
        refresh();
        PrefManager.SetLastAdWatchTime();
        coinAdditionAnim.SetActive(false);
    }

    public void onUserPressedSkip()
    {
        selectBtn.SetActive(true);
        msgObject.SetActive(false);
    }


    private int getSelectedShip()
    {
        return PrefManager.GetSelectedShip();
    }
}
