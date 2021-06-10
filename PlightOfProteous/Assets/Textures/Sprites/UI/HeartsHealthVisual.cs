using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class HeartsHealthVisual : MonoBehaviour
{
    public static HeartHealthSystem heartHealthSystemStatic;

    [SerializeField] Sprite emptyHeartSprite;
    [SerializeField] Sprite halfHeartSprite;
    [SerializeField] Sprite fullHeartSprite;
    [SerializeField] Sprite blueHalfHeartSprite;
    [SerializeField] Sprite blueFullHeartSprite;
    [SerializeField] AnimationClip heartFullAnimationClip;

    private HeartHealthSystem heartHealthSystem;
    private List<HeartImage> heartImageList;
    private bool isHealing;

    private float startTime;
    private float timeBtwCheck = 0.2f;

    private void Awake()
    {
        heartImageList = new List<HeartImage>();
    }

    void Start()
    {
        startTime = timeBtwCheck;
        HeartHealthSystem heartHealthSystem = new HeartHealthSystem(10);
        SetHeartsHealthSystem(heartHealthSystem);
    }

    private void Update()
    {
        if (startTime <= 0)
        {
            HealingAnimatedPeriodic();
            startTime = timeBtwCheck;
        }
        else
        {
            startTime -= Time.deltaTime;
        }
    }

    public void SetHeartsHealthSystem(HeartHealthSystem heartHealthSystem)
    {
        this.heartHealthSystem = heartHealthSystem;
        heartHealthSystemStatic = heartHealthSystem;

        List<HeartHealthSystem.Heart> heartList = heartHealthSystem.GetHeartList();
        int row = 0;
        int col = 0;
        int colMax = 5;
        float rowColSize = 45f;

        for(int i = 0; i < heartList.Count; i++)
        {
            HeartHealthSystem.Heart heart = heartList[i];
            Vector2 heartAnchoredPosition = new Vector2(col * rowColSize, -row * rowColSize);
            CreateHeartImage(heartAnchoredPosition).SetHeartFragments(heart.GetFragmentAmount());

            col++;
            if (col >= colMax)
            {
                row++;
                col = 0;
            }
        }

        heartHealthSystem.OnDamaged += HeartHealthSystem_OnDamaged;
        heartHealthSystem.OnHealed += HeartHealthSystem_OnHealed;
        heartHealthSystem.OnDead += HeartHealthSystem_OnDead;
    }

    private void HeartHealthSystem_OnDamaged(object sender, System.EventArgs e)
    {
        RefreshHearts();
    }

    private void HeartHealthSystem_OnHealed(object sender, System.EventArgs e)
    {
        //RefreshHearts();
        isHealing = true;
    }

    private void HeartHealthSystem_OnDead(object sender, System.EventArgs e)
    {
        Debug.Log("Player is Dead");
    }

    private void RefreshHearts()
    {
        // Hearts health system was damaged
        List<HeartHealthSystem.Heart> heartList = heartHealthSystem.GetHeartList();
        for (int i = 0; i < heartImageList.Count; i++)
        {
            HeartImage heartImage = heartImageList[i];
            HeartHealthSystem.Heart heart = heartList[i];
            heartImage.SetHeartFragments(heart.GetFragmentAmount());
        }
    }

    private void HealingAnimatedPeriodic()
    {
        if (isHealing)
        {
            bool fullyHealed = true;
            List<HeartHealthSystem.Heart> heartList = heartHealthSystem.GetHeartList();
            for (int i = 0; i < heartList.Count; i++)
            {
                HeartImage heartImage = heartImageList[i];
                HeartHealthSystem.Heart heart = heartList[i];
                if (heartImage.GetFragmentAmount() != heart.GetFragmentAmount())
                {
                    // Visual is different from logic
                    heartImage.AddHeartVisualFragment();
                    if (heartImage.GetFragmentAmount() == HeartHealthSystem.MAX_FRAGMENT_AMOUNT)
                    {
                        // Heart was fully healed
                        heartImage.PlayHeartFullAnimation();
                    }
                    fullyHealed = false;
                    break;
                }
            }

            if (fullyHealed)
            {
                isHealing = false;
            }
        }
    }

    private HeartImage CreateHeartImage(Vector2 anchoredPosition)
    {
        GameObject heartGameObject = new GameObject("Heart", typeof(Image), typeof(Animation));
        //heartGameObject.transform.parent = transform;
        heartGameObject.transform.SetParent(transform, false);
        heartGameObject.GetComponent<Image>().sprite = fullHeartSprite;
        heartGameObject.transform.localPosition = Vector3.zero;

        // Set as child of this transform
        heartGameObject.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;
        heartGameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 50);

        heartGameObject.GetComponent<Animation>().AddClip(heartFullAnimationClip, "HeartFull");
       
        // Locate and size heart
        Image heartImageUI = heartGameObject.GetComponent<Image>();
        heartImageUI.sprite = fullHeartSprite;
        HeartImage heartImage = new HeartImage(this, heartImageUI, heartGameObject.GetComponent<Animation>());
        heartImageList.Add(heartImage);

        return heartImage;
    }

    public class HeartImage
    {
        private int fragments;
        private Image heartImage;
        private Animation animation;
        private HeartsHealthVisual heartsHealthVisual;
        public HeartImage(HeartsHealthVisual heartsHealthVisual, Image heartImage, Animation animation)
        {
            this.heartsHealthVisual = heartsHealthVisual;
            this.heartImage = heartImage;
            this.animation = animation;
        }

        public void SetHeartFragments(int fragments)
        {
            this.fragments = fragments;
            switch(fragments)
            {
                case 0: heartImage.sprite = heartsHealthVisual.emptyHeartSprite; break;
                case 1: heartImage.sprite = heartsHealthVisual.halfHeartSprite; break;
                case 2: heartImage.sprite = heartsHealthVisual.fullHeartSprite; break;
                //case 3: heartImage.sprite = heartsHealthVisual.blueHalfHeartSprite; break;
                //case 4: heartImage.sprite = heartsHealthVisual.blueFullHeartSprite; break;
            }
        }

        public int GetFragmentAmount()
        {
            return fragments;
        }

        public void AddHeartVisualFragment()
        {
            SetHeartFragments(fragments + 1);
        }

        public void PlayHeartFullAnimation()
        {
            animation.Play("HeartFull", PlayMode.StopAll);
        }
    }
}
