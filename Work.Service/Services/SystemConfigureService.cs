using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Work.Models.EntityModels;
using Work.Models.RequestModels;
using Work.Models.ResponseModels;
using Work.Repository.IRepository;
using Work.Service.IService;

namespace Work.Service.Services
{
    public class SystemConfigureService : ISystemConfigureService
    {
        private readonly ISystemConfigureRepository _repository;

        public SystemConfigureService(ISystemConfigureRepository repository)
        {
            _repository = repository;
        }

        public async Task<SystemConfigureResponse> GetAsync()
        {
            var entity = await _repository.allGetAsync();

            if (entity == null)
                return new SystemConfigureResponse();

            return new SystemConfigureResponse
            {
                Id = entity.Id,
                CompanyName = entity.CompanyName,
                Email = entity.Email,
                Phone = entity.Phone,
                Logo = entity.Logo,
                ImagePath = entity.ImagePath,

                SocialMedias = entity.SocialMedias.Select(x => new SocialMediaResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Url = x.Url,
                    Icon = x.Icon,
                    IconUrl = x.IconUrl,
                    Status = x.Status
                }).ToList()
            };
        }

        public async Task SaveAsync(SaveSystemConfigurationRequest request)
        {
            try
            {


                if (request == null)
                    throw new ArgumentNullException(nameof(request));

                var jsonString = request.SocialMediaJson;

                var socialMedia = new List<SocialMediaRequest>();

                if (!string.IsNullOrWhiteSpace(jsonString))
                {
                    jsonString = jsonString.Trim();

                    // If JSON is coming as string value: "[{...}]"
                    if (jsonString.StartsWith("\"") && jsonString.EndsWith("\""))
                    {
                        jsonString = jsonString.Substring(1, jsonString.Length - 2);

                        jsonString = jsonString
                            .Replace("\\\"", "\"")
                            .Replace("\\n", "")
                            .Replace("\\r", "");
                    }

                    socialMedia = JsonSerializer.Deserialize<List<SocialMediaRequest>>(
                        jsonString,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        }
                    ) ?? new List<SocialMediaRequest>();
                }
                //==========================
                // INSERT
                //==========================
                if (string.IsNullOrWhiteSpace(request.Id))
                {
                    var entity = new SystemConfigure
                    {
                        Id = ObjectId.GenerateNewId(),
                        CompanyName = request.CompanyName,
                        Email = request.Email,
                        Phone = request.Phone,
                        Logo = request.Logo?.FileName ?? string.Empty,
                        ImagePath = request.Logo != null
                            ? "/uploads/company/" + request.Logo.FileName
                            : string.Empty,

                        SocialMedias = new List<SocialMedia>()
                    };

                    for (int i = 0; i < socialMedia.Count; i++)
                    {
                        var item = socialMedia[i];

                        IFormFile? icon = i < request.SocialMediaIcons.Count
                            ? request.SocialMediaIcons[i]
                            : null;

                        entity.SocialMedias.Add(new SocialMedia
                        {
                            Id = ObjectId.GenerateNewId(),
                            Name = item.name,
                            Url = item.url,
                            Status = item.status,
                            Icon = icon?.FileName ?? string.Empty,
                            IconUrl = icon != null
                                ? "/uploads/social/" + icon.FileName
                                : string.Empty
                        });
                    }

                    await _repository.UpsertAsync(entity);
                    return;
                }

                //==========================
                // UPDATE
                //==========================
                var existingEntity = await _repository.GetAsync(request.Id);

                if (existingEntity == null)
                    throw new Exception("System configuration not found.");

                bool isUpdated = false;

                if (existingEntity.CompanyName != request.CompanyName)
                {
                    existingEntity.CompanyName = request.CompanyName;
                    isUpdated = true;
                }

                if (existingEntity.Email != request.Email)
                {
                    existingEntity.Email = request.Email;
                    isUpdated = true;
                }

                if (existingEntity.Phone != request.Phone)
                {
                    existingEntity.Phone = request.Phone;
                    isUpdated = true;
                }

                if (request.Logo != null)
                {
                    existingEntity.Logo = request.Logo.FileName;
                    existingEntity.ImagePath = "/uploads/company/" + request.Logo.FileName;
                    isUpdated = true;
                }

                for (int i = 0; i < socialMedia.Count; i++)
                {
                    var item = socialMedia[i];

                    IFormFile? icon = i < request.SocialMediaIcons.Count
                        ? request.SocialMediaIcons[i]
                        : null;

                    SocialMedia? social = null;

                   

                    if (social == null)
                    {
                        existingEntity.SocialMedias.Add(new SocialMedia
                        {
                            Id = ObjectId.GenerateNewId(),
                            Name = item.name,
                            Url = item.url,
                            Status = item.status,
                            Icon = icon?.FileName ?? string.Empty,
                            IconUrl = icon != null
                                ? "/uploads/social/" + icon.FileName
                                : string.Empty
                        });

                        isUpdated = true;
                    }
                    else
                    {
                        if (social.Name != item.name)
                        {
                            social.Name = item.name;
                            isUpdated = true;
                        }

                        if (social.Url != item.url)
                        {
                            social.Url = item.url;
                            isUpdated = true;
                        }

                        if (social.Status != item.status)
                        {
                            social.Status = item.status;
                            isUpdated = true;
                        }

                        if (icon != null)
                        {
                            social.Icon = icon.FileName;
                            social.IconUrl = "/uploads/social/" + icon.FileName;
                            isUpdated = true;
                        }
                    }
                }

                if (isUpdated)
                {
                    await _repository.UpsertAsync(existingEntity);
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
