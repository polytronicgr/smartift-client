using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
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

    [GeneratedCode("NSwag", "3.28.6043.39005")]
    public partial class ProductClient
    {
        public ProductClient() : this(ApiClient.BaseUrl + "product/1") { }

        public ProductClient(string baseUrl)
        {
            BaseUrl = baseUrl;
        }

        partial void PrepareRequest(HttpClient request, ref string url);

        partial void ProcessResponse(HttpClient request, HttpResponseMessage response);

        public string BaseUrl { get; set; }

        /// <summary>Gets all products known to the system.</summary>
        /// <param name="authorization">The JWT token to use for authorization (must include Bearer prefix)</param>
        /// <returns>A list of all products contained within the LTS backend and all their associated information.</returns>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public Task<List<Product>> ProductGetAsync(string authorization)
        {
            return ProductGetAsync(authorization, CancellationToken.None);
        }

        /// <summary>Gets all products known to the system.</summary>
        /// <param name="authorization">The JWT token to use for authorization (must include Bearer prefix)</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A list of all products contained within the LTS backend and all their associated information.</returns>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public async Task<List<Product>> ProductGetAsync(string authorization, CancellationToken cancellationToken)
        {
            var url_ = string.Format("{0}/{1}?", BaseUrl, "");


            var client_ = new HttpClient();
            PrepareRequest(client_, ref url_);
            client_.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authorization);

            var response_ = await client_.GetAsync(url_, cancellationToken).ConfigureAwait(false);
            ProcessResponse(client_, response_);

            var responseData_ = await response_.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            var status_ = ((int)response_.StatusCode).ToString();

            if (status_ == "200")
            {
                var result_ = default(List<Product>);
                try
                {
                    if (responseData_.Length > 0)
                        result_ = JsonConvert.DeserializeObject<List<Product>>(Encoding.UTF8.GetString(responseData_));
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
                return default(List<Product>);

            }
            else
            if (status_ == "404")
            {
                return default(List<Product>);

            }
            else
            {
            }

            throw new SwaggerException("The HTTP status code of the response was not expected (" + (int)response_.StatusCode + ").", status_, responseData_, null);
        }

        /// <summary>Adds or updates a product.</summary>
        /// <param name="product">The product to add or update.</param>
        /// <param name="authorization">The JWT token to use for authorization (must include Bearer prefix)</param>
        /// <returns>An existing product has been updated.</returns>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public Task<Product> ProductSetAsync(Product product, string authorization)
        {
            return ProductSetAsync(product, authorization, CancellationToken.None);
        }

        /// <summary>Adds or updates a product.</summary>
        /// <param name="product">The product to add or update.</param>
        /// <param name="authorization">The JWT token to use for authorization (must include Bearer prefix)</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>An existing product has been updated.</returns>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public async Task<Product> ProductSetAsync(Product product, string authorization, CancellationToken cancellationToken)
        {
            var url_ = string.Format("{0}/{1}?", BaseUrl, "");


            var client_ = new HttpClient();
            PrepareRequest(client_, ref url_);
            client_.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authorization);

            var content_ = new StringContent(JsonConvert.SerializeObject(product));
            content_.Headers.ContentType.MediaType = "application/json";

            var response_ = await client_.PutAsync(url_, content_, cancellationToken).ConfigureAwait(false);
            ProcessResponse(client_, response_);

            var responseData_ = await response_.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            var status_ = ((int)response_.StatusCode).ToString();

            if (status_ == "200")
            {
                var result_ = default(Product);
                try
                {
                    if (responseData_.Length > 0)
                        result_ = JsonConvert.DeserializeObject<Product>(Encoding.UTF8.GetString(responseData_));
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
                var result_ = default(Product);
                try
                {
                    if (responseData_.Length > 0)
                        result_ = JsonConvert.DeserializeObject<Product>(Encoding.UTF8.GetString(responseData_));
                    return result_;
                }
                catch (Exception exception)
                {
                    throw new SwaggerException("Could not deserialize the response body.", status_, responseData_, exception);
                }
            }
            else
            if (status_ == "400")
            {
                return default(Product);

            }
            else
            if (status_ == "401")
            {
                return default(Product);

            }
            else
            {
            }

            throw new SwaggerException("The HTTP status code of the response was not expected (" + (int)response_.StatusCode + ").", status_, responseData_, null);
        }

        /// <summary>Gets summary information about a single product that is available to non-administrative users.</summary>
        /// <param name="productId">The product to get information about.</param>
        /// <param name="authorization">The JWT token to use for authorization (must include Bearer prefix)</param>
        /// <returns>A summary of information for the requested product.</returns>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public Task<ProductSummaryResponse> ProductSummaryGetAsync(string productId, string authorization)
        {
            return ProductSummaryGetAsync(productId, authorization, CancellationToken.None);
        }

        /// <summary>Gets summary information about a single product that is available to non-administrative users.</summary>
        /// <param name="productId">The product to get information about.</param>
        /// <param name="authorization">The JWT token to use for authorization (must include Bearer prefix)</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A summary of information for the requested product.</returns>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public async Task<ProductSummaryResponse> ProductSummaryGetAsync(string productId, string authorization, CancellationToken cancellationToken)
        {
            var url_ = string.Format("{0}/{1}?", BaseUrl, "{productId}/summary");
            if (productId == null)
                throw new ArgumentNullException("productId");

            url_ = url_.Replace("{productId}", Uri.EscapeUriString(productId.ToString()));


            var client_ = new HttpClient();
            PrepareRequest(client_, ref url_);
            client_.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authorization);

            var response_ = await client_.GetAsync(url_, cancellationToken).ConfigureAwait(false);
            ProcessResponse(client_, response_);

            var responseData_ = await response_.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            var status_ = ((int)response_.StatusCode).ToString();

            if (status_ == "200")
            {
                var result_ = default(ProductSummaryResponse);
                try
                {
                    if (responseData_.Length > 0)
                        result_ = JsonConvert.DeserializeObject<ProductSummaryResponse>(Encoding.UTF8.GetString(responseData_));
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
                return default(ProductSummaryResponse);

            }
            else
            if (status_ == "404")
            {
                return default(ProductSummaryResponse);

            }
            else
            {
            }

            throw new SwaggerException("The HTTP status code of the response was not expected (" + (int)response_.StatusCode + ").", status_, responseData_, null);
        }

    }

    /// <summary>A product is any individual software offering offered by LTS - be it a re-branding solution of an existing product or a standalone piece of software.  This object
    ///             describes that product.</summary>
    [JsonObject(MemberSerialization.OptIn)]
    [GeneratedCode("NJsonSchema", "2.60.6043.38307")]
    public partial class Product : INotifyPropertyChanged
    {
        private string _productId;
        private string _name;
        private string _broadcastMessage;
        private DateTime? _broadcastValidUntil;
        private List<ProductVersion> _versions;
        private DateTime _createTime;

        /// <summary>Gets the unique identifier of this product.</summary>
        [JsonProperty("ProductId", Required = Required.Always)]
        [Required]
        public string ProductId
        {
            get { return _productId; }
            set
            {
                if (_productId != value)
                {
                    _productId = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>Gets the name of this product.</summary>
        [JsonProperty("Name", Required = Required.Always)]
        [Required]
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>Gets the broadcast message to send to clients using this product.</summary>
        [JsonProperty("BroadcastMessage", Required = Required.Default)]
        public string BroadcastMessage
        {
            get { return _broadcastMessage; }
            set
            {
                if (_broadcastMessage != value)
                {
                    _broadcastMessage = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>Gets or sets when to cease broadcasting (in UTC) this specified broadcast message.  This should be null if BroadcastMessage is not supplied.</summary>
        [JsonProperty("BroadcastValidUntil", Required = Required.Default)]
        public DateTime? BroadcastValidUntil
        {
            get { return _broadcastValidUntil; }
            set
            {
                if (_broadcastValidUntil != value)
                {
                    _broadcastValidUntil = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>Gets a list of the versions of this product that have ever existed.</summary>
        [JsonProperty("Versions", Required = Required.Default)]
        public List<ProductVersion> Versions
        {
            get { return _versions; }
            set
            {
                if (_versions != value)
                {
                    _versions = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>Gets the time this product was created.</summary>
        [JsonProperty("CreateTime", Required = Required.Always)]
        [Required]
        public DateTime CreateTime
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

        public event PropertyChangedEventHandler PropertyChanged;

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static Product FromJson(string data)
        {
            return JsonConvert.DeserializeObject<Product>(data);
        }

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>A product version represents a single version of a product right down to the revision level.</summary>
    [JsonObject(MemberSerialization.OptIn)]
    [GeneratedCode("NJsonSchema", "2.60.6043.38307")]
    public partial class ProductVersion : INotifyPropertyChanged
    {
        private string _version;
        private bool _isLatest;
        private bool _isPreRelease;
        private string _downloadUrl;
        private DateTime _createTime;

        /// <summary>Gets the version string for this version of the software.</summary>
        [JsonProperty("Version", Required = Required.Always)]
        [Required]
        public string Version
        {
            get { return _version; }
            set
            {
                if (_version != value)
                {
                    _version = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>Gets whether or not this is the latest verison of the product.</summary>
        [JsonProperty("IsLatest", Required = Required.Always)]
        public bool IsLatest
        {
            get { return _isLatest; }
            set
            {
                if (_isLatest != value)
                {
                    _isLatest = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>Gets whether or not this version is pre-release (true) or generally available (false).</summary>
        [JsonProperty("IsPreRelease", Required = Required.Always)]
        public bool IsPreRelease
        {
            get { return _isPreRelease; }
            set
            {
                if (_isPreRelease != value)
                {
                    _isPreRelease = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>Gets the URL to download this specified version, if applicable.</summary>
        [JsonProperty("DownloadUrl", Required = Required.Default)]
        public string DownloadUrl
        {
            get { return _downloadUrl; }
            set
            {
                if (_downloadUrl != value)
                {
                    _downloadUrl = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>Gets the time this product version was added to the product.</summary>
        [JsonProperty("CreateTime", Required = Required.Always)]
        [Required]
        public DateTime CreateTime
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

        public event PropertyChangedEventHandler PropertyChanged;

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static ProductVersion FromJson(string data)
        {
            return JsonConvert.DeserializeObject<ProductVersion>(data);
        }

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>This object is used to represent a summary information about an LTS product and is ideally designed for displaying upgrade
    ///             information within an application.</summary>
    [JsonObject(MemberSerialization.OptIn)]
    [GeneratedCode("NJsonSchema", "2.60.6043.38307")]
    public partial class ProductSummaryResponse : INotifyPropertyChanged
    {
        private string _latestVersion;
        private string _latestDownloadUrl;
        private string _broadcastMessage;

        /// <summary>Gets the latest version number for this product.</summary>
        [JsonProperty("LatestVersion", Required = Required.Default)]
        public string LatestVersion
        {
            get { return _latestVersion; }
            set
            {
                if (_latestVersion != value)
                {
                    _latestVersion = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>Gets the download URL for the latest version.</summary>
        [JsonProperty("LatestDownloadUrl", Required = Required.Default)]
        public string LatestDownloadUrl
        {
            get { return _latestDownloadUrl; }
            set
            {
                if (_latestDownloadUrl != value)
                {
                    _latestDownloadUrl = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>Gets the current broadcast message, if any, for this product.</summary>
        [JsonProperty("BroadcastMessage", Required = Required.Default)]
        public string BroadcastMessage
        {
            get { return _broadcastMessage; }
            set
            {
                if (_broadcastMessage != value)
                {
                    _broadcastMessage = value;
                    RaisePropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static ProductSummaryResponse FromJson(string data)
        {
            return JsonConvert.DeserializeObject<ProductSummaryResponse>(data);
        }

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}