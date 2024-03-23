using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class PlayFabManager : MonoBehaviour
{
    private const string EMAIL_REGEX = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
    private const string TITLE_ID = "9ECA5";

    [Header("UI")]
    public Text ValidationMessage;
    public TMP_InputField EmailInput;
    public TMP_InputField UsernameInput;
    public TMP_InputField PasswordInput;
    public TMP_InputField PasswordConfirmationInput;

    public GetAccountInfoResult AccountInfo;

    public void Register()
    {
        var email = EmailInput.text.Trim();
        var username = UsernameInput.text.Trim();
        var password = PasswordInput.text.Trim();
        var confirmation = PasswordConfirmationInput.text.Trim();

        if (!password.Equals(confirmation))
        {
            ValidationMessage.text = "Passwords don't match.";
            PasswordInput.text = string.Empty;
            PasswordConfirmationInput.text = string.Empty;
            return;
        }

        if (password.Length < 6)
        {
            ValidationMessage.text = "Password length should be more than 6.";
            PasswordInput.text = string.Empty;
            PasswordConfirmationInput.text = string.Empty;
            return;
        }

        if (!Regex.IsMatch(email, EMAIL_REGEX))
        {
            ValidationMessage.text = "Incorrect email format.";
            PasswordInput.text = string.Empty;
            PasswordConfirmationInput.text = string.Empty;
            return;
        }

        var registerRequest = new RegisterPlayFabUserRequest { Email = email, Username = username, Password = password, RequireBothUsernameAndEmail = false };
        PlayFabClientAPI.RegisterPlayFabUser(registerRequest, OnRegisterSuccess, OnRegisterFailure);
    }

    public string Login()
    {
        var email = EmailInput.text.Trim();
        var password = PasswordInput.text.Trim();

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ValidationMessage.text = "Email and password are required.";
            return null;
        }

        if (!Regex.IsMatch(email, EMAIL_REGEX))
        {
            ValidationMessage.text = "Incorrect email format.";
            return null;
        }

        Login(email, password);
        return this.AccountInfo.AccountInfo.Username;
    }

    public void RequestPasswordReset()
    {
        var email = EmailInput.text.Trim();

        if (string.IsNullOrEmpty(email))
        {
            ValidationMessage.text = "Email is required for password reset.";
            return;
        }

        if (!Regex.IsMatch(email, EMAIL_REGEX))
        {
            ValidationMessage.text = "Incorrect email format.";
            return;
        }

        var request = new SendAccountRecoveryEmailRequest
        {
            Email = email,
            TitleId = TITLE_ID,
        };

        PlayFabClientAPI.SendAccountRecoveryEmail(request, OnPasswordResetEmailSent, OnPasswordResetFailure);
    }

    #region OnSuccess

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        this.Login(EmailInput.text, PasswordInput.text);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        var request = new GetAccountInfoRequest() { PlayFabId = result.PlayFabId };
        PlayFabClientAPI.GetAccountInfo(request, (GetAccountInfoResult info) => this.AccountInfo = info, OnGetAccountInfoFailure);
    }

    private void OnPasswordResetEmailSent(SendAccountRecoveryEmailResult result)
    {
        ValidationMessage.text = "Password reset email sent. Please check your inbox.";
    }

    private void Login(string email, string password)
    {
        var loginRequest = new LoginWithEmailAddressRequest { Email = email, Password = password, TitleId = TITLE_ID };
        PlayFabClientAPI.LoginWithEmailAddress(loginRequest, OnLoginSuccess, OnLoginFailure);
    }

    #endregion

    #region OnFailure

    private void OnRegisterFailure(PlayFabError error)
    {
        if (error.Error == PlayFabErrorCode.EmailAddressNotAvailable)
        {
            ValidationMessage.text = "Email is already in use.";
            PasswordInput.text = string.Empty;
            PasswordConfirmationInput.text = string.Empty;
        }
        else
        {
            ValidationMessage.text = "Registration failed: " + error.GenerateErrorReport();
            PasswordInput.text = string.Empty;
            PasswordConfirmationInput.text = string.Empty;
        }
    }

    private void OnLoginFailure(PlayFabError error)
    {
        if (error.Error == PlayFabErrorCode.AccountNotFound)
        {
            ValidationMessage.text = "Wrong login or password.";
        }
        else
        {
            Debug.LogError("Login failed: " + error.GenerateErrorReport());
        }
    }

    private void OnGetAccountInfoFailure(PlayFabError error)
    {
        Debug.LogError("Failed to retrieve account info: " + error.GenerateErrorReport());
    }

    private void OnPasswordResetFailure(PlayFabError error)
    {
        ValidationMessage.text = "Failed to send password reset email: " + error.GenerateErrorReport();
    }

    #endregion
}
