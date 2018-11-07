package com.example.kip.pickem_droid.services.models

data class UserLoggedIn (
    val defaultLeagueCode: String,
    val email: String,
    val token: String,
    val userName: String,
    val leagues: List<League>
);