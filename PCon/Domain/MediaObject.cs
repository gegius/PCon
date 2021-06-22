using System;
using System.Windows;

namespace PCon.Domain
{
    public class MediaObject
    {
        public string Url { get; }
        public string Title { get; }
        
        public string Description { get; }
        public string Author { get; }
        public TimeSpan? Duration { get; }
        public string DescriptionThumbnails { get; }
        public string TitleThumbnails { get; }
        public MediaObject(string url, string title=default, string description=default, string author=default, TimeSpan? duration=default, string descriptionThumbnails=default, string titleThumbnails=default)
        {
            Url = url;
            Title = title;
            Description = description;
            Author = author;
            Duration = duration;
            DescriptionThumbnails = descriptionThumbnails;
            TitleThumbnails = titleThumbnails;
        }
        
        public override string ToString() => $"{Author}. {Title}";
    }
}