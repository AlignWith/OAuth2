using Newtonsoft.Json.Linq;
using OAuth2.Configuration;
using OAuth2.Infrastructure;
using OAuth2.Models;

namespace OAuth2.Client.Impl
{
    /// <summary>
    /// Google authentication client.
    /// </summary>
    public class LinearClient : OAuth2Client
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LinearClient"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="configuration">The configuration.</param>
        public LinearClient(IRequestFactory factory, IClientConfiguration configuration)
            : base(factory, configuration)
        {
        }

        /// <summary>
        /// Defines URI of service which issues access code.
        /// </summary>
        protected override Endpoint AccessCodeServiceEndpoint
        {
            get
            {
                return new Endpoint
                {
                    BaseUri = "https://linear.app",
                    Resource = "/oauth/authorize"
                };
            }
        }

        /// <summary>
        /// Defines URI of service which issues access token.
        /// </summary>
        protected override Endpoint AccessTokenServiceEndpoint
        {
            get
            {
                return new Endpoint
                {
                    BaseUri = "https://api.linear.app",
                    Resource = "/oauth/token"
                };
            }
        }

        /// <summary>
        /// Defines URI of service which allows to obtain information about user which is currently logged in.
        /// </summary>
        protected override Endpoint UserInfoServiceEndpoint
        {
            get
            {
                return new Endpoint
                {
                    BaseUri = "https://api.linear.app",
                    Resource = "/graphql"
                };
            }
        }

        /// <summary>
        /// Friendly name of provider (OAuth2 service).
        /// </summary>
        public override string Name
        {
            get { return "Linear"; }
        }


        /// <summary>
        /// Should return parsed <see cref="UserInfo"/> from content received from third-party service.
        /// </summary>
        /// <param name="content">The content which is received from third-party service.</param>
        protected override UserInfo ParseUserInfo(string content)
        {
            var response = JObject.Parse(content);
            var avatarUri = response["picture"].SafeGet(x => x.Value<string>());
            const string avatarUriTemplate = "{0}?sz={1}";
            return new UserInfo
            {
                Id = response["id"].Value<string>(),
                Email = response["email"].SafeGet(x => x.Value<string>()),
                FirstName = response["given_name"].Value<string>(),
                LastName = response["family_name"].Value<string>(),
                AvatarUri =
                    {
                        Small = !string.IsNullOrWhiteSpace(avatarUri) ? string.Format(avatarUriTemplate, avatarUri, AvatarInfo.SmallSize) : string.Empty,
                        Normal = avatarUri,
                        Large = !string.IsNullOrWhiteSpace(avatarUri) ? string.Format(avatarUriTemplate, avatarUri, AvatarInfo.LargeSize): string.Empty
                    }
            };
        }
    }
}