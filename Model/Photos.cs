//
// Photos.cs
//
// Author:  endofunk
//
// Copyright (c) 2019 
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Endofunk.FX;
using static Endofunk.FX.Prelude;
using Endofunk.FX.Net;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace WeatherFX.Model {
  using JP = JsonPropertyAttribute;

  public static partial class Unsplash {
    public static Result<Root> Get(string query) => Config.Unsplash.URL(query).DownloadAndDeserialize(Create);
    public static Func<Root, Result<Uri>> FirstPhoto() => root => Try(() => root.Photos.Results.Where(x => !x.Sponsored).First().Urls.Regular);
    public static Func<Uri, Result<string>> DownloadFile(string filepath) => uri => Try(() => {
      new WebClient().DownloadFile(uri, filepath);
      return filepath;
    });

    public static Func<string, Result<Texture2D>> OpenFileStreamAsTexture2D(GraphicsDeviceManager graphics) => filepath => Try(() => {
      using (var fileStream = new FileStream(filepath, FileMode.Open)) {
        return Texture2D.FromStream(graphics.GraphicsDevice, fileStream);
      }
    });

    public class Root {
      [JP("photos")] public readonly Photos Photos;
      [JP("collections")] public readonly Collections Collections;
      [JP("users")] public readonly Collections Users;
      [JP("related_searches")] public readonly List<RelatedSearch> RelatedSearches;
      [JP("meta")] public readonly Meta Meta;
      public Root(Photos photos, Collections collections, Collections users, List<RelatedSearch> relatedSearches, Meta meta) {
        Photos = photos;
        Collections = collections;
        Users = users;
        RelatedSearches = relatedSearches;
        Meta = meta;
      }
      public override string ToString() => $"Root: [Photos: {Photos}, Collections: {Collections}, Users: {Collections}, RelatedSearches: [{RelatedSearches.Join(", ")}], Meta: {Meta}]";
    }

    public class Collections {
      [JP("total")] public readonly int Total;
      [JP("total_pages")] public readonly int TotalPages;
      [JP("results")] public readonly List<CollectionsResult> Results;
      public Collections(int total, int totalPages, List<CollectionsResult> results) {
        Total = total;
        TotalPages = totalPages;
        Results = results;
      }
      public override string ToString() => $"Collections: [Total: {Total}, TotalPages: {TotalPages}, Results: [{Results.Join(", ")}]]";
    }

    public class CollectionsResult {
      [JP("id")] public readonly string Id;
      [JP("title")] public readonly string Title;
      [JP("description")] public readonly string Description;
      [JP("published_at")] public readonly DateTimeOffset PublishedAt;
      [JP("updated_at")] public readonly DateTimeOffset UpdatedAt;
      [JP("curated")] public readonly bool Curated;
      [JP("featured")] public readonly bool Featured;
      [JP("total_photos")] public readonly int TotalPhotos;
      [JP("private")] public readonly bool Private;
      [JP("share_key")] public readonly string ShareKey;
      [JP("tags")] public readonly List<RelatedSearch> Tags;
      [JP("links")] public readonly PurpleLinks Links;
      [JP("user")] public readonly User User;
      [JP("cover_photo")] public readonly CoverPhoto CoverPhoto;
      [JP("preview_photos")] public readonly List<PreviewPhoto> PreviewPhotos;
      public CollectionsResult(string id, string title, string description, DateTimeOffset publishedAt, DateTimeOffset updatedAt, bool curated, bool featured, int totalPhotos, bool @private, string shareKey, List<RelatedSearch> tags, PurpleLinks links, User user, CoverPhoto coverPhoto, List<PreviewPhoto> previewPhotos) {
        Id = id;
        Title = title;
        Description = description;
        PublishedAt = publishedAt;
        UpdatedAt = updatedAt;
        Curated = curated;
        Featured = featured;
        TotalPhotos = totalPhotos;
        Private = @private;
        ShareKey = shareKey;
        Tags = tags;
        Links = links;
        User = user;
        CoverPhoto = coverPhoto;
        PreviewPhotos = previewPhotos;
      }
      public override string ToString() => $"CollectionsResult: [Id: {Id}, Title: {Title}, Description: {Description}, PublishedAt: {PublishedAt}, " +
        $"UpdatedAt: {UpdatedAt}, Curated: {Curated}, Featured: {Featured}, TotalPhotos: {TotalPhotos}, Private: {Private}, ShareKey: {ShareKey}, " +
        $"Tags: [{Tags.Join(", ")}], Links: {Links}, User: {User}, CoverPhoto: {CoverPhoto}, PreviewPhotos: [{PreviewPhotos.Join(", ")}]]";
    }

    public class CoverPhoto {
      [JP("id")] public readonly string Id;
      [JP("created_at")] public readonly DateTimeOffset CreatedAt;
      [JP("updated_at")] public readonly DateTimeOffset UpdatedAt;
      [JP("width")] public readonly int Width;
      [JP("height")] public readonly int Height;
      [JP("color")] public readonly string Color;
      [JP("description")] public readonly string Description;
      [JP("alt_description")] public readonly string AltDescription;
      [JP("urls")] public readonly Urls Urls;
      [JP("links")] public readonly CoverPhotoLinks Links;
      [JP("categories")] public readonly List<object> Categories;
      [JP("sponsored")] public readonly bool Sponsored;
      [JP("sponsored_by")] public readonly object SponsoredBy;
      [JP("sponsored_impressions_id")] public readonly object SponsoredImpressionsId;
      [JP("likes")] public readonly int Likes;
      [JP("liked_by_user")] public readonly bool LikedByUser;
      [JP("current_user_collections")] public readonly List<object> CurrentUserCollections;
      [JP("user")] public readonly User User;
      public CoverPhoto(string id, DateTimeOffset createdAt, DateTimeOffset updatedAt, int width, int height, string color, string description, string altDescription, Urls urls, CoverPhotoLinks links, List<object> categories, bool sponsored, object sponsoredBy, object sponsoredImpressionsId, int likes, bool likedByUser, List<object> currentUserCollections, User user) {
        Id = id;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        Width = width;
        Height = height;
        Color = color;
        Description = description;
        AltDescription = altDescription;
        Urls = urls;
        Links = links;
        Categories = categories;
        Sponsored = sponsored;
        SponsoredBy = sponsoredBy;
        SponsoredImpressionsId = sponsoredImpressionsId;
        Likes = likes;
        LikedByUser = likedByUser;
        CurrentUserCollections = currentUserCollections;
        User = user;
      }
      public override string ToString() => $"CoverPhoto: [Id: {Id}, CreatedAt: {CreatedAt}, UpdatedAt: {UpdatedAt}, Width: {Width}, Height: {Height}, " +
        $"Color: {Color}, Description: {Description}, AltDescription: {AltDescription}, Urls: {Urls}, Links: {Links}, Categories: [{Categories.Join(", ")}], " +
        $"Sponsored: {Sponsored}, SponsoredBy: {SponsoredBy}, SponsoredImpressionsId: {SponsoredImpressionsId}, Likes: {Likes}, LikedByUser: {LikedByUser}, " +
        $"CurrentUserCollections: [{CurrentUserCollections.Join(", ")}], User: {User}]";
    }

    public class CoverPhotoLinks {
      [JP("self")] public readonly Uri Self;
      [JP("html")] public readonly Uri Html;
      [JP("download")] public readonly Uri Download;
      [JP("download_location")] public readonly Uri DownloadLocation;

      public CoverPhotoLinks(Uri self, Uri html, Uri download, Uri downloadLocation) {
        Self = self;
        Html = html;
        Download = download;
        DownloadLocation = downloadLocation;
      }
      public override string ToString() => $"CoverPhotoLinks: [Self: {Self}, Html: {Html}, Download: {Download}, DownloadLocation: {DownloadLocation}]";
    }

    public class Urls {
      [JP("raw")] public readonly Uri Raw;
      [JP("full")] public readonly Uri Full;
      [JP("regular")] public readonly Uri Regular;
      [JP("small")] public readonly Uri Small;
      [JP("thumb")] public readonly Uri Thumb;
      public Urls(Uri raw, Uri full, Uri regular, Uri small, Uri thumb) {
        Raw = raw;
        Full = full;
        Regular = regular;
        Small = small;
        Thumb = thumb;
      }
      public override string ToString() => $"Urls:[Raw: {Raw}, Full: {Full}, Regular: {Regular}, Small: {Small}, Thumb: {Thumb}]";
    }

    public class User {
      [JP("id")] public readonly string Id;
      [JP("updated_at")] public readonly DateTimeOffset UpdatedAt;
      [JP("username")] public readonly string Username;
      [JP("name")] public readonly string Name;
      [JP("first_name")] public readonly string FirstName;
      [JP("last_name")] public readonly string LastName;
      [JP("twitter_username")] public readonly string TwitterUsername;
      [JP("portfolio_url")] public readonly Uri PortfolioUrl;
      [JP("bio")] public readonly string Bio;
      [JP("location")] public readonly string Location;
      [JP("links")] public readonly UserLinks Links;
      [JP("profile_image")] public readonly ProfileImage ProfileImage;
      [JP("instagram_username")] public readonly string InstagramUsername;
      [JP("total_collections")] public readonly int TotalCollections;
      [JP("total_likes")] public readonly int TotalLikes;
      [JP("total_photos")] public readonly int TotalPhotos;
      [JP("accepted_tos")] public readonly bool AcceptedTos;
      public User(string id, DateTimeOffset updatedAt, string username, string name, string firstName, string lastName, string twitterUsername, Uri portfolioUrl, string bio, string location, UserLinks links, ProfileImage profileImage, string instagramUsername, int totalCollections, int totalLikes, int totalPhotos, bool acceptedTos) {
        Id = id;
        UpdatedAt = updatedAt;
        Username = username;
        Name = name;
        FirstName = firstName;
        LastName = lastName;
        TwitterUsername = twitterUsername;
        PortfolioUrl = portfolioUrl;
        Bio = bio;
        Location = location;
        Links = links;
        ProfileImage = profileImage;
        InstagramUsername = instagramUsername;
        TotalCollections = totalCollections;
        TotalLikes = totalLikes;
        TotalPhotos = totalPhotos;
        AcceptedTos = acceptedTos;
      }
      public override string ToString() => $"User:[Id = {Id}, UpdatedAt: {UpdatedAt}, Username: {Username}, Name: {Name}, FirstName: {FirstName}, " +
        $"LastName: {LastName}, TwitterUsernam: {TwitterUsername}, PortfolioUrl: {PortfolioUrl}, Bio: {Bio}, Location: {Location}, Links: {Links}, " +
        $"ProfileImage: {ProfileImage}, InstagramUsername: {InstagramUsername}, TotalCollections: {TotalCollections}, TotalLikes: {TotalLikes}, " +
        $"TotalPhotos: {TotalPhotos}, AcceptedTos: {AcceptedTos}]";
    }

    public class UserLinks {
      [JP("self")] public readonly Uri Self;
      [JP("html")] public readonly Uri Html;
      [JP("photos")] public readonly Uri Photos;
      [JP("likes")] public readonly Uri Likes;
      [JP("portfolio")] public readonly Uri Portfolio;
      [JP("following")] public readonly Uri Following;
      [JP("followers")] public readonly Uri Followers;
      public UserLinks(Uri self, Uri html, Uri photos, Uri likes, Uri portfolio, Uri following, Uri followers) {
        Self = self;
        Html = html;
        Photos = photos;
        Likes = likes;
        Portfolio = portfolio;
        Following = following;
        Followers = followers;
      }
      public override string ToString() => $"UserLinks: [Self: {Self}, Html: {Html}, Photos: {Photos}, Likes: {Likes}, Portfolio: {Portfolio}, " +
        $"Following: {Following}, Followers: {Followers}]";
    }

    public class ProfileImage {
      [JP("small")] public readonly Uri Small;
      [JP("medium")] public readonly Uri Medium;
      [JP("large")] public readonly Uri Large;
      public ProfileImage(Uri small, Uri medium, Uri large) {
        Small = small;
        Medium = medium;
        Large = large;
      }
      public override string ToString() => $"ProfileImage: [Small: {Small}, Medium: {Medium}, Large: {Large}]";
    }

    public class PurpleLinks {
      [JP("self")] public readonly Uri Self;
      [JP("html")] public readonly Uri Html;
      [JP("photos")] public readonly Uri Photos;
      [JP("related")] public readonly Uri Related;
      public PurpleLinks(Uri self, Uri html, Uri photos, Uri related) {
        Self = self;
        Html = html;
        Photos = photos;
        Related = related;
      }
      public override string ToString() => $"PurpleLinks: [Self: {Self}, Html: {Html}, Photos: {Photos}, Related: {Related}]";
    }

    public class PreviewPhoto {
      [JP("id")] public readonly string Id;
      [JP("urls")] public readonly Urls Urls;
      public PreviewPhoto(string id, Urls urls) {
        Id = id;
        Urls = urls;
      }
      public override string ToString() => $"Id: {Id}, Urls: {Urls}";
    }

    public class RelatedSearch {
      [JP("title")] public readonly string Title;
      public RelatedSearch(string title) {
        Title = title;
      }
      public override string ToString() => $"RelatedSearch: [Title: {Title}]";
    }

    public class Meta {
      [JP("keyword")] public readonly string Keyword;
      [JP("text")] public readonly object Text;
      [JP("title")] public readonly string Title;
      [JP("description")] public readonly object Description;
      [JP("suffix")] public readonly object Suffix;
      [JP("index")] public readonly bool Index;
      [JP("h1")] public readonly object H1;
      [JP("canonical")] public readonly object Canonical;
      public Meta(string keyword, object text, string title, object description, object suffix, bool index, object h1, object canonical) {
        Keyword = keyword;
        Text = text;
        Title = title;
        Description = description;
        Suffix = suffix;
        Index = index;
        H1 = h1;
        Canonical = canonical;
      }
      public override string ToString() => $"Meta: [Keyword: {Keyword}, Text: {Text}, Title: {Title}, Description: {Description}, Suffix: {Suffix}, Index: {Index}, H1: {H1}, Canonical: {Canonical}]";
    }

    public class Photos {
      [JP("total")] public readonly int Total;
      [JP("total_pages")] public readonly int TotalPages;
      [JP("results")] public readonly List<PhotosResult> Results;
      public Photos(int total, int totalPages, List<PhotosResult> results) {
        Total = total;
        TotalPages = totalPages;
        Results = results;
      }
      public override string ToString() => $"Photos: [Total: {Total}, TotalPages: {TotalPages}, Results: [{Results.Join(", ")}]]";
    }

    public class PhotosResult {
      [JP("id")] public readonly string Id;
      [JP("created_at")] public readonly DateTimeOffset CreatedAt;
      [JP("updated_at")] public readonly DateTimeOffset UpdatedAt;
      [JP("width")] public readonly int Width;
      [JP("height")] public readonly int Height;
      [JP("color")] public readonly string Color;
      [JP("description")] public readonly string Description;
      [JP("alt_description")] public readonly string AltDescription;
      [JP("urls")] public readonly Urls Urls;
      [JP("links")] public readonly CoverPhotoLinks Links;
      [JP("categories")] public readonly List<object> Categories;
      [JP("sponsored")] public readonly bool Sponsored;
      [JP("sponsored_by")] public readonly object SponsoredBy;
      [JP("sponsored_impressions_id")] public readonly object SponsoredImpressionsId;
      [JP("likes")] public readonly int Likes;
      [JP("liked_by_user")] public readonly bool LikedByUser;
      [JP("current_user_collections")] public readonly List<object> CurrentUserCollections;
      [JP("user")] public readonly User User;
      [JP("tags")] public readonly List<RelatedSearch> Tags;
      public PhotosResult(string id, DateTimeOffset createdAt, DateTimeOffset updatedAt, int width, int height, string color, string description, string altDescription, Urls urls, CoverPhotoLinks links, List<object> categories, bool sponsored, object sponsoredBy, object sponsoredImpressionsId, int likes, bool likedByUser, List<object> currentUserCollections, User user, List<RelatedSearch> tags) {
        Id = id;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        Width = width;
        Height = height;
        Color = color;
        Description = description;
        AltDescription = altDescription;
        Urls = urls;
        Links = links;
        Categories = categories;
        Sponsored = sponsored;
        SponsoredBy = sponsoredBy;
        SponsoredImpressionsId = sponsoredImpressionsId;
        Likes = likes;
        LikedByUser = likedByUser;
        CurrentUserCollections = currentUserCollections;
        User = user;
        Tags = tags;
      }
      public override string ToString() => $"PhotosResult: [Id: {Id}, CreatedAt: {CreatedAt}, UpdatedAt: {UpdatedAt}, Width: {Width}, Height: {Height}, Color: {Color}, " +
        $"Description: {Description}, AltDescription: {AltDescription}, Urls: {Urls}, Links: {Links}, Categories: [{Categories.Join(", ")}], Sponsored: {Sponsored}, " +
        $"SponsoredBy: {SponsoredBy}, SponsoredImpressionsId: {SponsoredImpressionsId}, Likes: {Likes}, LikedByUser: {LikedByUser}, CurrentUserCollections: [{CurrentUserCollections.Join(", ")}], " +
        $"User: {User}, Tags: {Tags}]";
    }

    private static Root FromJson(string json) => JsonConvert.DeserializeObject<Root>(json, Converter.Settings);
    private static Func<string, Result<Root>> Create => json => FromJson(json).ToResult();

    private static class Converter {
      public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings {
        MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
        DateParseHandling = DateParseHandling.None,
        Converters = { new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal } }
      };
    }
  }
}
