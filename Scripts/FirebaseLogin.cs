using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleFirebaseUnity;
using TMPro;
using UnityEngine.Networking;
using System;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;
using SimpleFirebaseUnity.MiniJSON;
public class FirebaseLogin : MonoBehaviour
{
    public TMP_InputField email, pword;
    public TextMeshProUGUI message;
    public Sprite eyeOn, eyeOff;
    public CanvasGroup raycastBlocker;
    public Button[] butts;
    public Button submit;
    public readonly Color32 redColor = new Color32(255, 121, 140, 255), greenColor = new Color32(107, 202, 77, 255);


    public const string API_KEY = //TYPE IN YOUR API KEY HERE;

    public void buttonClick()
    {
        GameControl.control.getSoundManager().playButton();
    }


    // attach to loginScreen
    private void Start()
    {
        if (GameControl.control.playerData.loggedIn)
        {
            LoggedinUI();
        }
        else
        {
            LoginUI();
        }
    }
    private void LoggedinUI()
    {
        if (GameControl.control.playerData.email != null)
        {
            setMessage("Hi, " + GameControl.control.playerData.email + "!");
        }
        email.gameObject.SetActive(false);
        pword.gameObject.SetActive(false);
        changeButton(submit, Logout, "logout", submit.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>());
        changeButton(butts[0], deleteAccount, "Delete account", (TextMeshProUGUI)butts[0].targetGraphic);
        butts[0].transform.parent.gameObject.SetActive(true);
        butts[1].gameObject.SetActive(false);
        butts[0].gameObject.SetActive(true);
        submit.targetGraphic.color = redColor;
    }
    private bool changeButton(Button b, UnityAction listener, string text = null, TextMeshProUGUI tmpro = null)
    {
        b.onClick.RemoveAllListeners();
        b.onClick.AddListener(listener);
        if (text != null)
        {
            if (tmpro.text == text)
            {
                // BUTTON ALREADY SET!
                return false;
            }
            tmpro.text = text;
        }
        return true;
    }
    private void LoginUI()
    {
        setMessage("");
        email.text = "";
        pword.text = "";
        email.gameObject.SetActive(true);
        pword.gameObject.SetActive(true);
        butts[0].transform.parent.gameObject.SetActive(true);
        changeButton(submit, Login, "login", submit.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>());
        changeButton(butts[0], forgotPasswordUI, "Forgot password", (TextMeshProUGUI)butts[0].targetGraphic);
        changeButton(butts[1], SignUpUI, "Create account", (TextMeshProUGUI)butts[1].targetGraphic);
        butts[1].gameObject.SetActive(true);
        submit.targetGraphic.color = redColor;
    }
    public void revealPassword(Image eye)
    {
        if (pword.contentType == TMP_InputField.ContentType.Password)
        {
            pword.contentType = TMP_InputField.ContentType.Standard;
            pword.ForceLabelUpdate();
            eye.sprite = eyeOff;
        }
        else
        {
            pword.contentType = TMP_InputField.ContentType.Password;
            pword.ForceLabelUpdate();
            eye.sprite = eyeOn;
        }
    }
    public void SignUpUI()
    {
        setMessage("Use a password with 8 or more characters.");
        butts[0].transform.parent.gameObject.SetActive(false);
        changeButton(submit, SignUp, "sign up", submit.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>());
        submit.targetGraphic.color = greenColor;
    }
    public void forgotPasswordUI()
    {
        setMessage("Enter email to change password:");
        butts[0].transform.parent.gameObject.SetActive(false);
        pword.gameObject.SetActive(false);
        email.gameObject.SetActive(true);
        changeButton(submit, forgotPasswordButton, "enter", submit.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>());
    }
    public void forgotPasswordButton()
    {
        raycastBlocker.blocksRaycasts = false;
        setMessage("Loading..");
        WWWForm form = new WWWForm();
        form.AddField("requestType", "PASSWORD_RESET");
        form.AddField("email", email.text);
        GameControl.control.PostRequest(form, "https://www.googleapis.com/identitytoolkit/v3/relyingparty/getOobConfirmationCode?key=", (UnityWebRequest www, bool success) =>
        {
            raycastBlocker.blocksRaycasts = true;
            if (!success)
            {
                setMessage(www);
                return;
            }
            doneForgotPwordUI();
            // sentForgotPwordUI();
        });
    }
    // private void sentForgotPwordUI()
    // {
    //     ((TextMeshProUGUI)email.placeholder).text = "Enter code";
    //     email.text = "";
    //     changeButton(submit, confirmForgotPwordCodeButton);
    //     butts[0].transform.parent.gameObject.SetActive(true);
    //     butts[0].gameObject.SetActive(false);
    //     butts[1].gameObject.SetActive(true);
    //     if (changeButton(butts[1], forgotPasswordButton, "Resend code", (TextMeshProUGUI)butts[1].targetGraphic))
    //     {
    //         setMessage("Enter code sent to email:");
    //     }
    //     else
    //     {
    //         setMessage("Resent code to email!");
    //     }
    // }
    // private void confirmForgotPwordCodeButton()
    // {
    //     setMessage("Loading..");
    //     WWWForm form = new WWWForm();
    //     form.AddField("oobCode", email.text);

