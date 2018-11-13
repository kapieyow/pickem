package com.example.kip.pickem_droid

import android.support.v7.app.AppCompatActivity
import android.os.Bundle
import android.support.design.widget.TabLayout
import android.support.v4.view.ViewPager
import android.view.Menu
import com.example.kip.pickem_droid.R.layout.activity_scoreboards
import com.example.kip.pickem_droid.services.ScoreboardTabPagerAdapter
import kotlinx.android.synthetic.main.activity_scoreboards.*


class ScoreboardsActivity : AppCompatActivity() {

    private lateinit var viewPager: ViewPager

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(activity_scoreboards)
        setSupportActionBar(toolbar)

        tab_layout.addTab(tab_layout.newTab().setText("Player"))
        tab_layout.addTab(tab_layout.newTab().setText("Week"))
        tab_layout.addTab(tab_layout.newTab().setText("Season"))
        tab_layout.tabGravity = TabLayout.GRAVITY_FILL

        val adapter = ScoreboardTabPagerAdapter(supportFragmentManager, tab_layout.tabCount)

        viewPager = findViewById(R.id.pager)
        viewPager.adapter = adapter
        viewPager.addOnPageChangeListener(TabLayout.TabLayoutOnPageChangeListener(tab_layout))

        tab_layout.setOnTabSelectedListener(object : TabLayout.OnTabSelectedListener {
            override fun onTabSelected(tab: TabLayout.Tab) {
                viewPager.currentItem = tab.position
            }
            override fun onTabUnselected(tab: TabLayout.Tab) { /*nop*/ }
            override fun onTabReselected(tab: TabLayout.Tab) { /*nop*/ }
        })
    }

    override fun onCreateOptionsMenu(menu: Menu?): Boolean {
        // Inflate the menu; this adds items to the action bar if it is present.
        menuInflater.inflate(R.menu.options_menu, menu)
        return true
    }
}
