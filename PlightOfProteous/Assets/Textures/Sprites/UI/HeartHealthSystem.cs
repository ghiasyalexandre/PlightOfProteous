using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartHealthSystem
{
    public const int MAX_FRAGMENT_AMOUNT = 2;

    public event EventHandler OnDamaged;
    public event EventHandler OnHealed;
    public event EventHandler OnDead;

    private List<Heart> heartList;

    public HeartHealthSystem(int heartAmount)
    {
        heartList = new List<Heart>();
        for (int i = 0; i < heartAmount; i++)
        {
            Heart heart = new Heart(2);
            heartList.Add(heart);
        }
    }

    public List<Heart> GetHeartList()
    {
        return heartList;
    }

    public void Damage(int damageAmount)
    {
        // Cycles through all hearts starting from end
        for (int i = heartList.Count - 1; i >= 0; i--)
        {
            Heart heart = heartList[i];
            // Test if this heart can absorb damageAmount
            if (damageAmount > heart.GetFragmentAmount())
            {
                //Heart cannot absorb full damageAmount goto next Heart
                damageAmount -= heart.GetFragmentAmount();
                heart.Damage(heart.GetFragmentAmount());

            }
            else
            {
                // Heart can absorb full damageAmount then break
                heart.Damage(damageAmount);
                break;
            }
        }

        if (OnDamaged != null) OnDamaged(this, EventArgs.Empty);

        if (IsDead())
        {
            if (OnDead != null) OnDead(this, EventArgs.Empty);
        }
    }

    public void Heal(int healAmount)
    {
        for (int i = 0; i < heartList.Count; i++)
        {
            Heart heart = heartList[i];
            int missingFragments = MAX_FRAGMENT_AMOUNT - heart.GetFragmentAmount();

            if (healAmount > missingFragments)
            {
                healAmount -= missingFragments;
                heart.Heal(missingFragments);
            }
            else
            {
                heart.Heal(healAmount);
                break;
            }
        }
        
        if (OnHealed != null) OnHealed(this, EventArgs.Empty);
    }


    public bool IsDead()
    {
        return heartList[0].GetFragmentAmount() == 0;
    }

    public bool IsFullHp()
    {
        return heartList[heartList.Count - 1].GetFragmentAmount() == MAX_FRAGMENT_AMOUNT;
    }

    // Represents a single Heart
    public class Heart
    {
        private int fragments;
        public Heart(int fragments)
        {
            this.fragments = fragments;
        }

        public void SetFragments(int fragments)
        {
            this.fragments = fragments;
        }

        public int GetFragmentAmount()
        {
            return fragments;
        }

        public void Damage(int damageAmount)
        {
            if (damageAmount >= fragments)
            {
                fragments = 0;
            }
            else
            {
                fragments -= damageAmount;
            }
        }

        public void Heal(int healAmount)
        {
            if (fragments + healAmount > MAX_FRAGMENT_AMOUNT)
            {
                fragments = MAX_FRAGMENT_AMOUNT;
            }
            else
            {
                fragments += healAmount;
            }
        }
    }
}