    //     GameControl.control.PostRequest(form, "https://www.googleapis.com/identitytoolkit/v3/relyingparty/resetPassword?key=", (UnityWebRequest www, bool success) =>
    //     {
    //         if (!success)
    //         {
    //             setMessage(www);
    //             return;
    //         }
    //         confirmForgotPwordUI();
    //     });
    // }
    // private void confirmForgotPwordUI()
    // {
    //     ((TextMeshProUGUI)email.placeholder).text = "Email";
    //     setMessage("Type in new password:");
    //     email.gameObject.SetActive(false);
    //     pword.gameObject.SetActive(true);
    //     changeButton(submit, enterNewPwordButton);
    //     butts[0].transform.parent.gameObject.SetActive(false);
    //     butts[0].gameObject.SetActive(true);
    //     butts[1].gameObject.SetActive(true);
    // }
    // private void enterNewPwordButton()
    // {
    //     setMessage("Loading..");
    //     WWWForm form = new WWWForm();
    //     form.AddField("oobCode", email.text);
    //     form.AddField("newPassword", pword.text);
    //     GameControl.control.PostRequest(form, "https://www.googleapis.com/identitytoolkit/v3/relyingparty/resetPassword?key=", (UnityWebRequest www, bool success) =>
    //     {
    //         if (!success)
    //         {
    //             setMessage(www);
    //             return;
    //         }
    //         doneForgotPwordUI();
    //     });
    // }
    private void doneForgotPwordUI()
    {
        pword.gameObject.SetActive(false);
        setMessage("Check email to reset password.");
        email.gameObject.SetActive(false);
        submit.targetGraphic.color = greenColor;
        changeButton(submit, LoginUI, "ok", submit.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>());
    }
    private void setMessage(string mssg)
    {
        message.text = mssg;
    }
    private void setMessage(UnityWebRequest www)
    {
        string mssg;
        if (string.IsNullOrEmpty(www.downloadHandler.text))
        {
            mssg = "Check connection and try again.";
            setMessage(mssg);
            return;
        }
        object error = JsonConvert.DeserializeObject<Dictionary<string, object>>(www.downloadHandler.text)["error"];
        string errorStr = (string)JsonConvert.DeserializeObject<Dictionary<string, object>>(error.ToString())["message"];
        Debug.Log("error: " + errorStr);
        switch (errorStr)
        {
            case "MISSING_EMAIL":
                mssg = "Missing email. Try again.";
                break;
            case "EMAIL_EXISTS":
                mssg = "That email is taken. Try again.";
                break;
            case "INVALID_EMAIL":
                mssg = "Invalid email. Try again.";
                break;
            case "TOO_MANY_ATTEMPTS_TRY_LATER":
                mssg = "Too many attempts. Try again later.";
                break;
            case "INVALID_ID_TOKEN":
                mssg = "Server timed out. Try again later.";
                break;
            case "USER_NOT_FOUND":
                mssg = "User couldn't be found.";
                break;
            case "EMAIL_NOT_FOUND":
                mssg = "Email not found. Try again.";
                break;
            case "EXPIRED_OOB_CODE":
                mssg = "Code expired. Try resending code.";
                break;
            case "INVALID_OOB_CODE":
                mssg = "Code is invalid.";
                break;
            case "MISSING_PASSWORD":
                mssg = "Missing password. Try again.";
                break;
            case "INVALID_PASSWORD":
                mssg = "Invalid password. Try again.";
                break;
            default:
                mssg = "Server failed. Try again.";
                break;
        }
        setMessage(mssg);
    }

