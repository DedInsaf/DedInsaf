using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

public class CameraController : MonoBehaviour
{
    private InterstitialAd interstitialAd;

    private const string interstitialUnitId = "ca-app-pub-3940256099942544/8691691433";

    [SerializeField]
    private float speed = 5.0F;

    [SerializeField]
    private Transform target;

    private void OnEnable()
    {
        interstitialAd = new InterstitialAd(interstitialUnitId);
        AdRequest adRequest = new AdRequest.Builder().Build();
        interstitialAd.LoadAd(adRequest);
    }
    
    public void ShowAd()
    {
        if (interstitialAd.IsLoaded())
        {
            interstitialAd.Show();
        }
    }

    private void Awake()
    {
        if (!target) target = FindObjectOfType<Player>().transform;
    }

    private void Update()
    {
        Vector3 position = target.position;         position.z = -10.0F;

        transform.position = Vector3.Lerp(transform.position, position, speed * Time.deltaTime); //Lerp - сгживание движения
    }
}
