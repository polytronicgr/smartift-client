using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;

namespace Lts.Sift.WinClient
{
#pragma warning disable // Disable all warnings

    [GeneratedCode("NSwag", "3.30.6051.40425")]
    public partial class AuthenticationClient
    {
        public AuthenticationClient() : this(ApiClient.BaseUrl + "auth/1") { }

        public AuthenticationClient(string baseUrl)
        {
            BaseUrl = baseUrl;
        }

        partial void PrepareRequest(HttpClient request, ref string url);

        partial void ProcessResponse(HttpClient request, HttpResponseMessage response);

        public string BaseUrl { get; set; }

        /// <summary>Authenticates the user based on credentials and returns them a suitable JWT token.</summary>
        /// <param name="request">The authentication request including user and password details.</param>
        /// <returns>The authentication was a success and user information will be returned along with a new token.</returns>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public Task<AuthenticateUserResponse> AuthenticateJwtAsync(AuthenticateUserRequest request)
        {
            return AuthenticateJwtAsync(request, CancellationToken.None);
        }

        /// <summary>Authenticates the user based on credentials and returns them a suitable JWT token.</summary>
        /// <param name="request">The authentication request including user and password details.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The authentication was a success and user information will be returned along with a new token.</returns>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public async Task<AuthenticateUserResponse> AuthenticateJwtAsync(AuthenticateUserRequest request, CancellationToken cancellationToken)
        {
            var url_ = string.Format("{0}/{1}?", BaseUrl, "");


            var client_ = new HttpClient();
            PrepareRequest(client_, ref url_);

            var content_ = new StringContent(JsonConvert.SerializeObject(request));
            content_.Headers.ContentType.MediaType = "application/json";

            var response_ = await client_.PutAsync(url_, content_, cancellationToken).ConfigureAwait(false);
            ProcessResponse(client_, response_);

            var responseData_ = await response_.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            var status_ = ((int)response_.StatusCode).ToString();

            if (status_ == "200")
            {
                var result_ = default(AuthenticateUserResponse);
                try
                {
                    if (responseData_.Length > 0)
                        result_ = JsonConvert.DeserializeObject<AuthenticateUserResponse>(Encoding.UTF8.GetString(responseData_));
                    return result_;
                }
                catch (Exception exception)
                {
                    throw new SwaggerException("Could not deserialize the response body.", status_, responseData_, exception);
                }
            }
            else
            if (status_ == "401")
            {
                return default(AuthenticateUserResponse);

            }
            else
            {
            }

            throw new SwaggerException("The HTTP status code of the response was not expected (" + (int)response_.StatusCode + ").", status_, responseData_, null);
        }

        /// <summary>Renews the token for the current user.</summary>
        /// <param name="authorization">The JWT token to use for authorization (must include Bearer prefix)</param>
        /// <returns>The authentication was a success and user information will be returned along with a new token.</returns>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public Task<AuthenticateUserResponse> RenewJwtAsync(string authorization)
        {
            return RenewJwtAsync(authorization, CancellationToken.None);
        }

        /// <summary>Renews the token for the current user.</summary>
        /// <param name="authorization">The JWT token to use for authorization (must include Bearer prefix)</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The authentication was a success and user information will be returned along with a new token.</returns>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public async Task<AuthenticateUserResponse> RenewJwtAsync(string authorization, CancellationToken cancellationToken)
        {
            var url_ = string.Format("{0}/{1}?", BaseUrl, "renew");


            var client_ = new HttpClient();
            PrepareRequest(client_, ref url_);
            client_.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authorization);

            var response_ = await client_.GetAsync(url_, cancellationToken).ConfigureAwait(false);
            ProcessResponse(client_, response_);

            var responseData_ = await response_.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            var status_ = ((int)response_.StatusCode).ToString();

