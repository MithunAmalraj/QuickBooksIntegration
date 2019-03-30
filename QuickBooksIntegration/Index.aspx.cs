using Newtonsoft.Json;
using QuickBooksIntegration.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;

namespace QuickBooksIntegration
{
    public partial class Index : System.Web.UI.Page
    {
        // client configuration
        private static string redirectURI = ConfigurationManager.AppSettings["redirectURI"];

        private static string discoveryURI = ConfigurationManager.AppSettings["discoveryURI"];
        private static string clientID = ConfigurationManager.AppSettings["clientID"];
        private static string clientSecret = ConfigurationManager.AppSettings["clientSecret"];
        private static string qboBaseUrl = ConfigurationManager.AppSettings["qboBaseUrl"];
        private static string logPath = ConfigurationManager.AppSettings["logPath"];

        private static string scopeValC2QB = System.Uri.EscapeDataString(ConfigurationManager.AppSettings["scopeValC2QB"]);
        private static string scopeValOpenId = System.Uri.EscapeDataString(ConfigurationManager.AppSettings["scopeValOpenId"]);
        private static string scopeValSIWI = System.Uri.EscapeDataString(ConfigurationManager.AppSettings["scopeValSIWI"]);

        private static string authorizationEndpoint;
        private static string tokenEndpoint;
        private static string userinfoEndPoint;
        private static string revokeEndpoint;
        private static string issuerUrl;
        private static string jwksEndpoint;

        private string code = "";
        private string incoming_state = "";
        private string realmId = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["accessToken"] == null)
            {
                connect.Visible = true;
                revoke.Visible = false;
                lblConnected.Visible = false;
            }
            else
            {
                connect.Visible = false;
                revoke.Visible = true;
                //Disconnect();
            }