    private void setMessage(FirebaseError error, string action)
    {
        setMessage(getErrorString(error, action));
    }
    private string getErrorString(FirebaseError error, string action)
    {
        Debug.LogWarning("err msg - " + error.Message);
        return action + " failed: " + error.Message + ". Try again.";
    }

    private Dictionary<string, string> getDic(string text)
    {
        return JsonConvert.DeserializeObject<Dictionary<string, string>>(text);
    }

    public void SignUp()
    {
        if (pword.text.Length < 8)
        {
            setMessage("Use 8 or more characters for your password.");
            return;
        }
        raycastBlocker.blocksRaycasts = false;
        setMessage("Signing up..");

        WWWForm form = new WWWForm();
        form.AddField("email", email.text);
        form.AddField("password", pword.text);
        form.AddField("returnSecureToken", "true");
        GameControl.control.PostRequest(form, "https://www.googleapis.com/identitytoolkit/v3/relyingparty/signupNewUser?key=", (UnityWebRequest www, bool success) =>
        {

            if (!success)
            {
                setMessage(www);
                raycastBlocker.blocksRaycasts = true;
                return;
            }

            sendVerification(getDic(www.downloadHandler.text)["idToken"]);
        });
    }
    private void sendVerification(string idToken)
    {
        setMessage("Hold on..");
        WWWForm form = new WWWForm();
        form.AddField("requestType", "VERIFY_EMAIL");
        form.AddField("idToken", idToken);
        GameControl.control.PostRequest(form, "https://www.googleapis.com/identitytoolkit/v3/relyingparty/getOobConfirmationCode?key=", (UnityWebRequest www, bool success) =>
        {
            raycastBlocker.blocksRaycasts = true;
            if (!success)
            {
                setMessage(www);
                return;
            }
            butts[0].transform.parent.gameObject.SetActive(false);
            email.gameObject.SetActive(false);
            pword.gameObject.SetActive(false);
            ConfirmLinkUI(idToken);
        });
    }
    private void ConfirmLinkUI(string idToken)
    {
        changeButton(submit, () => ConfirmVerified(idToken), "Done", submit.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>());
        butts[0].transform.parent.gameObject.SetActive(true);
        butts[0].gameObject.SetActive(false);
        butts[1].gameObject.SetActive(true);

        if (changeButton(butts[1], () => sendVerification(idToken), "Resend link", (TextMeshProUGUI)butts[1].targetGraphic))
        {
            setMessage("Thank you! To finish,\nclick the link sent to your email.");
        }
        else
        {
            setMessage("Link resent!");
        }
    }
    private void ConfirmVerified(string idToken)
    {
        raycastBlocker.blocksRaycasts = false;
        setMessage("Loading..");
        WWWForm form = new WWWForm();
        form.AddField("idToken", idToken);
        GameControl.control.PostRequest(form, "https://www.googleapis.com/identitytoolkit/v3/relyingparty/getAccountInfo?key=", (UnityWebRequest www, bool success) =>
        {
            raycastBlocker.blocksRaycasts = true;
            if (!success)
            {
                setMessage(www);
                return;
            }
            IEnumerable verifiedDic = JsonConvert.DeserializeObject<Dictionary<string, object>>(www.downloadHandler.text)["users"] as IEnumerable;
            bool verified = false;
            foreach (object o in verifiedDic)
            {
                Debug.Log("Login data PT 2: " + o.ToString());
                verified = (bool)JsonConvert.DeserializeObject<Dictionary<string, object>>(o.ToString())["emailVerified"];
            }
            //  ((TextMeshProUGUI)email.placeholder).text = "Email";
            if (verified == false)
            {
                setMessage("Verify email to finish.");
                return;
            }
            Login();
        });
    }