            if (status_ == "200")
            {
                var result_ = default(AuthenticateUserResponse);
                try
                {
                    if (responseData_.Length > 0)
                        result_ = JsonConvert.DeserializeObject<AuthenticateUserResponse>(Encoding.UTF8.GetString(responseData_));
                    return result_;
                }
                catch (Exception exception)
                {
                    throw new SwaggerException("Could not deserialize the response body.", status_, responseData_, exception);
                }
            }
            else
            if (status_ == "401")
            {
                return default(AuthenticateUserResponse);

            }
            else
            {
            }

            throw new SwaggerException("The HTTP status code of the response was not expected (" + (int)response_.StatusCode + ").", status_, responseData_, null);
        }

    }

    [GeneratedCode("NSwag", "3.30.6051.40425")]
    public partial class UserClient
    {
        public UserClient() : this(ApiClient.BaseUrl + "auth/1") { }

        public UserClient(string baseUrl)
        {
            BaseUrl = baseUrl;
        }

        partial void PrepareRequest(HttpClient request, ref string url);

        partial void ProcessResponse(HttpClient request, HttpResponseMessage response);

        public string BaseUrl { get; set; }

        /// <summary>Gets all users known to the system.</summary>
        /// <param name="authorization">The JWT token to use for authorization (must include Bearer prefix)</param>
        /// <returns>A list of all users contained within the LTS backend and all their associated information.</returns>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public Task<ObservableCollection<User>> UserGetAsync(string authorization)
        {
            return UserGetAsync(authorization, CancellationToken.None);
        }

        /// <summary>Gets all users known to the system.</summary>
        /// <param name="authorization">The JWT token to use for authorization (must include Bearer prefix)</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A list of all users contained within the LTS backend and all their associated information.</returns>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public async Task<ObservableCollection<User>> UserGetAsync(string authorization, CancellationToken cancellationToken)
        {
            var url_ = string.Format("{0}/{1}?", BaseUrl, "users");


            var client_ = new HttpClient();
            PrepareRequest(client_, ref url_);
            client_.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authorization);

            var response_ = await client_.GetAsync(url_, cancellationToken).ConfigureAwait(false);
            ProcessResponse(client_, response_);

            var responseData_ = await response_.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            var status_ = ((int)response_.StatusCode).ToString();

            if (status_ == "200")
            {
                var result_ = default(ObservableCollection<User>);
                try
                {
                    if (responseData_.Length > 0)
                        result_ = JsonConvert.DeserializeObject<ObservableCollection<User>>(Encoding.UTF8.GetString(responseData_));
                    return result_;
                }
                catch (Exception exception)
                {
                    throw new SwaggerException("Could not deserialize the response body.", status_, responseData_, exception);
                }
            }
            else
            if (status_ == "401")
            {
                return default(ObservableCollection<User>);

            }
            else
            {
            }

            throw new SwaggerException("The HTTP status code of the response was not expected (" + (int)response_.StatusCode + ").", status_, responseData_, null);
        }

        /// <summary>Adds a new user to the system.</summary>
        /// <param name="user">The details of the user to add to the system.</param>
        /// <param name="authorization">The JWT token to use for authorization (must include Bearer prefix)</param>
        /// <returns>OK</returns>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public Task<Dictionary<string, object>> UserAddAsync(UserAddRequest user, string authorization)
        {
            return UserAddAsync(user, authorization, CancellationToken.None);
        }

        /// <summary>Adds a new user to the system.</summary>
        /// <param name="user">The details of the user to add to the system.</param>
        /// <param name="authorization">The JWT token to use for authorization (must include Bearer prefix)</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>OK</returns>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public async Task<Dictionary<string, object>> UserAddAsync(UserAddRequest user, string authorization, CancellationToken cancellationToken)
        {
            var url_ = string.Format("{0}/{1}?", BaseUrl, "users");


            var client_ = new HttpClient();
            PrepareRequest(client_, ref url_);
            client_.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authorization);

            var content_ = new StringContent(JsonConvert.SerializeObject(user));
            content_.Headers.ContentType.MediaType = "application/json";

            var response_ = await client_.PostAsync(url_, content_, cancellationToken).ConfigureAwait(false);
            ProcessResponse(client_, response_);

            var responseData_ = await response_.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            var status_ = ((int)response_.StatusCode).ToString();

            if (status_ == "200")
            {
                var result_ = default(Dictionary<string, object>);
                try
                {
                    if (responseData_.Length > 0)
                        result_ = JsonConvert.DeserializeObject<Dictionary<string, object>>(Encoding.UTF8.GetString(responseData_));
                    return result_;
                }
                catch (Exception exception)
                {
                    throw new SwaggerException("Could not deserialize the response body.", status_, responseData_, exception);
                }
            }
            else
            if (status_ == "201")
            {
                return default(Dictionary<string, object>);

            }
            else
            if (status_ == "400")
            {
                return default(Dictionary<string, object>);

            }
            else
            if (status_ == "401")
            {
                return default(Dictionary<string, object>);

            }
            else
            {
            }

            throw new SwaggerException("The HTTP status code of the response was not expected (" + (int)response_.StatusCode + ").", status_, responseData_, null);
        }

    }

    /// <summary>This request allows a calling client to request an authenticated token allowing access to methods that require authentication or authorisation.</summary>
    [JsonObject(MemberSerialization.OptIn)]
    [GeneratedCode("NJsonSchema", "2.62.6049.40362")]
    public partial class AuthenticateUserRequest : INotifyPropertyChanged
    {
        private string _username;
        private string _password;

        /// <summary>Gets or sets the user to authenticate with.</summary>
        [JsonProperty("Username", Required = Required.Always)]
        [Required]
        public string Username
        {
            get { return _username; }
            set
            {
                if (_username != value)
                {
                    _username = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>Gets or sets the password to authenticate with.</summary>
        [JsonProperty("Password", Required = Required.Always)]
        [Required]
        public string Password
        {
            get { return _password; }
            set
            {
                if (_password != value)
                {
                    _password = value;
                    RaisePropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static AuthenticateUserRequest FromJson(string data)
        {
            return JsonConvert.DeserializeObject<AuthenticateUserRequest>(data);
        }

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>This response is sent to a client upon a successful authentication.  Invalid authentication will return a non-200 response instead.</summary>
    [JsonObject(MemberSerialization.OptIn)]
    [GeneratedCode("NJsonSchema", "2.62.6049.40362")]
    public partial class AuthenticateUserResponse : INotifyPropertyChanged
    {
        private string _jsonWebToken;
        private User _user;

        /// <summary>Gets the JWT being returned to the client.</summary>
        [JsonProperty("JsonWebToken", Required = Required.Default)]
        public string JsonWebToken
        {
            get { return _jsonWebToken; }
            set
            {
                if (_jsonWebToken != value)
                {
                    _jsonWebToken = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>Gets the user that has logged in.</summary>
        [JsonProperty("User", Required = Required.Default)]
        public User User
        {
            get { return _user; }
            set
            {
                if (_user != value)
                {
                    _user = value;
                    RaisePropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static AuthenticateUserResponse FromJson(string data)
        {
            return JsonConvert.DeserializeObject<AuthenticateUserResponse>(data);
        }

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>This class represents a single user in the LTS authentication system.</summary>
    [JsonObject(MemberSerialization.OptIn)]
    [GeneratedCode("NJsonSchema", "2.62.6049.40362")]
    public partial class User : INotifyPropertyChanged
    {
        private string _userId;
        private string _username;
        private DateTime? _createTime;
        private DateTime? _updateTime;
        private ObservableCollection<string> _roles;
        private Dictionary<string, string> _additionalDetails;

        /// <summary>Gets the ID of this user.</summary>
        [JsonProperty("UserId", Required = Required.Default)]
        public string UserId
        {
            get { return _userId; }
            set
            {
                if (_userId != value)
                {
                    _userId = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>Gets the username of this user.</summary>
        [JsonProperty("Username", Required = Required.Default)]
        public string Username
        {
            get { return _username; }
            set
            {
                if (_username != value)
                {
                    _username = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>Gets the time the user was created (UTC).</summary>
        [JsonProperty("CreateTime", Required = Required.Default)]
        public DateTime? CreateTime
        {
            get { return _createTime; }
            set
            {
                if (_createTime != value)
                {
                    _createTime = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>Gets the time the user's record was last updated (UTC).</summary>
        [JsonProperty("UpdateTime", Required = Required.Default)]
        public DateTime? UpdateTime
        {
            get { return _updateTime; }
            set
            {
                if (_updateTime != value)
                {
                    _updateTime = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>Gets the roles assigned to the user.</summary>
        [JsonProperty("Roles", Required = Required.Default)]
        public ObservableCollection<string> Roles
        {
            get { return _roles; }
            set
            {
                if (_roles != value)
                {
                    _roles = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>Gets any additional details related to the user keyed by the URN of a setting and storing a value.</summary>
        [JsonProperty("AdditionalDetails", Required = Required.Default)]
        public Dictionary<string, string> AdditionalDetails
        {
            get { return _additionalDetails; }
            set
            {
                if (_additionalDetails != value)
                {
                    _additionalDetails = value;
                    RaisePropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static User FromJson(string data)
        {
            return JsonConvert.DeserializeObject<User>(data);
        }

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>This class represents a request to add a new user to the backend system.</summary>
    [JsonObject(MemberSerialization.OptIn)]
    [GeneratedCode("NJsonSchema", "2.62.6049.40362")]
    public partial class UserAddRequest : INotifyPropertyChanged
    {
        private string _username;
        private string _password;
        private ObservableCollection<string> _roles;
        private Dictionary<string, string> _additionalDetails;

        /// <summary>Gets the username of this user.</summary>
        [JsonProperty("Username", Required = Required.Always)]
        [Required]
        public string Username
        {
            get { return _username; }
            set
            {
                if (_username != value)
                {
                    _username = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>Gets the password of this user.</summary>
        [JsonProperty("Password", Required = Required.Always)]
        [Required]
        public string Password
        {
            get { return _password; }
            set
            {
                if (_password != value)
                {
                    _password = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>Gets the roles assigned to the user.</summary>
        [JsonProperty("Roles", Required = Required.Default)]
        public ObservableCollection<string> Roles
        {
            get { return _roles; }
            set
            {
                if (_roles != value)
                {
                    _roles = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>Gets any additional details related to the user keyed by the URN of a setting and storing a value.</summary>
        [JsonProperty("AdditionalDetails", Required = Required.Default)]
        public Dictionary<string, string> AdditionalDetails
        {
            get { return _additionalDetails; }
            set
            {
                if (_additionalDetails != value)
                {
                    _additionalDetails = value;
                    RaisePropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static UserAddRequest FromJson(string data)
        {
            return JsonConvert.DeserializeObject<UserAddRequest>(data);
        }

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    [GeneratedCode("NSwag", "3.30.6051.40425")]
    public class SwaggerException : Exception
    {
        public string StatusCode { get; private set; }

        public byte[] ResponseData { get; private set; }

        public SwaggerException(string message, string statusCode, byte[] responseData, Exception innerException)
            : base(message, innerException)
        {
            StatusCode = statusCode;
            ResponseData = responseData;
        }

        public override string ToString()
        {
            return string.Format("HTTP Response: n{0}n{1}", Encoding.UTF8.GetString(ResponseData), base.ToString());
        }
    }

    [GeneratedCode("NSwag", "3.30.6051.40425")]
    public class SwaggerException<TResponse> : SwaggerException
    {
        public TResponse Response { get; private set; }

        public SwaggerException(string message, string statusCode, byte[] responseData, TResponse response, Exception innerException)
            : base(message, statusCode, responseData, innerException)
        {
            Response = response;
        }
    }

}