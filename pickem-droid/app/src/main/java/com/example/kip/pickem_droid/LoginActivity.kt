package com.example.kip.pickem_droid

import android.animation.Animator
import android.animation.AnimatorListenerAdapter
import android.annotation.TargetApi
import android.support.v7.app.AppCompatActivity
import android.os.Build
import android.os.Bundle
import android.text.TextUtils
import android.view.View
import android.view.inputmethod.EditorInfo
import android.widget.TextView
import com.example.kip.pickem_droid.services.AuthService
import com.example.kip.pickem_droid.services.models.UserLoggedIn
import com.google.gson.Gson
import io.reactivex.android.schedulers.AndroidSchedulers
import io.reactivex.schedulers.Schedulers

import kotlinx.android.synthetic.main.activity_login.*
import okhttp3.ResponseBody
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

        password_textbox.setOnEditorActionListener(
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

        login_button.setOnClickListener { attemptLogin() }
    }


    /**
     * Attempts to sign in or register the account specified by the login form.
     * If there are form errors (invalid email, missing fields, etc.), the
     * errors are presented and no actual login attempt is made.
     */
    private fun attemptLogin()
    {
        // Reset errors.
        username_textbox.error = null
        password_textbox.error = null

        // Store values at the time of the login attempt.
        val username = username_textbox.text.toString()
        val password = password_textbox.text.toString()

        var cancel = false
        var focusView: View? = null

        if ( TextUtils.isEmpty(username) )
        {
            username_textbox.error = "User Name is required"
            focusView = username_textbox
            cancel = true
        }

        if ( TextUtils.isEmpty(password) )
        {
            password_textbox.error = "Password is required"
            focusView = password_textbox
            cancel = true
        }

        if (cancel)
        {
            focusView?.requestFocus()
        }
        else
        {
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
    }

    private fun showServiceError(error: Throwable)
    {
        if ( error is HttpException )
        {
            if ( error.code() == 401 )
            {
                // not authorized, not a error, bad user or password
                lblTempOut.text = "User name and password are not valid"
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
                    var json = JSONObject(errorBody?.toString())
                    lblTempOut.text = "HMMMMMMMMMM"
                }
                catch (e: Exception)
                {
                    lblTempOut.text = error.message
                }
            }
        }
        else
        {
            lblTempOut.text = error.message
        }
        showProgress(false)
    }

    private fun successfulLogin(userLoggedIn: UserLoggedIn)
    {
        lblTempOut.text = """
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

        login_progress.visibility = if (show) View.VISIBLE else View.GONE
        login_progress
            .animate()
            .setDuration(shortAnimTime)
            .alpha((if (show) 1 else 0).toFloat())
            .setListener(object : AnimatorListenerAdapter() {
                override fun onAnimationEnd(animation: Animator) {
                    login_progress.visibility = if (show) View.VISIBLE else View.GONE
                }
            })

    }

}