    public void Login()
    {
        raycastBlocker.blocksRaycasts = false;
        setMessage("Logging in..");
        WWWForm form = new WWWForm();
        form.AddField("email", email.text);
        form.AddField("password", pword.text);
        form.AddField("returnSecureToken", "true");
        //Json.Serialize()
        GameControl.control.PostRequest(form, "https://www.googleapis.com/identitytoolkit/v3/relyingparty/verifyPassword?key=", (UnityWebRequest www, bool success) =>
        {

            if (!success)
            {
                raycastBlocker.blocksRaycasts = true;
                setMessage(www);
                return;
            }
            form = new WWWForm();
            Dictionary<string, string> dicString = getDic(www.downloadHandler.text);
            GameControl.control.setUser(dicString["refreshToken"], dicString["email"], dicString["localId"]);
            form.AddField("idToken", dicString["idToken"]);
            //check if email is verified
            GameControl.control.PostRequest(form, "https://www.googleapis.com/identitytoolkit/v3/relyingparty/getAccountInfo?key=", (UnityWebRequest www2, bool success2) =>
                {
                    if (!success2)
                    {
                        raycastBlocker.blocksRaycasts = true;
                        setMessage(www2);
                        return;
                    }
                    Debug.Log("Login data: " + www2.downloadHandler.text);

                    //need a reference to www2's response before expires
                    IEnumerable verifiedDic = JsonConvert.DeserializeObject<Dictionary<string, object>>(www2.downloadHandler.text)["users"] as IEnumerable;
                    bool verified = false;
                    foreach (object o in verifiedDic)
                    {
                        Debug.Log("Login data PT 2: " + o.ToString());
                        verified = (bool)JsonConvert.DeserializeObject<Dictionary<string, object>>(o.ToString())["emailVerified"];
                    }
                    if (verified == false)
                    {
                        raycastBlocker.blocksRaycasts = true;
                        GameControl.control.setUser(null, null, null);
                        GameControl.control.SavePlayerData();
                        setMessage("Verify email to log in.");
                        return;
                    }
                    GameControl.control.setFirebaseNull();
                    GetLoginData((bool loginSuccess, string message) =>
                          {
                              raycastBlocker.blocksRaycasts = true;
                              if (!loginSuccess)
                              {
                                  GameControl.control.setUser(null, null, null);
                                  GameControl.control.SavePlayerData();
                                  setMessage(message);
                                  return;
                              }

                              GameControl.control.setLoggedIn(true);
                              GameControl.control.SavePlayerData();
                              LoggedinUI();
                          });
                });

        });
    }