            if (Session["accessToken"] == null)
            {
                if (Request.QueryString.Count > 0)
                {
                    List<string> queryKeys = new List<string>(Request.QueryString.AllKeys);
                    // Check for errors.
                    if (queryKeys.Contains("error") == true)
                    {
                        output(String.Format("OAuth authorization error: {0}.", Request.QueryString["error"].ToString()));
                        return;
                    }
                    if (queryKeys.Contains("code") == false
                        || queryKeys.Contains("state") == false)
                    {
                        output("Malformed authorization response.");
                        return;
                    }

                    //extracts the state
                    if (Request.QueryString["state"] != null)
                    {
                        incoming_state = Request.QueryString["state"].ToString();
                        if (Session["CSRF"] != null)
                        {
                            //match incoming state with the saved State in your DB from doOAuth function and then execute the below steps
                            if (Session["CSRF"].ToString() == incoming_state)
                            {
                                //extract realmId is scope is for C2QB or Get App Now
                                //SIWI will not return realmId/companyId
                                if (Request.QueryString["realmId"] != null)
                                {
                                    realmId = Request.QueryString["realmId"].ToString();
                                    Session["realmId"] = realmId;
                                }

                                //extract the code
                                if (Request.QueryString["code"] != null)
                                {
                                    code = Request.QueryString["code"].ToString();
                                    output("Authorization code obtained.");

                                    //start the code exchange at the Token Endpoint.
                                    //this call will fail with 'invalid grant' error if application is not stopped after testing one button click flow as code is not renewed
                                    performCodeExchange(code, redirectURI, realmId);
                                }
                            }
                            else
                            {
                                output("Invalid State");

                                Session.Clear();
                                Session.Abandon();
                            }
                        }
                    }
                }
            }
        }

        #region OAuthMethods

        #region button click events

        protected void ImgC2QB_Click(object sender, ImageClickEventArgs e)
        {
            if (Session["accessToken"] == null)
            {
                //call this once a day or at application_start in your code.
                getDiscoveryData();

                //doOauth for Connect to Quickbooks button
                doOAuth("C2QB");
            }
        }

        protected void ImgRevoke_Click(object sender, EventArgs e)
        {
            if (Session["accessToken"] != null && Session["refreshToken"] != null)
            {
                //revoke tokens
                performRevokeToken(Session["accessToken"].ToString(), Session["refreshToken"].ToString());
            }
        }

        protected void ImgQBOAPICall_Click(object sender, EventArgs e)
        {
            if (Session["realmId"] != null)
            {
                if (Session["accessToken"] != null && Session["refreshToken"] != null)
                {
                    //call QBO api
                    qboApiCall(Session["accessToken"].ToString(), Session["refreshToken"].ToString(), Session["realmId"].ToString());
                }
            }
            else
            {
                output("SIWI call does not returns realm for QBO qbo api call.");
                lblQBOCall.Visible = true;
                lblQBOCall.Text = "SIWI call does not returns realm for QBO qbo api call";
            }
        }

        #endregion button click events

        #region get Discovery data

        private void getDiscoveryData()
        {
            output("Fetching Discovery Data.");

            DiscoveryData discoveryDataDecoded;

            // build the request
            HttpWebRequest discoveryRequest = (HttpWebRequest)WebRequest.Create(discoveryURI);
            discoveryRequest.Method = "GET";
            discoveryRequest.Accept = "application/json";

            try
            {
                //call Discovery endpoint
                HttpWebResponse discoveryResponse = (HttpWebResponse)discoveryRequest.GetResponse();
                using (var discoveryDataReader = new StreamReader(discoveryResponse.GetResponseStream()))
                {
                    //read response
                    string responseText = discoveryDataReader.ReadToEnd();

                    // converts to dictionary
                    discoveryDataDecoded = JsonConvert.DeserializeObject<DiscoveryData>(responseText);
                }

                //Authorization endpoint url
                authorizationEndpoint = discoveryDataDecoded.Authorization_endpoint;

                //Token endpoint url
                tokenEndpoint = discoveryDataDecoded.Token_endpoint;

                //UseInfo endpoint url
                userinfoEndPoint = discoveryDataDecoded.Userinfo_endpoint;

                //Revoke endpoint url
                revokeEndpoint = discoveryDataDecoded.Revocation_endpoint;

                //Issuer endpoint Url
                issuerUrl = discoveryDataDecoded.Issuer;

                //Json Web Key Store Url
                jwksEndpoint = discoveryDataDecoded.JWKS_uri;

                output("Discovery Data obtained.");
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = ex.Response as HttpWebResponse;
                    if (response != null)
                    {
                        output("HTTP Status: " + response.StatusCode);
                        var exceptionDetail = response.GetResponseHeader("WWW-Authenticate");
                        if (exceptionDetail != null && exceptionDetail != "")
                        {
                            output(exceptionDetail);
                        }
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            // read response body
                            string responseText = reader.ReadToEnd();
                            if (responseText != null && responseText != "")
                            {
                                output(responseText);
                            }
                        }
                    }
                }
                else
                {
                    output(ex.Message);
                }
            }
        }

        #endregion get Discovery data

        #region OAuth2 calls

        private void doOAuth(string callMadeBy)
        {
            output("Intiating OAuth2 call to get code.");
            string authorizationRequest = "";
            string scopeVal = "";

            //Generate the state and save this in DB to match it against the incoming_state value after this call is completed
            //Statecan be a unique Id, campaign id, tracking id or CSRF token
            string stateVal = randomDataBase64url(32);
            if (Session["CSRF"] == null)
            {
                Session["CSRF"] = stateVal;
            }

            //Decide scope based on which flow was initiated
            if (callMadeBy == "C2QB") //C2QB scopes
            {
                Session["callMadeBy"] = "C2QB";
                scopeVal = scopeValC2QB;
            }
            else if (callMadeBy == "OpenId")//Get App Now scopes
            {
                Session["callMadeBy"] = "OpenId";
                scopeVal = scopeValOpenId;
            }
            else if (callMadeBy == "SIWI")//Sign In With Intuit scopes
            {
                Session["callMadeBy"] = "SIWI";
                scopeVal = scopeValSIWI;
            }

            if (authorizationEndpoint != "" && authorizationEndpoint != null)
            {
                //Create the OAuth 2.0 authorization request.
                authorizationRequest = string.Format("{0}?client_id={1}&response_type=code&scope={2}&redirect_uri={3}&state={4}",
                    authorizationEndpoint,
                    clientID,
                    scopeVal,
                    System.Uri.EscapeDataString(redirectURI),
                    stateVal);

                if (callMadeBy == "C2QB" || callMadeBy == "SIWI")
                {
                    //redirect to authorization request url
                    Response.Redirect(authorizationRequest, "_blank", "menubar=0,scrollbars=1,width=780,height=900,top=10");
                }
                else
                {
                    //redirect to authorization request url
                    Response.Redirect(authorizationRequest);
                }
            }
            else
            {
                output("Missing authorizationEndpoint url!");
            }
        }

        private void performCodeExchange(string code, string redirectURI, string realmId)
        {
            output("Exchanging code for tokens.");

            string id_token = "";
            string refresh_token = "";
            string access_token = "";
            bool isTokenValid = false;
            string sub = "";
            string email = "";
            string emailVerified = "";
            string givenName = "";
            string familyName = "";
            string phoneNumber = "";
            string phoneNumberVerified = "";
            string streetAddress = "";
            string locality = "";
            string region = "";
            string postalCode = "";
            string country = "";

            string cred = string.Format("{0}:{1}", clientID, clientSecret);
            string enc = Convert.ToBase64String(Encoding.ASCII.GetBytes(cred));
            string basicAuth = string.Format("{0} {1}", "Basic", enc);

            // build the  request
            string accesstokenRequestBody = string.Format("grant_type=authorization_code&code={0}&redirect_uri={1}",
                code,
                System.Uri.EscapeDataString(redirectURI)
                );

            // send the Token request
            HttpWebRequest accesstokenRequest = (HttpWebRequest)WebRequest.Create(tokenEndpoint);
            accesstokenRequest.Method = "POST";
            accesstokenRequest.ContentType = "application/x-www-form-urlencoded";
            accesstokenRequest.Accept = "application/json";
            accesstokenRequest.Headers[HttpRequestHeader.Authorization] = basicAuth;//Adding Authorization header

            byte[] _byteVersion = Encoding.ASCII.GetBytes(accesstokenRequestBody);
            accesstokenRequest.ContentLength = _byteVersion.Length;
            Stream stream = accesstokenRequest.GetRequestStream();
            stream.Write(_byteVersion, 0, _byteVersion.Length);//verify
            stream.Close();

            try
            {
                // get the response
                HttpWebResponse accesstokenResponse = (HttpWebResponse)accesstokenRequest.GetResponse();
                using (var accesstokenReader = new StreamReader(accesstokenResponse.GetResponseStream()))
                {
                    //read response
                    string responseText = accesstokenReader.ReadToEnd();
                    //decode response
                    Dictionary<string, string> accesstokenEndpointDecoded = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseText);

                    if (accesstokenEndpointDecoded.ContainsKey("refresh_token"))
                    {
                        //save the refresh token in persistent store so that it can be used to refresh short lived access tokens
                        refresh_token = accesstokenEndpointDecoded["refresh_token"];
                        Session["refreshToken"] = refresh_token;

                        if (accesstokenEndpointDecoded.ContainsKey("access_token"))
                        {
                            output("Access token obtained.");
                            access_token = accesstokenEndpointDecoded["access_token"];
                            Session["accessToken"] = access_token;
                        }
                    }

                    if (Session["callMadeby"].ToString() == "OpenId")
                    {
                        if (Request.Url.Query == "")
                        {
                            Response.Redirect(Request.RawUrl);
                        }
                        else
                        {
                            Response.Redirect(Request.RawUrl.Replace(Request.Url.Query, ""));
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = ex.Response as HttpWebResponse;
                    if (response != null)
                    {
                        output("HTTP Status: " + response.StatusCode);
                        var exceptionDetail = response.GetResponseHeader("WWW-Authenticate");
                        if (exceptionDetail != null && exceptionDetail != "")
                        {
                            output(exceptionDetail);
                        }
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            // read response body
                            string responseText = reader.ReadToEnd();
                            if (responseText != null && responseText != "")
                            {
                                output(responseText);
                            }
                        }
                    }
                }
            }
        }

        private void performRefreshToken(string refresh_token)
        {
            output("Exchanging refresh token for access token.");//refresh token is valid for 100days and access token for 1hr
            string access_token = "";
            string cred = string.Format("{0}:{1}", clientID, clientSecret);
            string enc = Convert.ToBase64String(Encoding.ASCII.GetBytes(cred));
            string basicAuth = string.Format("{0} {1}", "Basic", enc);

            // build the  request
            string refreshtokenRequestBody = string.Format("grant_type=refresh_token&refresh_token={0}",
                refresh_token
                );

            // send the Refresh Token request
            HttpWebRequest refreshtokenRequest = (HttpWebRequest)WebRequest.Create(tokenEndpoint);
            refreshtokenRequest.Method = "POST";
            refreshtokenRequest.ContentType = "application/x-www-form-urlencoded";
            refreshtokenRequest.Accept = "application/json";
            //Adding Authorization header
            refreshtokenRequest.Headers[HttpRequestHeader.Authorization] = basicAuth;

            byte[] _byteVersion = Encoding.ASCII.GetBytes(refreshtokenRequestBody);
            refreshtokenRequest.ContentLength = _byteVersion.Length;
            Stream stream = refreshtokenRequest.GetRequestStream();
            stream.Write(_byteVersion, 0, _byteVersion.Length);
            stream.Close();

            try
            {
                //get response
                HttpWebResponse refreshtokenResponse = (HttpWebResponse)refreshtokenRequest.GetResponse();
                using (var refreshTokenReader = new StreamReader(refreshtokenResponse.GetResponseStream()))
                {
                    //read response
                    string responseText = refreshTokenReader.ReadToEnd();

                    // decode response
                    Dictionary<string, string> refreshtokenEndpointDecoded = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseText);

                    if (refreshtokenEndpointDecoded.ContainsKey("error"))
                    {
                        // Check for errors.
                        if (refreshtokenEndpointDecoded["error"] != null)
                        {
                            output(String.Format("OAuth token refresh error: {0}.", refreshtokenEndpointDecoded["error"]));
                            return;
                        }
                    }
                    else
                    {
                        //if no error
                        if (refreshtokenEndpointDecoded.ContainsKey("refresh_token"))
                        {
                            refresh_token = refreshtokenEndpointDecoded["refresh_token"];
                            Session["refreshToken"] = refresh_token;

                            if (refreshtokenEndpointDecoded.ContainsKey("access_token"))
                            {
                                //save both refresh token and new access token in permanent store
                                access_token = refreshtokenEndpointDecoded["access_token"];
                                Session["accessToken"] = access_token;
                            }
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = ex.Response as HttpWebResponse;
                    if (response != null)
                    {
                        output("HTTP Status: " + response.StatusCode);
                        var exceptionDetail = response.GetResponseHeader("WWW-Authenticate");
                        if (exceptionDetail != null && exceptionDetail != "")
                        {
                            output(exceptionDetail);
                        }
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            // read response body
                            string responseText = reader.ReadToEnd();
                            if (responseText != null && responseText != "")
                            {
                                output(responseText);
                            }
                        }
                    }
                }
            }

            output("Access token refreshed.");
        }

        private void performRevokeToken(string access_token, string refresh_token)
        {
            output("Performing Revoke tokens.");

            string cred = string.Format("{0}:{1}", clientID, clientSecret);
            string enc = Convert.ToBase64String(Encoding.ASCII.GetBytes(cred));
            string basicAuth = string.Format("{0} {1}", "Basic", enc);

            // build the request
            string tokenRequestBody = "{\"token\":\"" + refresh_token + "\"}";

            // send the Revoke token request
            HttpWebRequest tokenRequest = (HttpWebRequest)WebRequest.Create(revokeEndpoint);
            tokenRequest.Method = "POST";
            tokenRequest.ContentType = "application/json";
            tokenRequest.Accept = "application/json";
            //Add Authorization header
            tokenRequest.Headers[HttpRequestHeader.Authorization] = basicAuth;

            byte[] _byteVersion = Encoding.ASCII.GetBytes(tokenRequestBody);
            tokenRequest.ContentLength = _byteVersion.Length;
            Stream stream = tokenRequest.GetRequestStream();
            stream.Write(_byteVersion, 0, _byteVersion.Length);
            stream.Close();

            try
            {
                //get the response
                HttpWebResponse response = (HttpWebResponse)tokenRequest.GetResponse();

                //here you should handle status code and take action based on that
                if (response.StatusCode == HttpStatusCode.OK)//200
                {
                    output("Successful Revoke!");
                    revoke.Visible = false;
                    lblConnected.Visible = true;
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)//400
                {
                    output("One or more of BearerToken, RefreshToken, ClientId or, ClientSecret are incorrect.");
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)//401
                {
                    output("Bad authorization header or no authorization header sent.");
                }
                else if (response.StatusCode == HttpStatusCode.InternalServerError)//500
                {
                    output("Intuit server internal error, not the fault of the developer.");
                }

                //We are removing all sessions and qerystring here even if we get error on revoke.
                //In your code, you can choose to handle the errors and then delete sessions and querystring
                Session.Clear();
                Session.Abandon();
                if (Request.Url.Query == "")
                {
                    Response.Redirect(Request.RawUrl);
                }
                else
                {
                    Response.Redirect(Request.RawUrl.Replace(Request.Url.Query, ""));
                }
            }
            catch (WebException ex)
            {
                Session.Clear();
                Session.Abandon();
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = ex.Response as HttpWebResponse;
                    if (response != null)
                    {
                        output("HTTP Status: " + response.StatusCode);
                        var exceptionDetail = response.GetResponseHeader("WWW-Authenticate");
                        if (exceptionDetail != null && exceptionDetail != "")
                        {
                            output(exceptionDetail);
                        }
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            // read response body
                            string responseText = reader.ReadToEnd();
                            if (responseText != null && responseText != "")
                            {
                                output(responseText);
                            }
                        }
                    }
                }
            }

            output("Token revoked.");
        }

        #endregion OAuth2 calls

        #region qbo calls

        private void qboApiCall(string access_token, string refresh_token, string realmId)
        {
            try
            {
                if (realmId != "")
                {
                    output("Making QBO API Call.");

                    string query = "select * from CompanyInfo";
                    // build the  request
                    string encodedQuery = WebUtility.UrlEncode(query);

                    //add qbobase url and query
                    string uri = string.Format("https://{0}/v3/company/{1}/query?query={2}", qboBaseUrl, realmId, encodedQuery);

                    // send the request
                    HttpWebRequest qboApiRequest = (HttpWebRequest)WebRequest.Create(uri);
                    qboApiRequest.Method = "GET";
                    qboApiRequest.Headers.Add(string.Format("Authorization: Bearer {0}", access_token));
                    qboApiRequest.ContentType = "application/json;charset=UTF-8";
                    qboApiRequest.Accept = "*/*";

                    // get the response
                    HttpWebResponse qboApiResponse = (HttpWebResponse)qboApiRequest.GetResponse();
                    if (qboApiResponse.StatusCode == HttpStatusCode.Unauthorized)//401
                    {
                        output("Invalid/Expired Access Token.");
                        //if you get a 401 token expiry then perform token refresh
                        performRefreshToken(refresh_token);

                        //Retry QBO API call again with new tokens
                        if (Session["accessToken"] != null && Session["refreshToken"] != null && Session["realmId"] != null)
                        {
                            qboApiCall(Session["accessToken"].ToString(), Session["refreshToken"].ToString(), Session["realmId"].ToString());
                        }
                    }
                    else
                    {
                        //read qbo api response
                        using (var qboApiReader = new StreamReader(qboApiResponse.GetResponseStream()))
                        {
                            string responseText = qboApiReader.ReadToEnd();
                            output("QBO call successful.");
                            lblQBOCall.Visible = true;
                            lblQBOCall.Text = "QBO Call successful";
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = ex.Response as HttpWebResponse;
                    if (response != null)
                    {
                        output("HTTP Status: " + response.StatusCode);
                        var exceptionDetail = response.GetResponseHeader("WWW-Authenticate");
                        if (exceptionDetail != null && exceptionDetail != "")
                        {
                            output(exceptionDetail);
                        }
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            // read response body
                            string responseText = reader.ReadToEnd();
                            if (responseText != null && responseText != "")
                            {
                                output(responseText);
                            }
                        }
                    }
                }
            }
        }

        #endregion qbo calls

        #region methods for Oauth2

        /// <summary>
        /// Appends the given string to the on-screen log, and the debug console.
        /// </summary>
        /// <param name="output">string to be appended</param>
        public string GetLogPath()
        {
            try
            {
                if (logPath == "")
                {
                    logPath = System.Environment.GetEnvironmentVariable("TEMP");
                    if (!logPath.EndsWith("\\")) logPath += "\\";
                }
            }
            catch
            {
                output("Log error path not found.");
            }

            return logPath;
        }

        /// <summary>
        /// Appends the given string to the on-screen log, and the debug console.
        /// </summary>
        /// <param name="output">string to be appended</param>
        public void output(string logMsg)
        {
            //Console.WriteLine(logMsg);

            System.IO.StreamWriter sw = System.IO.File.AppendText(GetLogPath() + "OAuth2SampleAppLogs.txt");
            try
            {
                string logLine = System.String.Format(
                    "{0:G}: {1}.", System.DateTime.Now, logMsg);
                sw.WriteLine(logLine);
            }
            finally
            {
                sw.Close();
            }
        }

        /// <summary>
        /// Returns URI-safe data with a given input length.
        /// </summary>
        /// <param name="length">Input length (nb. output will be longer)</param>
        /// <returns></returns>
        public static string randomDataBase64url(uint length)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] bytes = new byte[length];
            rng.GetBytes(bytes);
            return base64urlencodeNoPadding(bytes);
        }

        /// <summary>
        /// Returns the SHA256 hash of the input string.
        /// </summary>
        /// <param name="inputStirng"></param>
        /// <returns></returns>
        public static byte[] sha256(string inputString)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(inputString);
            SHA256Managed sha256 = new SHA256Managed();
            return sha256.ComputeHash(bytes);
        }

        /// <summary>
        /// Base64url no-padding encodes the given input buffer. (encode)
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static string base64urlencodeNoPadding(byte[] buffer)
        {
            string base64 = Convert.ToBase64String(buffer);

            // Converts base64 to base64url.
            base64 = base64.Replace("+", "-");
            base64 = base64.Replace("/", "_");
            // Strips padding.
            base64 = base64.Replace("=", "");

            return base64;
        }

        /// <summary>
        /// Generates byte array  from Base64url string (decode)
        /// </summary>
        /// <param name="base64Url"></param>
        /// <returns></returns>
        private static byte[] FromBase64Url(string base64Url)
        {
            string padded = base64Url.Length % 4 == 0
                ? base64Url : base64Url + "====".Substring(base64Url.Length % 4);
            string base64 = padded.Replace("_", "/")
                                  .Replace("-", "+");
            return Convert.FromBase64String(base64);
        }

        #endregion methods for Oauth2

        #endregion OAuthMethods

        protected void ddlModule_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblMessage.Text = "";

            divPayment.Visible = false;
            divCustomer.Visible = false;
            divAccount.Visible = false;
            divVendor.Visible = false;
            divBill.Visible = false;
            divBillPayment.Visible = false;
            divInvoice.Visible = false;
            divPurchase.Visible = false;
            divSalesReceipt.Visible = false;

            if (ddlModule.SelectedValue == "Payment")
            {
                divPayment.Visible = true;
            }
            else if (ddlModule.SelectedValue == "Customer")
            {
                divCustomer.Visible = true;
            }
            else if (ddlModule.SelectedValue == "Account")
            {
                divAccount.Visible = true;
            }
            else if (ddlModule.SelectedValue == "Vendor")
            {
                divVendor.Visible = true;
            }
            else if (ddlModule.SelectedValue == "Bill")
            {
                divBill.Visible = true;
            }
            else if (ddlModule.SelectedValue == "BillPayment")
            {
                divBillPayment.Visible = true;
            }
            else if (ddlModule.SelectedValue == "Invoice")
            {
                divInvoice.Visible = true;
            }
            else if (ddlModule.SelectedValue == "Purchase")
            {
                divPurchase.Visible = true;
            }
            else if (ddlModule.SelectedValue == "SalesReceipt")
            {
                divSalesReceipt.Visible = true;
            }
        }

        public string GETQBAPICall(string uri)
        {
            string responseText = "";
            HttpWebRequest qboApiRequest = (HttpWebRequest)WebRequest.Create(uri);
            qboApiRequest.Method = "GET";
            qboApiRequest.Headers.Add(string.Format("Authorization: Bearer {0}", Session["accessToken"]));
            qboApiRequest.ContentType = "application/json;charset=UTF-8";
            qboApiRequest.Accept = "application/json";

            // get the response
            HttpWebResponse qboApiResponse = (HttpWebResponse)qboApiRequest.GetResponse();
            if (qboApiResponse.StatusCode == HttpStatusCode.Unauthorized)//401
            {
                //Refresh Token
            }
            else
            {
                //read qbo api response
                using (var qboApiReader = new StreamReader(qboApiResponse.GetResponseStream()))
                {
                    responseText = qboApiReader.ReadToEnd();
                }
            }
            return responseText;
        }

        public string POSTQBAPICall(string uri, string body)
        {
            string responseText = "";
            try
            {
                HttpWebRequest qboApiRequest = (HttpWebRequest)WebRequest.Create(uri);
                qboApiRequest.Method = "POST";
                qboApiRequest.Headers.Add(string.Format("Authorization: Bearer {0}", Session["accessToken"]));
                qboApiRequest.ContentType = "application/json;charset=UTF-8";
                qboApiRequest.Accept = "application/json";
                // get the response
                using (var streamWriter = new StreamWriter(qboApiRequest.GetRequestStream()))
                {
                    streamWriter.Write(body);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                HttpWebResponse qboApiResponse = (HttpWebResponse)qboApiRequest.GetResponse();
                if (qboApiResponse.StatusCode == HttpStatusCode.Unauthorized)//401
                {
                    //Refresh Token
                }
                else
                {
                    //read qbo api response
                    using (var qboApiReader = new StreamReader(qboApiResponse.GetResponseStream()))
                    {
                        responseText = qboApiReader.ReadToEnd();
                    }
                }
            }
            catch (WebException ex)
            {
                using (WebResponse response = ex.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    Console.WriteLine("Error code: {0}", httpResponse.StatusCode);
                    using (Stream data = response.GetResponseStream())
                    using (var reader = new StreamReader(data))
                    {
                        string text = reader.ReadToEnd();
                        Console.WriteLine(text);
                    }
                }
            }
            return responseText;
        }

        #region Customer

        protected void ddlCustomerOptions_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblMessage.Text = "";
            divCreateCustomer.Visible = false;
            divCustomerReadById.Visible = false;
            divUpdateCustomer.Visible = false;
            divCustomerDelete.Visible = false;
            divCustomers.Visible = false;

            if (ddlCustomerOptions.SelectedValue == "CreateCustomer")
            {
                divCreateCustomer.Visible = true;
            }
            if (ddlCustomerOptions.SelectedValue == "GetCustomerById")
            {
                divCustomerReadById.Visible = true;
            }
            if (ddlCustomerOptions.SelectedValue == "CustomerUpdate")
            {
                divUpdateCustomer.Visible = true;
            }
            if (ddlCustomerOptions.SelectedValue == "CustomerDelete")
            {
                divCustomerDelete.Visible = true;
            }
            if (ddlCustomerOptions.SelectedValue == "ReadAllCustomers")
            {
                divCustomers.Visible = true;
            }
        }

        protected void btnCreateCustomer_Click(object sender, EventArgs e)
        {
            try
            {
                string responseText = "";
                string uri = string.Format("https://{0}/v3/company/{1}/customer", qboBaseUrl, Session["realmId"]);
                //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/customers/1/bank-accounts/";
                string body = "";
                CreateCustomerReq ccObj = new CreateCustomerReq();
                ccObj.BillAddr = new CreateCustomerReqBillAddr();
                ccObj.BillAddr.Line1 = txtCCBillingAddressLine1.Text;
                ccObj.BillAddr.City = txtCCBillingAddressCity.Text;
                ccObj.BillAddr.Country = txtCCBillingAddressCountry.Text;
                ccObj.DisplayName = txtCCName.Text;
                ccObj.Notes = txtCCNotes.Text;
                body = JsonConvert.SerializeObject(ccObj);
                // send the request
                responseText = POSTQBAPICall(uri, body);
                if (responseText != null && responseText != "")
                {
                    CreateCustomerRes ccResObj = new CreateCustomerRes();
                    ccResObj = JsonConvert.DeserializeObject<CreateCustomerRes>(responseText);
                    if (ccResObj != null)
                    {
                        if (ccResObj.Customer != null && ccResObj.Customer.Id != null && ccResObj.Customer.Id != "")
                        {
                            //Customer Created Successfully
                            divCreateCustomer.Visible = false;
                            txtCCBillingAddressLine1.Text = "";
                            txtCCBillingAddressCity.Text = "";
                            txtCCBillingAddressCountry.Text = "";
                            txtCCName.Text = "";
                            txtCCNotes.Text = "";
                            lblMessage.Text = "Customer Created Successfully";
                        }
                    }
                    else
                    {
                        lblMessage.Text = responseText;
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        protected void btnGetCustomerById_Click(object sender, EventArgs e)
        {
            try
            {
                string CustomerId = txtCustomerId.Text;
                string responseText = "";
                string uri = string.Format("https://{0}/v3/company/{1}/customer/{2}", qboBaseUrl, Session["realmId"], CustomerId);
                //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/customers/1/bank-accounts/";

                // send the request
                responseText = GETQBAPICall(uri);
                if (responseText != null && responseText != "")
                {
                    CustomerByIdRes ccResObj = new CustomerByIdRes();
                    ccResObj = JsonConvert.DeserializeObject<CustomerByIdRes>(responseText);
                    if (ccResObj != null)
                    {
                        if (ccResObj.Customer != null && ccResObj.Customer.Id != null && ccResObj.Customer.Id != "")
                        {
                            lblMessage.Text = "Customer  Name : " + ccResObj.Customer.DisplayName;
                        }
                    }
                    else
                    {
                        lblMessage.Text = responseText;
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        protected void btnUpdateCustomer_Click(object sender, EventArgs e)
        {
            try
            {
                string CustomerId = txtUCCustomerId.Text;
                string firstresponseText = "";
                string firsturi = string.Format("https://{0}/v3/company/{1}/customer/{2}", qboBaseUrl, Session["realmId"], CustomerId);
                //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/customers/1/bank-accounts/";

                // send the request
                firstresponseText = GETQBAPICall(firsturi);
                if (firstresponseText != null && firstresponseText != "")
                {
                    CustomerByIdRes ccResObj = new CustomerByIdRes();
                    ccResObj = JsonConvert.DeserializeObject<CustomerByIdRes>(firstresponseText);
                    if (ccResObj != null)
                    {
                        if (ccResObj.Customer != null && ccResObj.Customer.Id != null && ccResObj.Customer.Id != "")
                        {
                            string responseText = "";
                            string uri = string.Format("https://{0}/v3/company/{1}/customer", qboBaseUrl, Session["realmId"]);
                            //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/customers/1/bank-accounts/";
                            string body = "";

                            UpdateCustoemrReq ccObj = new UpdateCustoemrReq();
                            ccObj.Taxable = ccResObj.Customer.Taxable;
                            ccObj.BillAddr = new UpdateCustoemrReqBillAddr();
                            ccObj.BillAddr.City = ccResObj.Customer.BillAddr.City;
                            ccObj.BillAddr.Country = ccResObj.Customer.BillAddr.Country;
                            ccObj.BillAddr.CountrySubDivisionCode = ccResObj.Customer.BillAddr.CountrySubDivisionCode;
                            ccObj.BillAddr.PostalCode = ccResObj.Customer.BillAddr.PostalCode;
                            ccObj.Notes = ccResObj.Customer.Notes;
                            ccObj.Job = ccResObj.Customer.Job;
                            ccObj.BillWithParent = ccResObj.Customer.BillWithParent;
                            ccObj.Balance = ccResObj.Customer.Balance;
                            ccObj.BalanceWithJobs = ccResObj.Customer.BalanceWithJobs;
                            ccObj.CurrencyRef = new UpdateCustoemrReqCurrencyRef();
                            ccObj.CurrencyRef.name = ccResObj.Customer.CurrencyRef.name;
                            ccObj.CurrencyRef.value = ccResObj.Customer.CurrencyRef.value;
                            ccObj.PreferredDeliveryMethod = ccResObj.Customer.PreferredDeliveryMethod;
                            ccObj.domain = ccResObj.Customer.domain;
                            ccObj.sparse = ccResObj.Customer.sparse;
                            ccObj.Id = ccResObj.Customer.Id;
                            ccObj.SyncToken = ccResObj.Customer.SyncToken;
                            ccObj.FullyQualifiedName = ccResObj.Customer.FullyQualifiedName;
                            ccObj.DisplayName = txtUCCustomerName.Text;
                            ccObj.PrintOnCheckName = ccResObj.Customer.PrintOnCheckName;
                            ccObj.Active = ccResObj.Customer.Active;
                            ccObj.PrimaryPhone = new UpdateCustoemrReqPrimaryPhone();
                            ccObj.PrimaryPhone.FreeFormNumber = ccResObj.Customer.PrimaryPhone.FreeFormNumber;
                            ccObj.PrimaryEmailAddr = new UpdateCustoemrReqPrimaryEmailAddr();
                            ccObj.PrimaryEmailAddr.Address = ccResObj.Customer.PrimaryEmailAddr.Address;
                            ccObj.DefaultTaxCodeRef = new UpdateCustoemrReqDefaultTaxCodeRef();
                            ccObj.DefaultTaxCodeRef.value = ccResObj.Customer.DefaultTaxCodeRef.value;
                            body = JsonConvert.SerializeObject(ccObj);
                            // send the request
                            responseText = POSTQBAPICall(uri, body);
                            if (responseText != null && responseText != "")
                            {
                            }
                        }
                    }
                    else
                    {
                        lblMessage.Text = firstresponseText;
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        protected void btnCustomerDelete_Click(object sender, EventArgs e)
        {
            try
            {
                string CustomerId = txtCDId.Text;
                string firstresponseText = "";
                string firsturi = string.Format("https://{0}/v3/company/{1}/customer/{2}", qboBaseUrl, Session["realmId"], CustomerId);
                //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/customers/1/bank-accounts/";

                // send the request
                firstresponseText = GETQBAPICall(firsturi);
                if (firstresponseText != null && firstresponseText != "")
                {
                    CustomerByIdRes ccResObj = new CustomerByIdRes();
                    ccResObj = JsonConvert.DeserializeObject<CustomerByIdRes>(firstresponseText);
                    if (ccResObj != null)
                    {
                        if (ccResObj.Customer != null && ccResObj.Customer.Id != null && ccResObj.Customer.Id != "")
                        {
                            string responseText = "";
                            string uri = string.Format("https://{0}/v3/company/{1}/customer", qboBaseUrl, Session["realmId"]);
                            //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/customers/1/bank-accounts/";
                            string body = "";

                            CustomerDeleteReq ccObj = new CustomerDeleteReq();
                            ccObj.domain = ccResObj.Customer.domain;
                            ccObj.sparse = true;
                            ccObj.Id = ccResObj.Customer.Id;
                            ccObj.SyncToken = ccResObj.Customer.SyncToken;
                            ccObj.Active = false;
                            body = JsonConvert.SerializeObject(ccObj);
                            // send the request
                            responseText = POSTQBAPICall(uri, body);
                            if (responseText != null && responseText != "")
                            {
                            }
                        }
                    }
                    else
                    {
                        lblMessage.Text = firstresponseText;
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        protected void btnGetCustomers_Click(object sender, EventArgs e)
        {
            try
            {
                List<CustomersSummary> customerList = new List<CustomersSummary>();
                customerList = GetCustomers();
                if (customerList != null && customerList.Count > 0)
                {
                    gvwCustomers.DataSource = customerList;
                    gvwCustomers.DataBind();
                }
                return;
            }
            catch (Exception ex)
            {
            }
        }

        public List<CustomersSummary> GetCustomers()
        {
            string responseText = "";
            List<CustomersSummary> customerList = new List<CustomersSummary>();
            try
            {
                string query = "Select * from Customer startposition 1 maxresults 5";
                // build the  request
                string encodedQuery = WebUtility.UrlEncode(query);

                //add qbobase url and query
                string uri = string.Format("https://{0}/v3/company/{1}/query?query={2}", qboBaseUrl, Session["realmId"], encodedQuery);
                //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/customers/1/bank-accounts/";
                // send the request
                responseText = GETQBAPICall(uri);
                if (responseText != null && responseText != "")
                {
                    ReadAllCustomesRes custObj = JsonConvert.DeserializeObject<ReadAllCustomesRes>(responseText);

                    foreach (var item in custObj.QueryResponse.Customer)
                    {
                        CustomersSummary customerObj = new CustomersSummary();
                        customerObj.id = item.Id;
                        customerObj.name = item.DisplayName;
                        customerList.Add(customerObj);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return customerList;
        }

        #endregion Customer

        #region CustomerPayments

        protected void ddlPaymentOptions_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblMessage.Text = "";
            divCreatePayment.Visible = false;
            divPaymentReadById.Visible = false;
            divUpdatePayment.Visible = false;
            divPaymentDelete.Visible = false;
            divPayments.Visible = false;

            if (ddlPaymentOptions.SelectedValue == "CreatePayment")
            {
                divCreatePayment.Visible = true;
            }
            if (ddlPaymentOptions.SelectedValue == "GetPaymentById")
            {
                divPaymentReadById.Visible = true;
            }
            if (ddlPaymentOptions.SelectedValue == "PaymentUpdate")
            {
                divUpdatePayment.Visible = true;
            }
            if (ddlPaymentOptions.SelectedValue == "PaymentDelete")
            {
                divPaymentDelete.Visible = true;
            }
            if (ddlPaymentOptions.SelectedValue == "ReadAllPayments")
            {
                divPayments.Visible = true;
            }
        }

        protected void btnCreatePayment_Click(object sender, EventArgs e)
        {
            try
            {
                string responseText = "";
                string uri = string.Format("https://{0}/v3/company/{1}/payment", qboBaseUrl, Session["realmId"]);
                //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/payments/1/bank-accounts/";
                string body = "";
                CreatePaymentReq ccObj = new CreatePaymentReq();
                

                body = JsonConvert.SerializeObject(ccObj);
                // send the request
                responseText = POSTQBAPICall(uri, body);
                if (responseText != null && responseText != "")
                {
                    //CreatePaymentRes ccResObj = new CreatePaymentRes();
                    //ccResObj = JsonConvert.DeserializeObject<CreatePaymentRes>(responseText);
                    //if (ccResObj != null)
                    //{
                    //    if (ccResObj.Payment != null && ccResObj.Payment.Id != null && ccResObj.Payment.Id != "")
                    //    {
                    //        //Payment Created Successfully
                    //        divCreatePayment.Visible = false;

                    //        lblMessage.Text = "Payment Created Successfully";
                    //    }
                    //}
                    //else
                    //{
                    //    lblMessage.Text = responseText;
                    //}
                }
            }
            catch (Exception ex)
            {
            }
        }

        protected void btnGetPaymentById_Click(object sender, EventArgs e)
        {
            try
            {
                string PaymentId = txtPaymentId.Text;
                string responseText = "";
                string uri = string.Format("https://{0}/v3/company/{1}/payment/{2}", qboBaseUrl, Session["realmId"], PaymentId);
                //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/payments/1/bank-accounts/";

                // send the request
                responseText = GETQBAPICall(uri);
                if (responseText != null && responseText != "")
                {
                    PaymentByIdRes ccResObj = new PaymentByIdRes();
                    ccResObj = JsonConvert.DeserializeObject<PaymentByIdRes>(responseText);
                    if (ccResObj != null)
                    {
                        if (ccResObj.Payment != null && ccResObj.Payment.Id != null && ccResObj.Payment.Id != "")
                        {
                            lblMessage.Text = "Payment  Date : " + ccResObj.Payment.TxnDate;
                        }
                    }
                    else
                    {
                        lblMessage.Text = responseText;
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        protected void btnUpdatePayment_Click(object sender, EventArgs e)
        {
            try
            {
                string PaymentId = txtUCPaymentId.Text;
                string firstresponseText = "";
                string firsturi = string.Format("https://{0}/v3/company/{1}/payment/{2}", qboBaseUrl, Session["realmId"], PaymentId);
                //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/payments/1/bank-accounts/";

                // send the request
                firstresponseText = GETQBAPICall(firsturi);
                if (firstresponseText != null && firstresponseText != "")
                {
                    PaymentByIdRes ccResObj = new PaymentByIdRes();
                    ccResObj = JsonConvert.DeserializeObject<PaymentByIdRes>(firstresponseText);
                    if (ccResObj != null)
                    {
                        if (ccResObj.Payment != null && ccResObj.Payment.Id != null && ccResObj.Payment.Id != "")
                        {
                            string responseText = "";
                            string uri = string.Format("https://{0}/v3/company/{1}/payment", qboBaseUrl, Session["realmId"]);
                            //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/payments/1/bank-accounts/";
                            string body = "";
                            UpdatePaymentReq ccObj = new UpdatePaymentReq();


                            ccObj.domain = ccResObj.Payment.domain;
                            ccObj.sparse = ccResObj.Payment.sparse;
                            ccObj.Id = ccResObj.Payment.Id;
                            ccObj.SyncToken = ccResObj.Payment.SyncToken;



                            //Pass Other Parameters if needed.

                            body = JsonConvert.SerializeObject(ccObj);
                            // send the request
                            responseText = POSTQBAPICall(uri, body);
                            if (responseText != null && responseText != "")
                            {
                                lblMessage.Text = "Payment Updated Successfully.";
                            }
                        }
                    }
                    else
                    {
                        lblMessage.Text = firstresponseText;
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        protected void btnPaymentDelete_Click(object sender, EventArgs e)
        {
            try
            {
                string PaymentId = txtVDPaymentId.Text;
                string firstresponseText = "";
                string firsturi = string.Format("https://{0}/v3/company/{1}/payment/{2}", qboBaseUrl, Session["realmId"], PaymentId);
                //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/payments/1/bank-accounts/";

                // send the request
                firstresponseText = GETQBAPICall(firsturi);
                if (firstresponseText != null && firstresponseText != "")
                {
                    PaymentByIdRes ccResObj = new PaymentByIdRes();
                    ccResObj = JsonConvert.DeserializeObject<PaymentByIdRes>(firstresponseText);
                    if (ccResObj != null)
                    {
                        if (ccResObj.Payment != null && ccResObj.Payment.Id != null && ccResObj.Payment.Id != "")
                        {
                            string responseText = "";
                            string uri = string.Format("https://{0}/v3/company/{1}/payment?operation=delete", qboBaseUrl, Session["realmId"]);
                            //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/payments/1/bank-accounts/";
                            string body = "";

                            PaymentDeleteReq ccObj = new PaymentDeleteReq();
                            ccObj.Id = ccResObj.Payment.Id;
                            ccObj.SyncToken = ccResObj.Payment.SyncToken;

                            //Other Fields

                            body = JsonConvert.SerializeObject(ccObj);
                            // send the request
                            responseText = POSTQBAPICall(uri, body);
                            if (responseText != null && responseText != "")
                            {
                                lblMessage.Text = "Payment deleted successfully";
                            }
                        }
                    }
                    else
                    {
                        lblMessage.Text = firstresponseText;
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        protected void btnGetPayments_Click(object sender, EventArgs e)
        {
            try
            {
                List<ReadAllPaymentSummary> paymentList = new List<ReadAllPaymentSummary>();
                paymentList = GetPayments();
                if (paymentList != null && paymentList.Count > 0)
                {
                    gvwPayments.DataSource = paymentList;
                    gvwPayments.DataBind();
                }
                return;
            }
            catch (Exception ex)
            {
            }
        }

        public List<ReadAllPaymentSummary> GetPayments()
        {
            string responseText = "";
            List<ReadAllPaymentSummary> paymentList = new List<ReadAllPaymentSummary>();
            try
            {
                string query = "Select * from Payment startposition 1 maxresults 5";
                // build the  request
                string encodedQuery = WebUtility.UrlEncode(query);

                //add qbobase url and query
                string uri = string.Format("https://{0}/v3/company/{1}/query?query={2}", qboBaseUrl, Session["realmId"], encodedQuery);
                //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/payments/1/bank-accounts/";
                // send the request
                responseText = GETQBAPICall(uri);
                if (responseText != null && responseText != "")
                {
                    ReadAllPaymentRes custObj = JsonConvert.DeserializeObject<ReadAllPaymentRes>(responseText);
                    foreach (var item in custObj.QueryResponse.Payment)
                    {
                        ReadAllPaymentSummary paymentObj = new ReadAllPaymentSummary();
                        paymentObj.Id = item.Id;
                        paymentObj.CustomerName = item.CustomerRef.name;
                        paymentObj.Amount = item.TotalAmt.ToString();
                        paymentObj.TransactionDate = item.TxnDate;
                        paymentList.Add(paymentObj);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return paymentList;
        }

        #endregion CustomerPayments

        #region Account

        protected void ddlAccountOptions_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblMessage.Text = "";
            divCreateAccount.Visible = false;
            divAccountReadById.Visible = false;
            divUpdateAccount.Visible = false;
            divAccounts.Visible = false;

            if (ddlAccountOptions.SelectedValue == "CreateAccount")
            {
                divCreateAccount.Visible = true;
            }
            if (ddlAccountOptions.SelectedValue == "GetAccountById")
            {
                divAccountReadById.Visible = true;
            }
            if (ddlAccountOptions.SelectedValue == "AccountUpdate")
            {
                divUpdateAccount.Visible = true;
            }
            if (ddlAccountOptions.SelectedValue == "ReadAllAccounts")
            {
                divAccounts.Visible = true;
            }
        }

        protected void btnCreateAccount_Click(object sender, EventArgs e)
        {
            try
            {
                string responseText = "";
                string uri = string.Format("https://{0}/v3/company/{1}/account?minorversion=4", qboBaseUrl, Session["realmId"]);
                //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/accounts/1/bank-accounts/";
                string body = "";
                CreateAccountReq ccObj = new CreateAccountReq();
                ccObj.AccountType = ddlAccountType.SelectedValue;
                ccObj.Name = txtAccountName.Text;
                body = JsonConvert.SerializeObject(ccObj);
                // send the request
                responseText = POSTQBAPICall(uri, body);
                if (responseText != null && responseText != "")
                {
                    CreateAccountRes ccResObj = new CreateAccountRes();
                    ccResObj = JsonConvert.DeserializeObject<CreateAccountRes>(responseText);
                    if (ccResObj != null)
                    {
                        if (ccResObj.Account != null && ccResObj.Account.Id != null && ccResObj.Account.Id != "")
                        {
                            //Account Created Successfully
                            divCreateAccount.Visible = false;
                            txtAccountName.Text = "";
                            lblMessage.Text = "Account Created Successfully";
                        }
                    }
                    else
                    {
                        lblMessage.Text = responseText;
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        protected void btnGetAccountById_Click(object sender, EventArgs e)
        {
            try
            {
                string AccountId = txtAccountId.Text;
                string responseText = "";
                string uri = string.Format("https://{0}/v3/company/{1}/account/{2}", qboBaseUrl, Session["realmId"], AccountId);
                //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/accounts/1/bank-accounts/";

                // send the request
                responseText = GETQBAPICall(uri);
                if (responseText != null && responseText != "")
                {
                    AccountByIdRes ccResObj = new AccountByIdRes();
                    ccResObj = JsonConvert.DeserializeObject<AccountByIdRes>(responseText);
                    if (ccResObj != null)
                    {
                        if (ccResObj.Account != null && ccResObj.Account.Id != null && ccResObj.Account.Id != "")
                        {
                            lblMessage.Text = "Account  Name : " + ccResObj.Account.Name;
                        }
                    }
                    else
                    {
                        lblMessage.Text = responseText;
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        protected void btnUpdateAccount_Click(object sender, EventArgs e)
        {
            try
            {
                string AccountId = txtUCAccountId.Text;
                string firstresponseText = "";
                string firsturi = string.Format("https://{0}/v3/company/{1}/account/{2}", qboBaseUrl, Session["realmId"], AccountId);
                //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/accounts/1/bank-accounts/";

                // send the request
                firstresponseText = GETQBAPICall(firsturi);
                if (firstresponseText != null && firstresponseText != "")
                {
                    AccountByIdRes ccResObj = new AccountByIdRes();
                    ccResObj = JsonConvert.DeserializeObject<AccountByIdRes>(firstresponseText);
                    if (ccResObj != null)
                    {
                        if (ccResObj.Account != null && ccResObj.Account.Id != null && ccResObj.Account.Id != "")
                        {
                            string responseText = "";
                            string uri = string.Format("https://{0}/v3/company/{1}/account", qboBaseUrl, Session["realmId"]);
                            //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/accounts/1/bank-accounts/";
                            string body = "";

                            UpdateAccountReq ccObj = new UpdateAccountReq();
                            ccObj.AccountSubType = ccResObj.Account.AccountSubType;
                            ccObj.AccountType = ccResObj.Account.AccountType;
                            ccObj.Active = ccResObj.Account.Active;
                            ccObj.Classification = ccResObj.Account.Classification;
                            ccObj.CurrentBalance = ccResObj.Account.CurrentBalance;
                            ccObj.CurrentBalanceWithSubAccounts = ccResObj.Account.CurrentBalanceWithSubAccounts;
                            ccObj.Name = txtUCAccountName.Text;
                            ccObj.SubAccount = ccResObj.Account.SubAccount;
                            ccObj.CurrencyRef = new UpdateAccountReqCurrencyRef();
                            ccObj.CurrencyRef.name = ccResObj.Account.CurrencyRef.name;
                            ccObj.CurrencyRef.value = ccResObj.Account.CurrencyRef.value;
                            ccObj.domain = ccResObj.Account.domain;
                            ccObj.sparse = ccResObj.Account.sparse;
                            ccObj.Id = ccResObj.Account.Id;
                            ccObj.SyncToken = ccResObj.Account.SyncToken;
                            ccObj.FullyQualifiedName = ccResObj.Account.FullyQualifiedName;
                            body = JsonConvert.SerializeObject(ccObj);
                            // send the request
                            responseText = POSTQBAPICall(uri, body);
                            if (responseText != null && responseText != "")
                            {
                            }
                        }
                    }
                    else
                    {
                        lblMessage.Text = firstresponseText;
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        protected void btnGetAccounts_Click(object sender, EventArgs e)
        {
            try
            {
                List<AccountsSummary> accountList = new List<AccountsSummary>();
                accountList = GetAccounts();
                if (accountList != null && accountList.Count > 0)
                {
                    gvwAccounts.DataSource = accountList;
                    gvwAccounts.DataBind();
                }
                return;
            }
            catch (Exception ex)
            {
            }
        }

        public List<AccountsSummary> GetAccounts()
        {
            string responseText = "";
            List<AccountsSummary> accountList = new List<AccountsSummary>();
            try
            {
                string query = "Select * from Account startposition 1 maxresults 5";
                // build the  request
                string encodedQuery = WebUtility.UrlEncode(query);

                //add qbobase url and query
                string uri = string.Format("https://{0}/v3/company/{1}/query?query={2}", qboBaseUrl, Session["realmId"], encodedQuery);
                //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/accounts/1/bank-accounts/";
                // send the request
                responseText = GETQBAPICall(uri);
                if (responseText != null && responseText != "")
                {
                    ReadAllAccountRes custObj = JsonConvert.DeserializeObject<ReadAllAccountRes>(responseText);

                    foreach (var item in custObj.QueryResponse.Account)
                    {
                        AccountsSummary accountObj = new AccountsSummary();
                        accountObj.id = item.Id;
                        accountObj.name = item.Name;
                        accountList.Add(accountObj);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return accountList;
        }

        #endregion Account

        #region Vendor

        protected void ddlVendorOptions_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblMessage.Text = "";
            divCreateVendor.Visible = false;
            divVendorReadById.Visible = false;
            divUpdateVendor.Visible = false;
            divVendorDelete.Visible = false;
            divVendors.Visible = false;

            if (ddlVendorOptions.SelectedValue == "CreateVendor")
            {
                divCreateVendor.Visible = true;
            }
            if (ddlVendorOptions.SelectedValue == "GetVendorById")
            {
                divVendorReadById.Visible = true;
            }
            if (ddlVendorOptions.SelectedValue == "VendorUpdate")
            {
                divUpdateVendor.Visible = true;
            }
            if (ddlVendorOptions.SelectedValue == "VendorDelete")
            {
                divVendorDelete.Visible = true;
            }
            if (ddlVendorOptions.SelectedValue == "ReadAllVendors")
            {
                divVendors.Visible = true;
            }
        }

        protected void btnCreateVendor_Click(object sender, EventArgs e)
        {
            try
            {
                string responseText = "";
                string uri = string.Format("https://{0}/v3/company/{1}/vendor", qboBaseUrl, Session["realmId"]);
                //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/vendors/1/bank-accounts/";
                string body = "";
                CreateVendorReq ccObj = new CreateVendorReq();
                ccObj.BillAddr = new CreateVendorReqBillAddr();
                ccObj.BillAddr.Line1 = txtVendorBillingAddressLine1.Text;
                ccObj.BillAddr.City = txtVendorBillingAddressCity.Text;
                ccObj.BillAddr.Country = txtVendorBillingAddressCountry.Text;
                ccObj.DisplayName = txtVendorName.Text;
                body = JsonConvert.SerializeObject(ccObj);
                // send the request
                responseText = POSTQBAPICall(uri, body);
                if (responseText != null && responseText != "")
                {
                    CreateVendorRes ccResObj = new CreateVendorRes();
                    ccResObj = JsonConvert.DeserializeObject<CreateVendorRes>(responseText);
                    if (ccResObj != null)
                    {
                        if (ccResObj.Vendor != null && ccResObj.Vendor.Id != null && ccResObj.Vendor.Id != "")
                        {
                            //Vendor Created Successfully
                            divCreateVendor.Visible = false;
                            txtVendorBillingAddressCity.Text = "";
                            txtVendorBillingAddressCountry.Text = "";
                            txtVendorBillingAddressLine1.Text = "";
                            txtVendorName.Text = "";
                            lblMessage.Text = "Vendor Created Successfully";
                        }
                    }
                    else
                    {
                        lblMessage.Text = responseText;
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        protected void btnGetVendorById_Click(object sender, EventArgs e)
        {
            try
            {
                string VendorId = txtVendorId.Text;
                string responseText = "";
                string uri = string.Format("https://{0}/v3/company/{1}/vendor/{2}", qboBaseUrl, Session["realmId"], VendorId);
                //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/vendors/1/bank-accounts/";

                // send the request
                responseText = GETQBAPICall(uri);
                if (responseText != null && responseText != "")
                {
                    VendorByIdRes ccResObj = new VendorByIdRes();
                    ccResObj = JsonConvert.DeserializeObject<VendorByIdRes>(responseText);
                    if (ccResObj != null)
                    {
                        if (ccResObj.Vendor != null && ccResObj.Vendor.Id != null && ccResObj.Vendor.Id != "")
                        {
                            lblMessage.Text = "Vendor  Name : " + ccResObj.Vendor.DisplayName;
                        }
                    }
                    else
                    {
                        lblMessage.Text = responseText;
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        protected void btnUpdateVendor_Click(object sender, EventArgs e)
        {
            try
            {
                string VendorId = txtUCVendorId.Text;
                string firstresponseText = "";
                string firsturi = string.Format("https://{0}/v3/company/{1}/vendor/{2}", qboBaseUrl, Session["realmId"], VendorId);
                //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/vendors/1/bank-accounts/";

                // send the request
                firstresponseText = GETQBAPICall(firsturi);
                if (firstresponseText != null && firstresponseText != "")
                {
                    VendorByIdRes ccResObj = new VendorByIdRes();
                    ccResObj = JsonConvert.DeserializeObject<VendorByIdRes>(firstresponseText);
                    if (ccResObj != null)
                    {
                        if (ccResObj.Vendor != null && ccResObj.Vendor.Id != null && ccResObj.Vendor.Id != "")
                        {
                            string responseText = "";
                            string uri = string.Format("https://{0}/v3/company/{1}/vendor", qboBaseUrl, Session["realmId"]);
                            //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/vendors/1/bank-accounts/";
                            string body = "";
                            UpdateVendorReq ccObj = new UpdateVendorReq();
                            ccObj.GivenName = txtUCVendorName.Text;
                            ccObj.domain = ccResObj.Vendor.domain;
                            ccObj.sparse = ccResObj.Vendor.sparse;
                            ccObj.Id = ccResObj.Vendor.Id;
                            ccObj.SyncToken = ccResObj.Vendor.SyncToken;
                            ccObj.Active = ccResObj.Vendor.Active;

                            //Pass Other Parameters if needed.

                            body = JsonConvert.SerializeObject(ccObj);
                            // send the request
                            responseText = POSTQBAPICall(uri, body);
                            if (responseText != null && responseText != "")
                            {
                                lblMessage.Text = "Vendor Updated Successfully.";
                            }
                        }
                    }
                    else
                    {
                        lblMessage.Text = firstresponseText;
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        protected void btnVendorDelete_Click(object sender, EventArgs e)
        {
            try
            {
                string VendorId = txtVDVendorId.Text;
                string firstresponseText = "";
                string firsturi = string.Format("https://{0}/v3/company/{1}/vendor/{2}", qboBaseUrl, Session["realmId"], VendorId);
                //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/vendors/1/bank-accounts/";

                // send the request
                firstresponseText = GETQBAPICall(firsturi);
                if (firstresponseText != null && firstresponseText != "")
                {
                    VendorByIdRes ccResObj = new VendorByIdRes();
                    ccResObj = JsonConvert.DeserializeObject<VendorByIdRes>(firstresponseText);
                    if (ccResObj != null)
                    {
                        if (ccResObj.Vendor != null && ccResObj.Vendor.Id != null && ccResObj.Vendor.Id != "")
                        {
                            string responseText = "";
                            string uri = string.Format("https://{0}/v3/company/{1}/vendor", qboBaseUrl, Session["realmId"]);
                            //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/vendors/1/bank-accounts/";
                            string body = "";

                            VendorDeleteReq ccObj = new VendorDeleteReq();
                            ccObj.domain = ccResObj.Vendor.domain;
                            ccObj.sparse = true;
                            ccObj.Id = ccResObj.Vendor.Id;
                            ccObj.SyncToken = ccResObj.Vendor.SyncToken;
                            ccObj.Active = false;

                            //Other Fields

                            body = JsonConvert.SerializeObject(ccObj);
                            // send the request
                            responseText = POSTQBAPICall(uri, body);
                            if (responseText != null && responseText != "")
                            {
                                lblMessage.Text = "Vendor made inactive successfully";
                            }
                        }
                    }
                    else
                    {
                        lblMessage.Text = firstresponseText;
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        protected void btnGetVendors_Click(object sender, EventArgs e)
        {
            try
            {
                List<VendorsSummary> vendorList = new List<VendorsSummary>();
                vendorList = GetVendors();
                if (vendorList != null && vendorList.Count > 0)
                {
                    gvwVendors.DataSource = vendorList;
                    gvwVendors.DataBind();
                }
                return;
            }
            catch (Exception ex)
            {
            }
        }

        public List<VendorsSummary> GetVendors()
        {
            string responseText = "";
            List<VendorsSummary> vendorList = new List<VendorsSummary>();
            try
            {
                string query = "Select * from Vendor startposition 1 maxresults 5";
                // build the  request
                string encodedQuery = WebUtility.UrlEncode(query);

                //add qbobase url and query
                string uri = string.Format("https://{0}/v3/company/{1}/query?query={2}", qboBaseUrl, Session["realmId"], encodedQuery);
                //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/vendors/1/bank-accounts/";
                // send the request
                responseText = GETQBAPICall(uri);
                if (responseText != null && responseText != "")
                {
                    ReadAllVendorRes custObj = JsonConvert.DeserializeObject<ReadAllVendorRes>(responseText);

                    foreach (var item in custObj.QueryResponse.Vendor)
                    {
                        VendorsSummary vendorObj = new VendorsSummary();
                        vendorObj.id = item.Id;
                        vendorObj.name = item.DisplayName;
                        vendorList.Add(vendorObj);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return vendorList;
        }

        #endregion
        #region Bill

        protected void ddlBillOptions_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblMessage.Text = "";
            divCreateBill.Visible = false;
            divBillReadById.Visible = false;
            divUpdateBill.Visible = false;
            divBillDelete.Visible = false;
            divBills.Visible = false;

            if (ddlBillOptions.SelectedValue == "CreateBill")
            {
                divCreateBill.Visible = true;
            }
            if (ddlBillOptions.SelectedValue == "GetBillById")
            {
                divBillReadById.Visible = true;
            }
            if (ddlBillOptions.SelectedValue == "BillUpdate")
            {
                divUpdateBill.Visible = true;
            }
            if (ddlBillOptions.SelectedValue == "BillDelete")
            {
                divBillDelete.Visible = true;
            }
            if (ddlBillOptions.SelectedValue == "ReadAllBills")
            {
                divBills.Visible = true;
            }
        }

        protected void btnCreateBill_Click(object sender, EventArgs e)
        {
            try
            {
                string responseText = "";
                string uri = string.Format("https://{0}/v3/company/{1}/bill", qboBaseUrl, Session["realmId"]);
                //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/bills/1/bank-accounts/";
                string body = "";
                CreateBillReq ccObj = new CreateBillReq();
                ccObj.Line = new List<CreateBillReqLine>();
                CreateBillReqLine lineObj = new CreateBillReqLine();

                  body = JsonConvert.SerializeObject(ccObj);
                // send the request
                responseText = POSTQBAPICall(uri, body);
                if (responseText != null && responseText != "")
                {
                    CreateBillRes ccResObj = new CreateBillRes();
                    ccResObj = JsonConvert.DeserializeObject<CreateBillRes>(responseText);
                    if (ccResObj != null)
                    {
                        if (ccResObj.Bill != null && ccResObj.Bill.Id != null && ccResObj.Bill.Id != "")
                        {
                            //Bill Created Successfully
                            divCreateBill.Visible = false;
                         
                            lblMessage.Text = "Bill Created Successfully";
                        }
                    }
                    else
                    {
                        lblMessage.Text = responseText;
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        protected void btnGetBillById_Click(object sender, EventArgs e)
        {
            try
            {
                string BillId = txtBillId.Text;
                string responseText = "";
                string uri = string.Format("https://{0}/v3/company/{1}/bill/{2}", qboBaseUrl, Session["realmId"], BillId);
                //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/bills/1/bank-accounts/";

                // send the request
                responseText = GETQBAPICall(uri);
                if (responseText != null && responseText != "")
                {
                    BillByIdRes ccResObj = new BillByIdRes();
                    ccResObj = JsonConvert.DeserializeObject<BillByIdRes>(responseText);
                    if (ccResObj != null)
                    {
                        if (ccResObj.Bill != null && ccResObj.Bill.Id != null && ccResObj.Bill.Id != "")
                        {
                            lblMessage.Text = "Customer  Name For Bill: " + ccResObj.Bill.VendorRef.name;
                        }
                    }
                    else
                    {
                        lblMessage.Text = responseText;
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        protected void btnUpdateBill_Click(object sender, EventArgs e)
        {
            try
            {
                string BillId = txtUCBillId.Text;
                string firstresponseText = "";
                string firsturi = string.Format("https://{0}/v3/company/{1}/bill/{2}", qboBaseUrl, Session["realmId"], BillId);
                //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/bills/1/bank-accounts/";

                // send the request
                firstresponseText = GETQBAPICall(firsturi);
                if (firstresponseText != null && firstresponseText != "")
                {
                    BillByIdRes ccResObj = new BillByIdRes();
                    ccResObj = JsonConvert.DeserializeObject<BillByIdRes>(firstresponseText);
                    if (ccResObj != null)
                    {
                        if (ccResObj.Bill != null && ccResObj.Bill.Id != null && ccResObj.Bill.Id != "")
                        {
                            string responseText = "";
                            string uri = string.Format("https://{0}/v3/company/{1}/bill", qboBaseUrl, Session["realmId"]);
                            //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/bills/1/bank-accounts/";
                            string body = "";
                            UpdateBillReq ccObj = new UpdateBillReq();

                            //Pass Other Parameters if needed.

                            body = JsonConvert.SerializeObject(ccObj);
                            // send the request
                            responseText = POSTQBAPICall(uri, body);
                            if (responseText != null && responseText != "")
                            {
                                lblMessage.Text = "Bill Updated Successfully.";
                            }
                        }
                    }
                    else
                    {
                        lblMessage.Text = firstresponseText;
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        protected void btnBillDelete_Click(object sender, EventArgs e)
        {
            try
            {
                string BillId = txtVDBillId.Text;
                string firstresponseText = "";
                string firsturi = string.Format("https://{0}/v3/company/{1}/bill/{2}", qboBaseUrl, Session["realmId"], BillId);
                //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/bills/1/bank-accounts/";

                // send the request
                firstresponseText = GETQBAPICall(firsturi);
                if (firstresponseText != null && firstresponseText != "")
                {
                    BillByIdRes ccResObj = new BillByIdRes();
                    ccResObj = JsonConvert.DeserializeObject<BillByIdRes>(firstresponseText);
                    if (ccResObj != null)
                    {
                        if (ccResObj.Bill != null && ccResObj.Bill.Id != null && ccResObj.Bill.Id != "")
                        {
                            string responseText = "";
                            string uri = string.Format("https://{0}/v3/company/{1}/bill?operation=delete", qboBaseUrl, Session["realmId"]);
                            //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/bills/1/bank-accounts/";
                            string body = "";

                            BillDeleteReq ccObj = new BillDeleteReq();
                            ccObj.Id = ccResObj.Bill.Id;
                            ccObj.SyncToken = ccResObj.Bill.SyncToken;

                            //Other Fields

                            body = JsonConvert.SerializeObject(ccObj);
                            // send the request
                            responseText = POSTQBAPICall(uri, body);
                            if (responseText != null && responseText != "")
                            {
                                lblMessage.Text = "Bill made inactive successfully";
                            }
                        }
                    }
                    else
                    {
                        lblMessage.Text = firstresponseText;
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        protected void btnGetBills_Click(object sender, EventArgs e)
        {
            try
            {
                List<BillsSummary> billList = new List<BillsSummary>();
                billList = GetBills();
                if (billList != null && billList.Count > 0)
                {
                    gvwBills.DataSource = billList;
                    gvwBills.DataBind();
                }
                return;
            }
            catch (Exception ex)
            {
            }
        }

        public List<BillsSummary> GetBills()
        {
            string responseText = "";
            List<BillsSummary> billList = new List<BillsSummary>();
            try
            {
                string query = "Select * from Bill startposition 1 maxresults 5";
                // build the  request
                string encodedQuery = WebUtility.UrlEncode(query);

                //add qbobase url and query
                string uri = string.Format("https://{0}/v3/company/{1}/query?query={2}", qboBaseUrl, Session["realmId"], encodedQuery);
                //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/bills/1/bank-accounts/";
                // send the request
                responseText = GETQBAPICall(uri);
                if (responseText != null && responseText != "")
                {
                    ReadAllBillRes custObj = JsonConvert.DeserializeObject<ReadAllBillRes>(responseText);

                    foreach (var item in custObj.QueryResponse.Bill)
                    {
                        BillsSummary billObj = new BillsSummary();
                        billObj.id = item.Id;
                        billObj.txndate = item.TxnDate;
                        billList.Add(billObj);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return billList;
        }

        #endregion
        #region BillPayment

        protected void ddlBillPaymentOptions_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblMessage.Text = "";
            divCreateBillPayment.Visible = false;
            divBillPaymentReadById.Visible = false;
            divUpdateBillPayment.Visible = false;
            divBillPaymentDelete.Visible = false;
            divBillPayments.Visible = false;

            if (ddlBillPaymentOptions.SelectedValue == "CreateBillPayment")
            {
                divCreateBillPayment.Visible = true;
            }
            if (ddlBillPaymentOptions.SelectedValue == "GetBillPaymentById")
            {
                divBillPaymentReadById.Visible = true;
            }
            if (ddlBillPaymentOptions.SelectedValue == "BillPaymentUpdate")
            {
                divUpdateBillPayment.Visible = true;
            }
            if (ddlBillPaymentOptions.SelectedValue == "BillPaymentDelete")
            {
                divBillPaymentDelete.Visible = true;
            }
            if (ddlBillPaymentOptions.SelectedValue == "ReadAllBillPayments")
            {
                divBillPayments.Visible = true;
            }
        }

        protected void btnCreateBillPayment_Click(object sender, EventArgs e)
        {
            try
            {
                string responseText = "";
                string uri = string.Format("https://{0}/v3/company/{1}/billPayment", qboBaseUrl, Session["realmId"]);
                //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/billPayments/1/bank-accounts/";
                string body = "";
                CreateBillPaymentReq ccObj = new CreateBillPaymentReq();
                ccObj.Line = new List<CreateBillPaymentReqLine>();
                CreateBillPaymentReqLine lineObj = new CreateBillPaymentReqLine();

                body = JsonConvert.SerializeObject(ccObj);
                // send the request
                responseText = POSTQBAPICall(uri, body);
                if (responseText != null && responseText != "")
                {
                    //CreateBillPaymentRes ccResObj = new CreateBillPaymentRes();
                    //ccResObj = JsonConvert.DeserializeObject<CreateBillPaymentRes>(responseText);
                    //if (ccResObj != null)
                    //{
                    //    if (ccResObj.BillPayment != null && ccResObj.BillPayment.Id != null && ccResObj.BillPayment.Id != "")
                    //    {
                    //        //BillPayment Created Successfully
                    //        divCreateBillPayment.Visible = false;

                    //        lblMessage.Text = "BillPayment Created Successfully";
                    //    }
                    //}
                    //else
                    //{
                    //    lblMessage.Text = responseText;
                    //}
                }
            }
            catch (Exception ex)
            {
            }
        }

        protected void btnGetBillPaymentById_Click(object sender, EventArgs e)
        {
            try
            {
                string BillPaymentId = txtBillPaymentId.Text;
                string responseText = "";
                string uri = string.Format("https://{0}/v3/company/{1}/billPayment/{2}", qboBaseUrl, Session["realmId"], BillPaymentId);
                //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/billPayments/1/bank-accounts/";

                // send the request
                responseText = GETQBAPICall(uri);
                if (responseText != null && responseText != "")
                {
                    BillPaymentByIdRes ccResObj = new BillPaymentByIdRes();
                    ccResObj = JsonConvert.DeserializeObject<BillPaymentByIdRes>(responseText);
                    if (ccResObj != null)
                    {
                        if (ccResObj.BillPayment != null && ccResObj.BillPayment.Id != null && ccResObj.BillPayment.Id != "")
                        {
                            lblMessage.Text = "Customer  Name For BillPayment: " + ccResObj.BillPayment.VendorRef.name;
                        }
                    }
                    else
                    {
                        lblMessage.Text = responseText;
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        protected void btnUpdateBillPayment_Click(object sender, EventArgs e)
        {
            try
            {
                string BillPaymentId = txtUCBillPaymentId.Text;
                string firstresponseText = "";
                string firsturi = string.Format("https://{0}/v3/company/{1}/billPayment/{2}", qboBaseUrl, Session["realmId"], BillPaymentId);
                //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/billPayments/1/bank-accounts/";

                // send the request
                firstresponseText = GETQBAPICall(firsturi);
                if (firstresponseText != null && firstresponseText != "")
                {
                    BillPaymentByIdRes ccResObj = new BillPaymentByIdRes();
                    ccResObj = JsonConvert.DeserializeObject<BillPaymentByIdRes>(firstresponseText);
                    if (ccResObj != null)
                    {
                        if (ccResObj.BillPayment != null && ccResObj.BillPayment.Id != null && ccResObj.BillPayment.Id != "")
                        {
                            string responseText = "";
                            string uri = string.Format("https://{0}/v3/company/{1}/billPayment", qboBaseUrl, Session["realmId"]);
                            //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/billPayments/1/bank-accounts/";
                            string body = "";
                            UpdateBillPaymentReq ccObj = new UpdateBillPaymentReq();

                            //Pass Other Parameters if needed.

                            body = JsonConvert.SerializeObject(ccObj);
                            // send the request
                            responseText = POSTQBAPICall(uri, body);
                            if (responseText != null && responseText != "")
                            {
                                lblMessage.Text = "BillPayment Updated Successfully.";
                            }
                        }
                    }
                    else
                    {
                        lblMessage.Text = firstresponseText;
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        protected void btnBillPaymentDelete_Click(object sender, EventArgs e)
        {
            try
            {
                string BillPaymentId = txtVDBillPaymentId.Text;
                string firstresponseText = "";
                string firsturi = string.Format("https://{0}/v3/company/{1}/billPayment/{2}", qboBaseUrl, Session["realmId"], BillPaymentId);
                //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/billPayments/1/bank-accounts/";

                // send the request
                firstresponseText = GETQBAPICall(firsturi);
                if (firstresponseText != null && firstresponseText != "")
                {
                    BillPaymentByIdRes ccResObj = new BillPaymentByIdRes();
                    ccResObj = JsonConvert.DeserializeObject<BillPaymentByIdRes>(firstresponseText);
                    if (ccResObj != null)
                    {
                        if (ccResObj.BillPayment != null && ccResObj.BillPayment.Id != null && ccResObj.BillPayment.Id != "")
                        {
                            string responseText = "";
                            string uri = string.Format("https://{0}/v3/company/{1}/billPayment?operation=delete", qboBaseUrl, Session["realmId"]);
                            //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/billPayments/1/bank-accounts/";
                            string body = "";

                            BillPaymentDeleteReq ccObj = new BillPaymentDeleteReq();
                            ccObj.Id = ccResObj.BillPayment.Id;
                            ccObj.SyncToken = ccResObj.BillPayment.SyncToken;

                            //Other Fields

                            body = JsonConvert.SerializeObject(ccObj);
                            // send the request
                            responseText = POSTQBAPICall(uri, body);
                            if (responseText != null && responseText != "")
                            {
                                lblMessage.Text = "BillPayment made inactive successfully";
                            }
                        }
                    }
                    else
                    {
                        lblMessage.Text = firstresponseText;
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        protected void btnGetBillPayments_Click(object sender, EventArgs e)
        {
            try
            {
                List<BillPaymentsSummary> billPaymentList = new List<BillPaymentsSummary>();
                billPaymentList = GetBillPayments();
                if (billPaymentList != null && billPaymentList.Count > 0)
                {
                    gvwBillPayments.DataSource = billPaymentList;
                    gvwBillPayments.DataBind();
                }
                return;
            }
            catch (Exception ex)
            {
            }
        }

        public List<BillPaymentsSummary> GetBillPayments()
        {
            string responseText = "";
            List<BillPaymentsSummary> billPaymentList = new List<BillPaymentsSummary>();
            try
            {
                string query = "Select * from BillPayment startposition 1 maxresults 5";
                // build the  request
                string encodedQuery = WebUtility.UrlEncode(query);

                //add qbobase url and query
                string uri = string.Format("https://{0}/v3/company/{1}/query?query={2}", qboBaseUrl, Session["realmId"], encodedQuery);
                //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/billPayments/1/bank-accounts/";
                // send the request
                responseText = GETQBAPICall(uri);
                if (responseText != null && responseText != "")
                {
                    ReadAllBillPaymentRes custObj = JsonConvert.DeserializeObject<ReadAllBillPaymentRes>(responseText);

                    foreach (var item in custObj.QueryResponse.BillPayment)
                    {
                        BillPaymentsSummary billPaymentObj = new BillPaymentsSummary();
                        billPaymentObj.id = item.Id;
                        billPaymentObj.txndate = item.TxnDate;
                        billPaymentList.Add(billPaymentObj);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return billPaymentList;
        }

        #endregion
        #region Invoice

        protected void ddlInvoiceOptions_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblMessage.Text = "";
            divCreateInvoice.Visible = false;
            divInvoiceReadById.Visible = false;
            divUpdateInvoice.Visible = false;
            divInvoiceDelete.Visible = false;
            divInvoices.Visible = false;

            if (ddlInvoiceOptions.SelectedValue == "CreateInvoice")
            {
                divCreateInvoice.Visible = true;
            }
            if (ddlInvoiceOptions.SelectedValue == "GetInvoiceById")
            {
                divInvoiceReadById.Visible = true;
            }
            if (ddlInvoiceOptions.SelectedValue == "InvoiceUpdate")
            {
                divUpdateInvoice.Visible = true;
            }
            if (ddlInvoiceOptions.SelectedValue == "InvoiceDelete")
            {
                divInvoiceDelete.Visible = true;
            }
            if (ddlInvoiceOptions.SelectedValue == "ReadAllInvoices")
            {
                divInvoices.Visible = true;
            }
        }

        protected void btnCreateInvoice_Click(object sender, EventArgs e)
        {
            try
            {
                string responseText = "";
                string uri = string.Format("https://{0}/v3/company/{1}/invoice", qboBaseUrl, Session["realmId"]);
                //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/invoices/1/bank-accounts/";
                string body = "";
                CreateInvoiceReq ccObj = new CreateInvoiceReq();
                ccObj.Line = new List<CreateInvoiceReqLine>();
                CreateInvoiceReqLine lineObj = new CreateInvoiceReqLine();

                body = JsonConvert.SerializeObject(ccObj);
                // send the request
                responseText = POSTQBAPICall(uri, body);
                if (responseText != null && responseText != "")
                {
                    //CreateInvoiceRes ccResObj = new CreateInvoiceRes();
                    //ccResObj = JsonConvert.DeserializeObject<CreateInvoiceRes>(responseText);
                    //if (ccResObj != null)
                    //{
                    //    if (ccResObj.Invoice != null && ccResObj.Invoice.Id != null && ccResObj.Invoice.Id != "")
                    //    {
                    //        //Invoice Created Successfully
                    //        divCreateInvoice.Visible = false;

                    //        lblMessage.Text = "Invoice Created Successfully";
                    //    }
                    //}
                    //else
                    //{
                    //    lblMessage.Text = responseText;
                    //}
                }
            }
            catch (Exception ex)
            {
            }
        }

        protected void btnGetInvoiceById_Click(object sender, EventArgs e)
        {
            try
            {
                string InvoiceId = txtInvoiceId.Text;
                string responseText = "";
                string uri = string.Format("https://{0}/v3/company/{1}/invoice/{2}", qboBaseUrl, Session["realmId"], InvoiceId);
                //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/invoices/1/bank-accounts/";

                // send the request
                responseText = GETQBAPICall(uri);
                if (responseText != null && responseText != "")
                {
                    InvoiceByIdRes ccResObj = new InvoiceByIdRes();
                    ccResObj = JsonConvert.DeserializeObject<InvoiceByIdRes>(responseText);
                    if (ccResObj != null)
                    {
                        if (ccResObj.Invoice != null && ccResObj.Invoice.Id != null && ccResObj.Invoice.Id != "")
                        {
                            lblMessage.Text = "Customer  Name For Invoice: " + ccResObj.Invoice.CustomerRef.name;
                        }
                    }
                    else
                    {
                        lblMessage.Text = responseText;
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        protected void btnUpdateInvoice_Click(object sender, EventArgs e)
        {
            try
            {
                string InvoiceId = txtUCInvoiceId.Text;
                string firstresponseText = "";
                string firsturi = string.Format("https://{0}/v3/company/{1}/invoice/{2}", qboBaseUrl, Session["realmId"], InvoiceId);
                //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/invoices/1/bank-accounts/";

                // send the request
                firstresponseText = GETQBAPICall(firsturi);
                if (firstresponseText != null && firstresponseText != "")
                {
                    InvoiceByIdRes ccResObj = new InvoiceByIdRes();
                    ccResObj = JsonConvert.DeserializeObject<InvoiceByIdRes>(firstresponseText);
                    if (ccResObj != null)
                    {
                        if (ccResObj.Invoice != null && ccResObj.Invoice.Id != null && ccResObj.Invoice.Id != "")
                        {
                            string responseText = "";
                            string uri = string.Format("https://{0}/v3/company/{1}/invoice", qboBaseUrl, Session["realmId"]);
                            //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/invoices/1/bank-accounts/";
                            string body = "";
                            UpdateInvoiceReq ccObj = new UpdateInvoiceReq();

                            //Pass Other Parameters if needed.

                            body = JsonConvert.SerializeObject(ccObj);
                            // send the request
                            responseText = POSTQBAPICall(uri, body);
                            if (responseText != null && responseText != "")
                            {
                                lblMessage.Text = "Invoice Updated Successfully.";
                            }
                        }
                    }
                    else
                    {
                        lblMessage.Text = firstresponseText;
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        protected void btnInvoiceDelete_Click(object sender, EventArgs e)
        {
            try
            {
                string InvoiceId = txtVDInvoiceId.Text;
                string firstresponseText = "";
                string firsturi = string.Format("https://{0}/v3/company/{1}/invoice/{2}", qboBaseUrl, Session["realmId"], InvoiceId);
                //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/invoices/1/bank-accounts/";

                // send the request
                firstresponseText = GETQBAPICall(firsturi);
                if (firstresponseText != null && firstresponseText != "")
                {
                    InvoiceByIdRes ccResObj = new InvoiceByIdRes();
                    ccResObj = JsonConvert.DeserializeObject<InvoiceByIdRes>(firstresponseText);
                    if (ccResObj != null)
                    {
                        if (ccResObj.Invoice != null && ccResObj.Invoice.Id != null && ccResObj.Invoice.Id != "")
                        {
                            string responseText = "";
                            string uri = string.Format("https://{0}/v3/company/{1}/invoice?operation=delete", qboBaseUrl, Session["realmId"]);
                            //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/invoices/1/bank-accounts/";
                            string body = "";

                            InvoiceDeleteReq ccObj = new InvoiceDeleteReq();
                            ccObj.Id = ccResObj.Invoice.Id;
                            ccObj.SyncToken = ccResObj.Invoice.SyncToken;

                            //Other Fields

                            body = JsonConvert.SerializeObject(ccObj);
                            // send the request
                            responseText = POSTQBAPICall(uri, body);
                            if (responseText != null && responseText != "")
                            {
                                lblMessage.Text = "Invoice made inactive successfully";
                            }
                        }
                    }
                    else
                    {
                        lblMessage.Text = firstresponseText;
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        protected void btnGetInvoices_Click(object sender, EventArgs e)
        {
            try
            {
                List<InvoicesSummary> invoiceList = new List<InvoicesSummary>();
                invoiceList = GetInvoices();
                if (invoiceList != null && invoiceList.Count > 0)
                {
                    gvwInvoices.DataSource = invoiceList;
                    gvwInvoices.DataBind();
                }
                return;
            }
            catch (Exception ex)
            {
            }
        }

        public List<InvoicesSummary> GetInvoices()
        {
            string responseText = "";
            List<InvoicesSummary> invoiceList = new List<InvoicesSummary>();
            try
            {
                string query = "Select * from Invoice startposition 1 maxresults 5";
                // build the  request
                string encodedQuery = WebUtility.UrlEncode(query);

                //add qbobase url and query
                string uri = string.Format("https://{0}/v3/company/{1}/query?query={2}", qboBaseUrl, Session["realmId"], encodedQuery);
                //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/invoices/1/bank-accounts/";
                // send the request
                responseText = GETQBAPICall(uri);
                if (responseText != null && responseText != "")
                {
                    ReadAllInvoiceRes custObj = JsonConvert.DeserializeObject<ReadAllInvoiceRes>(responseText);

                    foreach (var item in custObj.QueryResponse.Invoice)
                    {
                        InvoicesSummary invoiceObj = new InvoicesSummary();
                        invoiceObj.id = item.Id;
                        invoiceObj.txndate = item.TxnDate;
                        invoiceList.Add(invoiceObj);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return invoiceList;
        }

        #endregion
        #region Purchase

        protected void ddlPurchaseOptions_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblMessage.Text = "";
            divCreatePurchase.Visible = false;
            divPurchaseReadById.Visible = false;
            divUpdatePurchase.Visible = false;
            divPurchaseDelete.Visible = false;
            divPurchases.Visible = false;

            if (ddlPurchaseOptions.SelectedValue == "CreatePurchase")
            {
                divCreatePurchase.Visible = true;
            }
            if (ddlPurchaseOptions.SelectedValue == "GetPurchaseById")
            {
                divPurchaseReadById.Visible = true;
            }
            if (ddlPurchaseOptions.SelectedValue == "PurchaseUpdate")
            {
                divUpdatePurchase.Visible = true;
            }
            if (ddlPurchaseOptions.SelectedValue == "PurchaseDelete")
            {
                divPurchaseDelete.Visible = true;
            }
            if (ddlPurchaseOptions.SelectedValue == "ReadAllPurchases")
            {
                divPurchases.Visible = true;
            }
        }

        protected void btnCreatePurchase_Click(object sender, EventArgs e)
        {
            try
            {
                string responseText = "";
                string uri = string.Format("https://{0}/v3/company/{1}/purchase", qboBaseUrl, Session["realmId"]);
                //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/purchases/1/bank-accounts/";
                string body = "";
                CreatePurchaseReq ccObj = new CreatePurchaseReq();
                ccObj.Line = new List<CreatePurchaseReqLine>();
                CreatePurchaseReqLine lineObj = new CreatePurchaseReqLine();

                body = JsonConvert.SerializeObject(ccObj);
                // send the request
                responseText = POSTQBAPICall(uri, body);
                if (responseText != null && responseText != "")
                {
                    //CreatePurchaseRes ccResObj = new CreatePurchaseRes();
                    //ccResObj = JsonConvert.DeserializeObject<CreatePurchaseRes>(responseText);
                    //if (ccResObj != null)
                    //{
                    //    if (ccResObj.Purchase != null && ccResObj.Purchase.Id != null && ccResObj.Purchase.Id != "")
                    //    {
                    //        //Purchase Created Successfully
                    //        divCreatePurchase.Visible = false;

                    //        lblMessage.Text = "Purchase Created Successfully";
                    //    }
                    //}
                    //else
                    //{
                    //    lblMessage.Text = responseText;
                    //}
                }
            }
            catch (Exception ex)
            {
            }
        }

        protected void btnGetPurchaseById_Click(object sender, EventArgs e)
        {
            try
            {
                string PurchaseId = txtPurchaseId.Text;
                string responseText = "";
                string uri = string.Format("https://{0}/v3/company/{1}/purchase/{2}", qboBaseUrl, Session["realmId"], PurchaseId);
                //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/purchases/1/bank-accounts/";

                // send the request
                responseText = GETQBAPICall(uri);
                if (responseText != null && responseText != "")
                {
                    PurchaseByIdRes ccResObj = new PurchaseByIdRes();
                    ccResObj = JsonConvert.DeserializeObject<PurchaseByIdRes>(responseText);
                    if (ccResObj != null)
                    {
                        if (ccResObj.Purchase != null && ccResObj.Purchase.Id != null && ccResObj.Purchase.Id != "")
                        {
                            lblMessage.Text = "Customer  Name For Purchase: " + ccResObj.Purchase.CurrencyRef.name;
                        }
                    }
                    else
                    {
                        lblMessage.Text = responseText;
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        protected void btnUpdatePurchase_Click(object sender, EventArgs e)
        {
            try
            {
                string PurchaseId = txtUCPurchaseId.Text;
                string firstresponseText = "";
                string firsturi = string.Format("https://{0}/v3/company/{1}/purchase/{2}", qboBaseUrl, Session["realmId"], PurchaseId);
                //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/purchases/1/bank-accounts/";

                // send the request
                firstresponseText = GETQBAPICall(firsturi);
                if (firstresponseText != null && firstresponseText != "")
                {
                    PurchaseByIdRes ccResObj = new PurchaseByIdRes();
                    ccResObj = JsonConvert.DeserializeObject<PurchaseByIdRes>(firstresponseText);
                    if (ccResObj != null)
                    {
                        if (ccResObj.Purchase != null && ccResObj.Purchase.Id != null && ccResObj.Purchase.Id != "")
                        {
                            string responseText = "";
                            string uri = string.Format("https://{0}/v3/company/{1}/purchase", qboBaseUrl, Session["realmId"]);
                            //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/purchases/1/bank-accounts/";
                            string body = "";
                            UpdatePurchaseReq ccObj = new UpdatePurchaseReq();

                            //Pass Other Parameters if needed.

                            body = JsonConvert.SerializeObject(ccObj);
                            // send the request
                            responseText = POSTQBAPICall(uri, body);
                            if (responseText != null && responseText != "")
                            {
                                lblMessage.Text = "Purchase Updated Successfully.";
                            }
                        }
                    }
                    else
                    {
                        lblMessage.Text = firstresponseText;
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        protected void btnPurchaseDelete_Click(object sender, EventArgs e)
        {
            try
            {
                string PurchaseId = txtVDPurchaseId.Text;
                string firstresponseText = "";
                string firsturi = string.Format("https://{0}/v3/company/{1}/purchase/{2}", qboBaseUrl, Session["realmId"], PurchaseId);
                //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/purchases/1/bank-accounts/";

                // send the request
                firstresponseText = GETQBAPICall(firsturi);
                if (firstresponseText != null && firstresponseText != "")
                {
                    PurchaseByIdRes ccResObj = new PurchaseByIdRes();
                    ccResObj = JsonConvert.DeserializeObject<PurchaseByIdRes>(firstresponseText);
                    if (ccResObj != null)
                    {
                        if (ccResObj.Purchase != null && ccResObj.Purchase.Id != null && ccResObj.Purchase.Id != "")
                        {
                            string responseText = "";
                            string uri = string.Format("https://{0}/v3/company/{1}/purchase?operation=delete", qboBaseUrl, Session["realmId"]);
                            //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/purchases/1/bank-accounts/";
                            string body = "";

                            PurchaseDeleteReq ccObj = new PurchaseDeleteReq();
                            ccObj.Id = ccResObj.Purchase.Id;
                            ccObj.SyncToken = ccResObj.Purchase.SyncToken;

                            //Other Fields

                            body = JsonConvert.SerializeObject(ccObj);
                            // send the request
                            responseText = POSTQBAPICall(uri, body);
                            if (responseText != null && responseText != "")
                            {
                                lblMessage.Text = "Purchase made inactive successfully";
                            }
                        }
                    }
                    else
                    {
                        lblMessage.Text = firstresponseText;
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        protected void btnGetPurchases_Click(object sender, EventArgs e)
        {
            try
            {
                List<PurchasesSummary> purchaseList = new List<PurchasesSummary>();
                purchaseList = GetPurchases();
                if (purchaseList != null && purchaseList.Count > 0)
                {
                    gvwPurchases.DataSource = purchaseList;
                    gvwPurchases.DataBind();
                }
                return;
            }
            catch (Exception ex)
            {
            }
        }

        public List<PurchasesSummary> GetPurchases()
        {
            string responseText = "";
            List<PurchasesSummary> purchaseList = new List<PurchasesSummary>();
            try
            {
                string query = "Select * from Purchase startposition 1 maxresults 5";
                // build the  request
                string encodedQuery = WebUtility.UrlEncode(query);

                //add qbobase url and query
                string uri = string.Format("https://{0}/v3/company/{1}/query?query={2}", qboBaseUrl, Session["realmId"], encodedQuery);
                //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/purchases/1/bank-accounts/";
                // send the request
                responseText = GETQBAPICall(uri);
                if (responseText != null && responseText != "")
                {
                    ReadAllPurchaseRes custObj = JsonConvert.DeserializeObject<ReadAllPurchaseRes>(responseText);

                    foreach (var item in custObj.QueryResponse.Purchase)
                    {
                        PurchasesSummary purchaseObj = new PurchasesSummary();
                        purchaseObj.id = item.Id;
                        purchaseObj.txndate = item.TxnDate;
                        purchaseList.Add(purchaseObj);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return purchaseList;
        }

        #endregion
        #region SalesReceipt

        protected void ddlSalesReceiptOptions_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblMessage.Text = "";
            divCreateSalesReceipt.Visible = false;
            divSalesReceiptReadById.Visible = false;
            divUpdateSalesReceipt.Visible = false;
            divSalesReceiptDelete.Visible = false;
            divSalesReceipts.Visible = false;

            if (ddlSalesReceiptOptions.SelectedValue == "CreateSalesReceipt")
            {
                divCreateSalesReceipt.Visible = true;
            }
            if (ddlSalesReceiptOptions.SelectedValue == "GetSalesReceiptById")
            {
                divSalesReceiptReadById.Visible = true;
            }
            if (ddlSalesReceiptOptions.SelectedValue == "SalesReceiptUpdate")
            {
                divUpdateSalesReceipt.Visible = true;
            }
            if (ddlSalesReceiptOptions.SelectedValue == "SalesReceiptDelete")
            {
                divSalesReceiptDelete.Visible = true;
            }
            if (ddlSalesReceiptOptions.SelectedValue == "ReadAllSalesReceipts")
            {
                divSalesReceipts.Visible = true;
            }
        }

        protected void btnCreateSalesReceipt_Click(object sender, EventArgs e)
        {
            try
            {
                string responseText = "";
                string uri = string.Format("https://{0}/v3/company/{1}/salesReceipt", qboBaseUrl, Session["realmId"]);
                //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/salesReceipts/1/bank-accounts/";
                string body = "";
                CreateSalesReceiptReq ccObj = new CreateSalesReceiptReq();
                ccObj.Line = new List<CreateSalesReceiptReqLine>();
                CreateSalesReceiptReqLine lineObj = new CreateSalesReceiptReqLine();

                body = JsonConvert.SerializeObject(ccObj);
                // send the request
                responseText = POSTQBAPICall(uri, body);
                if (responseText != null && responseText != "")
                {
                    //CreateSalesReceiptRes ccResObj = new CreateSalesReceiptRes();
                    //ccResObj = JsonConvert.DeserializeObject<CreateSalesReceiptRes>(responseText);
                    //if (ccResObj != null)
                    //{
                    //    if (ccResObj.SalesReceipt != null && ccResObj.SalesReceipt.Id != null && ccResObj.SalesReceipt.Id != "")
                    //    {
                    //        //SalesReceipt Created Successfully
                    //        divCreateSalesReceipt.Visible = false;

                    //        lblMessage.Text = "SalesReceipt Created Successfully";
                    //    }
                    //}
                    //else
                    //{
                    //    lblMessage.Text = responseText;
                    //}
                }
            }
            catch (Exception ex)
            {
            }
        }

        protected void btnGetSalesReceiptById_Click(object sender, EventArgs e)
        {
            try
            {
                string SalesReceiptId = txtSalesReceiptId.Text;
                string responseText = "";
                string uri = string.Format("https://{0}/v3/company/{1}/salesReceipt/{2}", qboBaseUrl, Session["realmId"], SalesReceiptId);
                //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/salesReceipts/1/bank-accounts/";

                // send the request
                responseText = GETQBAPICall(uri);
                if (responseText != null && responseText != "")
                {
                    SalesReceiptByIdRes ccResObj = new SalesReceiptByIdRes();
                    ccResObj = JsonConvert.DeserializeObject<SalesReceiptByIdRes>(responseText);
                    if (ccResObj != null)
                    {
                        if (ccResObj.SalesReceipt != null && ccResObj.SalesReceipt.Id != null && ccResObj.SalesReceipt.Id != "")
                        {
                            lblMessage.Text = "Customer  Name For SalesReceipt: " + ccResObj.SalesReceipt.CustomerRef.name;
                        }
                    }
                    else
                    {
                        lblMessage.Text = responseText;
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        protected void btnUpdateSalesReceipt_Click(object sender, EventArgs e)
        {
            try
            {
                string SalesReceiptId = txtUCSalesReceiptId.Text;
                string firstresponseText = "";
                string firsturi = string.Format("https://{0}/v3/company/{1}/salesReceipt/{2}", qboBaseUrl, Session["realmId"], SalesReceiptId);
                //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/salesReceipts/1/bank-accounts/";

                // send the request
                firstresponseText = GETQBAPICall(firsturi);
                if (firstresponseText != null && firstresponseText != "")
                {
                    SalesReceiptByIdRes ccResObj = new SalesReceiptByIdRes();
                    ccResObj = JsonConvert.DeserializeObject<SalesReceiptByIdRes>(firstresponseText);
                    if (ccResObj != null)
                    {
                        if (ccResObj.SalesReceipt != null && ccResObj.SalesReceipt.Id != null && ccResObj.SalesReceipt.Id != "")
                        {
                            string responseText = "";
                            string uri = string.Format("https://{0}/v3/company/{1}/salesReceipt", qboBaseUrl, Session["realmId"]);
                            //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/salesReceipts/1/bank-accounts/";
                            string body = "";
                            UpdateSalesReceiptReq ccObj = new UpdateSalesReceiptReq();

                            //Pass Other Parameters if needed.

                            body = JsonConvert.SerializeObject(ccObj);
                            // send the request
                            responseText = POSTQBAPICall(uri, body);
                            if (responseText != null && responseText != "")
                            {
                                lblMessage.Text = "SalesReceipt Updated Successfully.";
                            }
                        }
                    }
                    else
                    {
                        lblMessage.Text = firstresponseText;
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        protected void btnSalesReceiptDelete_Click(object sender, EventArgs e)
        {
            try
            {
                string SalesReceiptId = txtVDSalesReceiptId.Text;
                string firstresponseText = "";
                string firsturi = string.Format("https://{0}/v3/company/{1}/salesReceipt/{2}", qboBaseUrl, Session["realmId"], SalesReceiptId);
                //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/salesReceipts/1/bank-accounts/";

                // send the request
                firstresponseText = GETQBAPICall(firsturi);
                if (firstresponseText != null && firstresponseText != "")
                {
                    SalesReceiptByIdRes ccResObj = new SalesReceiptByIdRes();
                    ccResObj = JsonConvert.DeserializeObject<SalesReceiptByIdRes>(firstresponseText);
                    if (ccResObj != null)
                    {
                        if (ccResObj.SalesReceipt != null && ccResObj.SalesReceipt.Id != null && ccResObj.SalesReceipt.Id != "")
                        {
                            string responseText = "";
                            string uri = string.Format("https://{0}/v3/company/{1}/salesReceipt?operation=delete", qboBaseUrl, Session["realmId"]);
                            //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/salesReceipts/1/bank-accounts/";
                            string body = "";

                            SalesReceiptDeleteReq ccObj = new SalesReceiptDeleteReq();
                            ccObj.Id = ccResObj.SalesReceipt.Id;
                            ccObj.SyncToken = ccResObj.SalesReceipt.SyncToken;

                            //Other Fields

                            body = JsonConvert.SerializeObject(ccObj);
                            // send the request
                            responseText = POSTQBAPICall(uri, body);
                            if (responseText != null && responseText != "")
                            {
                                lblMessage.Text = "SalesReceipt made inactive successfully";
                            }
                        }
                    }
                    else
                    {
                        lblMessage.Text = firstresponseText;
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        protected void btnGetSalesReceipts_Click(object sender, EventArgs e)
        {
            try
            {
                List<SalesReceiptsSummary> salesReceiptList = new List<SalesReceiptsSummary>();
                salesReceiptList = GetSalesReceipts();
                if (salesReceiptList != null && salesReceiptList.Count > 0)
                {
                    gvwSalesReceipts.DataSource = salesReceiptList;
                    gvwSalesReceipts.DataBind();
                }
                return;
            }
            catch (Exception ex)
            {
            }
        }

        public List<SalesReceiptsSummary> GetSalesReceipts()
        {
            string responseText = "";
            List<SalesReceiptsSummary> salesReceiptList = new List<SalesReceiptsSummary>();
            try
            {
                string query = "Select * from SalesReceipt startposition 1 maxresults 5";
                // build the  request
                string encodedQuery = WebUtility.UrlEncode(query);

                //add qbobase url and query
                string uri = string.Format("https://{0}/v3/company/{1}/query?query={2}", qboBaseUrl, Session["realmId"], encodedQuery);
                //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/salesReceipts/1/bank-accounts/";
                // send the request
                responseText = GETQBAPICall(uri);
                if (responseText != null && responseText != "")
                {
                    ReadAllSalesReceiptRes custObj = JsonConvert.DeserializeObject<ReadAllSalesReceiptRes>(responseText);

                    foreach (var item in custObj.QueryResponse.SalesReceipt)
                    {
                        SalesReceiptsSummary salesReceiptObj = new SalesReceiptsSummary();
                        salesReceiptObj.id = item.Id;
                        salesReceiptObj.txndate = item.TxnDate;
                        salesReceiptList.Add(salesReceiptObj);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return salesReceiptList;
        }

        #endregion
    }

    public static class ResponseHelper
    {
        public static void Redirect(this HttpResponse response, string url, string target, string windowFeatures)
        {
            if ((String.IsNullOrEmpty(target) || target.Equals("_self", StringComparison.OrdinalIgnoreCase)) && String.IsNullOrEmpty(windowFeatures))
            {
                response.Redirect(url);
            }
            else
            {
                Page page = (Page)HttpContext.Current.Handler;

                if (page == null)
                {
                    throw new InvalidOperationException("Cannot redirect to new window outside Page context.");
                }
                url = page.ResolveClientUrl(url);

                string script;
                if (!String.IsNullOrEmpty(windowFeatures))
                {
                    script = @"window.open(""{0}"", ""{1}"", ""{2}"");";
                }
                else
                {
                    script = @"window.open(""{0}"", ""{1}"");";
                }
                script = String.Format(script, url, target, windowFeatures);
                ScriptManager.RegisterStartupScript(page, typeof(Page), "Redirect", script, true);
            }
        }
    }
}