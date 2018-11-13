package com.example.kip.pickem_droid.services

import android.support.v4.app.Fragment
import android.support.v4.app.FragmentManager
import android.support.v4.app.FragmentPagerAdapter
import com.example.kip.pickem_droid.PlayerScoreboardFragment
import com.example.kip.pickem_droid.SeasonScoreboardFragment
import com.example.kip.pickem_droid.WeekScoreboardFragment

class ScoreboardTabPagerAdapter(fm: FragmentManager, private var tabCount: Int) :
        FragmentPagerAdapter(fm)
{

    override fun getItem(position: Int): Fragment?
    {
        when (position) {
            0 -> return PlayerScoreboardFragment()
            1 -> return WeekScoreboardFragment()
            2 -> return SeasonScoreboardFragment()
            else -> return null
        }
    }

    override fun getCount(): Int {
        return tabCount
    }
}