    private void GetLoginData(Action<bool, string> callback)
    {
        GameControl.control.getUser((FirebaseParam user) =>
        {
            GameControl.control.getFirebase().OnGetSuccess = (Firebase f, string RawJson) =>
            {
                GameControl.control.getFirebase().OnGetSuccess = null;
                if (!string.IsNullOrEmpty(RawJson) && RawJson != "null")
                {
                    Debug.Log("raw json - " + RawJson);

                    PlayerData temp = JsonUtility.FromJson<PlayerData>(RawJson);
                    if (temp != null)
                    {
                        Debug.Log("FOUND PLAYER DATA FILE! Login data: " + RawJson);
                        //are there multiple instances?
                        if (temp.loggedIn)
                        {
                            callback(false, "Login failed. Logout of other instances and try again.");
                            return;
                        }
                        //is temp the same as the current game?
                        if (temp.Equals(GameControl.control.playerData))
                        {
                            Debug.Log("temp is the same!");
                            callback(true, "");
                            return;
                        }
                        //can we override the current game?
                        GameControl.control.YesNoPrompt("Found saved game!\n<u>Overwrite current game with saved game?",
                        GameControl.control.transform, () =>
                        {
                            //yes
                            GameControl.control.playerData = temp;
                            GameControl.control.UpdatePlayerUI();
                            SceneManager.LoadScene(0);
                            callback(true, "");
                        }, () =>
                        {
                            //no
                            callback(false, "Login cancelled: Kept current game.");
                        }, "Over write", true);
                        return;
                    }
                }
                callback(true, "");
            };
            GameControl.control.getFirebase().OnGetFailed = (Firebase f, FirebaseError fe) =>
            {
                callback(false, getErrorString(fe, "Login"));
                GameControl.control.getFirebase().OnGetFailed = null;
            };
            GameControl.control.getFirebase().GetValue(user);
        }, false);
    }

    public void Logout()
    {
        //set login false at first so we can detect multiple instances
        raycastBlocker.blocksRaycasts = false;
        GameControl.control.setLoggedIn(false);
        setMessage("Logging out..");
        GameControl.control.getFirebase().OnSetFailed = (Firebase fb, FirebaseError error) =>
        {
            raycastBlocker.blocksRaycasts = true;
            GameControl.control.setLoggedIn(true);
            setMessage(error, "Logout");
            GameControl.control.getFirebase().OnSetFailed = null;
        };
        GameControl.control.getFirebase().OnSetSuccess = (Firebase fb, string ds) =>
        {
            raycastBlocker.blocksRaycasts = true;
            LoginUI();
            GameControl.control.setUser(null, null, null);
            GameControl.control.setFirebaseNull();
            GameControl.control.getFirebase().OnSetSuccess = null;
            GameControl.control.SavePlayerData();
        };
        GameControl.control.getUser((FirebaseParam user) =>
        {
            GameControl.control.getFirebase().SetValue(JsonUtility.ToJson(GameControl.control.playerData), true, user);
        }, true);
    }

    public void deleteAccount()
    {
        GameControl.control.YesNoPrompt("Are you sure you want to delete your account?", GameControl.control.transform, () =>
        {
            setMessage("Deleting account..");
            deleteAccountForce();
        }, null, "yes", true);
    }

    private void deleteAccountForce()
    {
        raycastBlocker.blocksRaycasts = false;
        GameControl.control.getUser((FirebaseParam user) =>
       {
           GameControl.control.getFirebase().OnDeleteSuccess = (Firebase fbDelete, string ds) =>
           {
               GameControl.control.getIdToken((string idToken) =>
               {
                   WWWForm form = new WWWForm();
                   form.AddField("idToken", idToken);
                   GameControl.control.PostRequest(form, "https://www.googleapis.com/identitytoolkit/v3/relyingparty/deleteAccount?key=", (UnityWebRequest www, bool success) =>
                   {
                       raycastBlocker.blocksRaycasts = true;
                       if (!success)
                       {
                           setMessage("Delete failed. Please try again.");
                           return;
                       }
                       //SUCESSFUL DELETE
                       LoginUI();
                       GameControl.control.setLoggedIn(false);
                       GameControl.control.setUser(null, null, null);
                       GameControl.control.setFirebaseNull();
                       GameControl.control.SavePlayerData();
                       setMessage("Deleted account successfully.");
                   });
               });
               GameControl.control.getFirebase().OnDeleteSuccess = null;
           };
           GameControl.control.getFirebase().OnDeleteFailed = (Firebase Firebase, FirebaseError error) =>
           {
               //pretend we deleted
               raycastBlocker.blocksRaycasts = true;
               setMessage("Delete failed. Please try again.");
           };
           GameControl.control.getFirebase().Delete(user);
       }, true);

    }


}
