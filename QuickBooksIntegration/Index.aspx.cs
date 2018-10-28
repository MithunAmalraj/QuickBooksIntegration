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

        protected void ImgRevoke_Click(object sender, ImageClickEventArgs e)
        {
            if (Session["accessToken"] != null && Session["refreshToken"] != null)
            {
                //revoke tokens
                performRevokeToken(Session["accessToken"].ToString(), Session["refreshToken"].ToString());
            }
        }

        protected void ImgQBOAPICall_Click(object sender, ImageClickEventArgs e)
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

        #endregion

        #region ApplicationMethods
        protected void btnGetPayments_Click(object sender, EventArgs e)
        {
            try
            {
                List<CustomerPaymentListSummary> paymentList = new List<CustomerPaymentListSummary>();
                paymentList = GetCustomerPayments();
                if (paymentList != null && paymentList.Count > 0)
                {
                    gvwCustomerPayments.DataSource = paymentList;
                    gvwCustomerPayments.DataBind();
                }
                return;


                // send the UserInfo endpoint request
                HttpWebRequest userinfoRequest = (HttpWebRequest)WebRequest.Create("https://sandbox-quickbooks.api.intuit.com/v3/company/123146130535274/query?minorversion=4");
                userinfoRequest.Method = "GET";
                userinfoRequest.Headers.Add(string.Format("Authorization: Bearer {0}", Session["accessToken"]));
                userinfoRequest.Accept = "application/json";

                // get the response
                HttpWebResponse userinfoResponse = (HttpWebResponse)userinfoRequest.GetResponse();

                using (var userinfoReader = new StreamReader(userinfoResponse.GetResponseStream()))
                {
                    //read response
                    string responseText = userinfoReader.ReadToEnd();

                    //If the user already exists in your database, initiate an application session for that user.
                    //If the user does not exist in your user database, redirect the user to your new- user, sign - up flow.
                    //You may be able to auto - register the user based on the information you receive from intuit.
                    //Or at the very least you may be able to pre - populate many of the fields that you require on your registration form.

                    //Decode userInfo response
                }
                output("Get User Info Call completed.");
            }
            catch (Exception ex)
            {
            }
        }

        public List<CustomerPaymentListSummary> GetCustomerPayments()
        {
            string responseText = "";
            List<CustomerPaymentListSummary> paymentList = new List<CustomerPaymentListSummary>();
            try
            {
                string query = "select * from payment startposition 1 maxresults 60";
                // build the  request
                string encodedQuery = WebUtility.UrlEncode(query);

                //add qbobase url and query
                string uri = string.Format("https://{0}/v3/company/{1}/query?query={2}", qboBaseUrl, Session["realmId"], encodedQuery);
                //string uri = "https://sandbox.api.intuit.com/quickbooks/v4/customers/1/bank-accounts/";
                // send the request
                responseText = GETQBAPICall(uri);
                if (responseText != null && responseText != "")
                {
                    CustomerPaymentListPaymentsRootObject PaymentObj = JsonConvert.DeserializeObject<CustomerPaymentListPaymentsRootObject>(responseText);

                    foreach (var item in PaymentObj.QueryResponse.Payment)
                    {
                        CustomerPaymentListSummary paymentObj = new CustomerPaymentListSummary();
                        paymentObj.Id = item.Id;
                        paymentObj.CustomerName = item.CustomerRef.name;
                        paymentObj.Amount = item.TotalAmt.ToString();
                        paymentObj.TransactionDate = item.TxnDate;
                        paymentList.Add(paymentObj);
                    }
                    gvwCustomerPayments.DataSource = paymentList;
                    gvwCustomerPayments.DataBind();
                }
            }
            catch (Exception ex)
            {

            }
            return paymentList;
        }
        #endregion

        protected void ddlModule_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblMessage.Text = "";

            divCustommerPayments.Visible = false;
            divCustomer.Visible = false;

            if (ddlModule.SelectedValue == "CustomerPayment")
            {
                divCustommerPayments.Visible = true;
            }
            else if (ddlModule.SelectedValue == "Customer")
            {
                divCustomer.Visible = true;
            }
        }
        protected void ddlCustomerOptions_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblMessage.Text = "";
            divCreateCustomer.Visible = false;
            divCustomerReadById.Visible = false;
            divUpdateCustomer.Visible = false;
            divCustomerDelete.Visible = false;

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
            catch (Exception ex)
            {

            }
            return responseText;
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
            catch(Exception ex)
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