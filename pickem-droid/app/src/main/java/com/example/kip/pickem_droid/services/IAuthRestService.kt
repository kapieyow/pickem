package com.example.kip.pickem_droid.services

import com.example.kip.pickem_droid.services.models.UserCredentials
import com.example.kip.pickem_droid.services.models.UserLoggedIn
import io.reactivex.Observable
import retrofit2.http.Body
import retrofit2.http.POST

interface IAuthRestService
{

    @POST("auth/login")
    fun login(@Body userCredentials: UserCredentials) : Observable<UserLoggedIn>

}