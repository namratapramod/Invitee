using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Invitee.Utils
{
    /// <summary>
    /// The http posted file base extensions.
    /// </summary>
    public static class HttpPostedFileBaseExtensions
    {
        /// <summary>
        /// The image minimum bytes - 512 bytes.
        /// </summary>
        public const int ImageMinimumBytes = 512;

        /// <summary>
        /// The image minimum bytes.
        /// </summary>
        public const int ImageMaximumBytes = 10 * 1024 * 1024;

        /// <summary>
        /// The allowed mime types - 10 Mb
        /// </summary>
        private static readonly List<string> ImageMimeTypes = new List<string>
                                   {
                                       "image/jpg",
                                       "image/jpeg",
                                       "image/pjpeg",
                                       "image/gif",
                                       "image/x-png",
                                       "image/png"
                                   };

        public static readonly List<string> VideoMimeTypes = new List<string>
        {
            "video/mp4",
            "video/quicktime",
            "video/x-msvideo",
            "video/mpeg",
            "video/3gpp",
            "video/avi"
        };

        public static readonly List<string> AudioMimeTypes = new List<string>
        {
            "audio/mpeg",
            "audio/mp4",
            "audio/x-mpegurl",
            "audio/vnd.wav",
            "audio/wave"
        };

        /// <summary>
        /// The image file extensions.
        /// </summary>
        private static readonly List<string> ImageFileExtensions = new List<string>
                                   {
                                       ".jpg",
                                       ".png",
                                       ".gif",
                                       ".jpeg"
                                   };

        private static readonly List<string> AudioFileExtensions = new List<string>
                                   {
                                       ".mp3",
                                       ".mp4",
                                       ".wav",
                                       ".m3u"
                                   };

        public static readonly List<String> VideoFileExtensions = new List<string>
        {
            ".mp4",
            ".3gp",
            ".avi",
            ".mpeg",
            ".mov"
        };

        /// <summary>
        /// Checks if posted file is a valid image file.
        /// </summary>
        /// <param name="postedFile">
        /// The posted file.
        /// </param>
        public static void ValidateImageFile(this HttpPostedFileBase postedFile)
        {
            if (!postedFile.ValidMinimumImageSize())
            {
                throw new ArgumentException($"Image file must be larger than {ImageMinimumBytes} bytes.");
            }

            if (!postedFile.ValidMaximumImageSize())
            {
                throw new ArgumentException($"Image file must be smaller than {ImageMaximumBytes / (1024 * 1024)} MB.");
            }

            if (!postedFile.ImageFile())
            {
                throw new ArgumentException("Uploaded file is not an image.");
            }
        }

        public static void ValidateVideoFile(this HttpPostedFileBase postedFile)
        {
            //if (!postedFile.ValidMinimumImageSize())
            //{
            //    throw new ArgumentException($"Image file must be larger than {ImageMinimumBytes} bytes.");
            //}

            //if (!postedFile.ValidMaximumImageSize())
            //{
            //    throw new ArgumentException($"Image file must be smaller than {ImageMaximumBytes / (1024 * 1024)} MB.");
            //}

            if (!postedFile.VideoFile())
            {
                throw new ArgumentException("Uploaded file is not an video.");
            }
        }

        public static void ValidateAudioFile(this HttpPostedFileBase postedFile)
        {
            //if (!postedFile.ValidMinimumImageSize())
            //{
            //    throw new ArgumentException($"Image file must be larger than {ImageMinimumBytes} bytes.");
            //}

            //if (!postedFile.ValidMaximumImageSize())
            //{
            //    throw new ArgumentException($"Image file must be smaller than {ImageMaximumBytes / (1024 * 1024)} MB.");
            //}

            if (!postedFile.AudioFile())
            {
                throw new ArgumentException("Uploaded file is not an audio.");
            }
        }

        /// <summary>
        /// Checks if file size is larger than allowed minimum image file size.
        /// </summary>
        /// <param name="postedFile">
        /// The posted file.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/> whether file size is larger than minimum size. True is valid and False is not.
        /// </returns>
        public static bool ValidMinimumImageSize(this HttpPostedFileBase postedFile)
        {
            return postedFile.ContentLength > ImageMinimumBytes;
        }

        /// <summary>
        /// Checks if file size is smaller than allowed maximum image file size.
        /// </summary>
        /// <param name="postedFile">
        /// The posted file.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/> whether file size is smaller than maximum size. True is valid and False is not.
        /// </returns>
        public static bool ValidMaximumImageSize(this HttpPostedFileBase postedFile)
        {
            return postedFile.ContentLength <= ImageMaximumBytes;
        }

        /// <summary>
        /// The image file.
        /// </summary>
        /// <param name="postedFile">
        /// The posted file.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool ImageFile(this HttpPostedFileBase postedFile)
        {
            var contentType = postedFile.ContentType.ToLower();

            // Check the file MIME type
            if (ImageMimeTypes.All(x => x != contentType))
            {
                return false;
            }

            // Check the file extension
            if (ImageFileExtensions.All(x => !postedFile.FileName.EndsWith(x)))
            {
                return false;
            }

            return true;
        }

        public static bool VideoFile(this HttpPostedFileBase postedFile)
        {
            var contentType = postedFile.ContentType.ToLower();

            // Check the file MIME type
            if (VideoMimeTypes.All(x => x != contentType))
            {
                return false;
            }

            // Check the file extension
            if (VideoFileExtensions.All(x => !postedFile.FileName.EndsWith(x)))
            {
                return false;
            }

            return true;
        }

        public static bool AudioFile(this HttpPostedFileBase postedFile)
        {
            var contentType = postedFile.ContentType.ToLower();

            // Check the file MIME type
            if (AudioMimeTypes.All(x => x != contentType))
            {
                return false;
            }

            // Check the file extension
            if (AudioFileExtensions.All(x => !postedFile.FileName.EndsWith(x)))
            {
                return false;
            }

            return true;
        }

        public static string GetNewFileName(this HttpPostedFileBase postedFile)
        {
            return $"{Guid.NewGuid().ToString()}{Path.GetExtension(postedFile.FileName)}";
        }
    }
}