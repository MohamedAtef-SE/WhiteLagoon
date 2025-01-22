using AutoMapper;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Web.ViewModels;

namespace WhiteLagoon.Web.Mapping
{
    public class VillaImageURLResolver : IValueResolver<Villa, VillaViewModel, string?>
    {
        private readonly IConfiguration _configuration;

        public VillaImageURLResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string? Resolve(Villa source, VillaViewModel destination, string? destMember, ResolutionContext context)
        {
            var baseURL = _configuration.GetSection("URLs")["baseURL"];

            if (!string.IsNullOrEmpty(source.ImageURL))
            {
                if (source.ImageURL.StartsWith(baseURL!))
                {
                    // For DB
                  return source.ImageURL.Replace(baseURL!,string.Empty);
                }
                    // retrieve and display
                return Path.Combine(baseURL!,source.ImageURL);
            }
            return null;
            }
        }
    }
