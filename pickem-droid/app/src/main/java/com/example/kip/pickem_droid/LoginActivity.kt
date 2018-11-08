package com.example.kip.pickem_droid

import android.animation.Animator
import android.animation.AnimatorListenerAdapter
import android.annotation.TargetApi
import android.support.v7.app.AppCompatActivity
import android.os.Build
import android.os.Bundle
import android.view.View
import android.view.inputmethod.EditorInfo
import android.widget.TextView
import com.example.kip.pickem_droid.services.AuthService
import com.example.kip.pickem_droid.services.models.UserLoggedIn
import io.reactivex.android.schedulers.AndroidSchedulers
import io.reactivex.schedulers.Schedulers

import kotlinx.android.synthetic.main.activity_login.*
import okhttp3.ResponseBody
import org.json.JSONArray
import org.json.JSONObject
import retrofit2.HttpException

/**
 * A login screen that offers login via email/password.
 */
class LoginActivity : AppCompatActivity() {

    private val _authService = AuthService()

    override fun onCreate(savedInstanceState: Bundle?)
    {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_login)
        // Set up the login form.

        txtPassword.setOnEditorActionListener(
            TextView.OnEditorActionListener
            { _, id, _ ->
                if (id == EditorInfo.IME_ACTION_DONE || id == EditorInfo.IME_NULL)
                {
                    attemptLogin()
                    return@OnEditorActionListener true
                }
            false
            }
        )

        btnLogin.setOnClickListener { attemptLogin() }
    }


    /**
     * Attempts to sign in or register the account specified by the login form.
     * If there are form errors (invalid email, missing fields, etc.), the
     * errors are presented and no actual login attempt is made.
     */
    private fun attemptLogin()
    {
        // clear any previous issues
        lblErrorMessages.text = ""

        // Store values at the time of the login attempt.
        val username = txtUsername.text.toString()
        val password = txtPassword.text.toString()

        // Show a progress spinner, and kick off a background task to
        // perform the user login attempt.
        showProgress(true)
        _authService.startLogin(username, password)
            .subscribeOn(Schedulers.io())
            .observeOn(AndroidSchedulers.mainThread())
            .subscribe(
                { result -> successfulLogin(result) },
                { error -> showServiceError(error) }
            )
    }

    private fun showServiceError(error: Throwable)
    {
        if ( error is HttpException )
        {
            if ( error.code() == 401 )
            {
                // not authorized, not a error, bad user or password
                lblErrorMessages.text = "User name and password are not valid"
            }
            else if ( error.code() == 400 )
            {
                // bad request, more detail in the body
                // try to parse it out

                try
                {
                    // if the server sent back structured data, it will be first level properties each with an array of messages.
                    // all we care about here is the messages
                    var errorBody: ResponseBody? = error.response().errorBody()
                    var errorJsonAsString = errorBody?.string()
                    var errorJson = JSONObject(errorJsonAsString)

                    var outputMessages: String = ""

                    for ( key in errorJson.keys() )
                    {
                        // ASSuming value is an array
                        var messages = (errorJson[key] as JSONArray)

                        for ( i in 0..messages.length() - 1 )
                        {
                            outputMessages = "${outputMessages}\n${messages[i]}"
                        }
                    }

                    lblErrorMessages.text = outputMessages;
                }
                catch (e: Exception)
                {
                    lblErrorMessages.text = error.message
                }
            }
        }
        else
        {
            lblErrorMessages.text = error.message
        }
        showProgress(false)
    }

    private fun successfulLogin(userLoggedIn: UserLoggedIn)
    {
        lblErrorMessages.text = """
            defaultLeagueCode: ${userLoggedIn.defaultLeagueCode}
            email: ${userLoggedIn.email}
            token: ${userLoggedIn.token}
            userName: ${userLoggedIn.userName}
            leagues: ${userLoggedIn.leagues}
        """.trimIndent()
        showProgress(false)
    }


    /**
     * Shows the progress UI and hides the login form.
     */
    @TargetApi(Build.VERSION_CODES.HONEYCOMB_MR2)
    private fun showProgress(show: Boolean) {

        val shortAnimTime = resources.getInteger(android.R.integer.config_shortAnimTime).toLong()

        pbLoginProgress.visibility = if (show) View.VISIBLE else View.GONE
        pbLoginProgress
            .animate()
            .setDuration(shortAnimTime)
            .alpha((if (show) 1 else 0).toFloat())
            .setListener(object : AnimatorListenerAdapter() {
                override fun onAnimationEnd(animation: Animator) {
                    pbLoginProgress.visibility = if (show) View.VISIBLE else View.GONE
                }
            })

    }

}
