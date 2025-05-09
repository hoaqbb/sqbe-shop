﻿using API.Helpers;
using API.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace API.Services
{
    public class ImageService : IImageService
    {
        private readonly Cloudinary _cloudinary;
        private readonly string _assetFolder;
        public ImageService(IOptions<CloudinarySettings> config)
        {
            var acc = new Account
            (
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(acc);
            _assetFolder = config.Value.AssetFolder;
        }
        public async Task<ImageUploadResult> AddImageAsync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                await using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    AssetFolder = _assetFolder
                };
                uploadResult = await _cloudinary.UploadAsync(uploadParams);

            }

            return uploadResult;
        }

        public async Task<List<ImageUploadResult>> AddMultipleImagesAsync(IFormFile[] files)
        {
            var uploadTasks = new List<Task<ImageUploadResult>>();
            if (files.Length > 0)
            {
                foreach (var file in files)
                {
                    uploadTasks.Add(AddImageAsync(file));
                }
            }

            var resultsArray = await Task.WhenAll(uploadTasks);

            return resultsArray.ToList();
        }

        public async Task<DeletionResult> DeleteImageAsync(string publicId)
        {
            var deleteResult = new DeletionResult();

            if (string.IsNullOrEmpty(publicId)) return deleteResult;

            var deleteParams = new DeletionParams(publicId);

            var result = await _cloudinary.DestroyAsync(deleteParams);

            return result;
        }

        public async Task<List<DeletionResult>> DeleteMultipleImagesAsync(List<string> publicIds)
        {
            var deleteTasks = new List<Task<DeletionResult>>();

            if (publicIds.Count() > 0)
            {
                foreach (var publicId in publicIds)
                {
                    deleteTasks.Add(DeleteImageAsync(publicId));
                }
            }

            var resultsArray = await Task.WhenAll(deleteTasks);

            return resultsArray.ToList();
        }
    }
}
