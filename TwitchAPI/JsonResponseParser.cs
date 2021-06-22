using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using TwitchAPI.DTO;

namespace TwitchAPI
{
    public static class JsonResponseParser
    {
        public static IEnumerable<StreamDto> SteamsResponseParse(string responseContent)
        {
            var jArr = JArray.Parse(responseContent);
            if (!(jArr.First() is JObject jArrElement))
                throw new ArgumentException($"Wrong response. {jArr}");

            if (!jArrElement.ContainsKey("data") || !(jArrElement["data"] is JObject dataValue))
                throw new ArgumentException($"Wrong response. {jArrElement}");

            if (!dataValue.ContainsKey("streams") || !(dataValue["streams"] is JObject streams))
                throw new ArgumentException($"Wrong response. {dataValue}");

            if (!streams.ContainsKey("edges") || !(streams["edges"] is JArray edges))
                throw new ArgumentException($"Wrong response. {streams}");

            foreach (var edgeToken in edges)
            {
                if (!(edgeToken is JObject edge))
                    throw new ArgumentException($"Wrong response. {edgeToken}");

                if (!edge.ContainsKey("node") || !(edge["node"] is JObject node))
                    throw new ArgumentException($"Wrong response. {edge}");

                if (!TryParseNode(node, out var streamInfo))
                    throw new ArgumentException($"Wrong response. {node}");

                yield return new StreamDto(streamInfo);
            }
        }

        public static bool VideoPlayerStatusOverlayChannelResponseParseUserIsOnline(string responseContent)
        {
            var jArr = JArray.Parse(responseContent);
            if (!(jArr.First() is JObject jArrElement))
                throw new ArgumentException($"Wrong response. {jArr}");

            if (!jArrElement.ContainsKey("data") || !(jArrElement["data"] is JObject dataValue))
                throw new ArgumentException($"Wrong response. {jArrElement}");

            if (!dataValue.ContainsKey("user") || !(dataValue["user"] is JObject userValue))
                throw new ArgumentException($"Wrong response. {dataValue}");

            if (!userValue.TryGetValue("stream", out var stream))
                throw new ArgumentException($"Wrong response. {userValue}");

            return stream.HasValues;
        }

        public static bool VideoPlayerStatusOverlayChannelResponseParseUserAvailable(string responseContent)
        {
            var jArr = JArray.Parse(responseContent);
            if (!(jArr.First() is JObject jArrElement))
                throw new ArgumentException($"Wrong response. {jArr}");

            if (!jArrElement.ContainsKey("data") || !(jArrElement["data"] is JObject dataValue))
                throw new ArgumentException($"Wrong response. {jArrElement}");

            if (!dataValue.TryGetValue("user", out var user))
                throw new ArgumentException($"Wrong response. {dataValue}");

            return user.HasValues;
        }

        private static bool TryParseNode(JObject node,
            out (string Broadcaster, string Title, string PrewiewImageUrl, string GameName, int ViewersCount) result)
        {
            result = default;

            if (!node.TryGetValue("title", out var title))
                return false;

            if (!node.TryGetValue("viewersCount", out var viewersCountJToken))
                return false;

            if (!int.TryParse(viewersCountJToken.ToString(), out var viewersCount))
                return false;

            if (!node.TryGetValue("previewImageURL", out var previewImageUrl))
                return false;

            if (!node.ContainsKey("game") || !(node["game"] is JObject game))
                return false;

            if (!game.TryGetValue("name", out var gameName))
                return false;

            if (!node.ContainsKey("broadcaster") || !(node["broadcaster"] is JObject broadcaster))
                return false;

            if (!broadcaster.TryGetValue("login", out var broadcasterName))
                return false;

            result = (broadcasterName.ToString(), title.ToString(), previewImageUrl.ToString(), gameName.ToString(),
                viewersCount);
            return true;
        }

        public static (string Token, string Signature) PlaybackAccessToken_TemplateResponseParse(string responseContent)
        {
            var jObj = JObject.Parse(responseContent);
            var dataProperty = jObj.Properties().First();
            if (!(dataProperty.Value is JObject dataPropertyValue))
                throw new ArgumentException($"Wrong response. {dataProperty}");

            var streamPlaybackAccessTokenProperty = dataPropertyValue.Properties().First();
            if (!(streamPlaybackAccessTokenProperty.Value is JObject streamPlaybackAccessTokenPropertyValue))
                throw new ArgumentException($"Wrong response. {streamPlaybackAccessTokenProperty}");

            var token = streamPlaybackAccessTokenPropertyValue["value"]?.ToString();
            var signature = streamPlaybackAccessTokenPropertyValue["signature"]?.ToString();

            return (token, signature);
        }

        public static IEnumerable<UserDto> SearchResultsPage_SearchResultsResponseParse(string responseContent)
        {
            var jArr = JArray.Parse(responseContent);

            if (!(jArr.First() is JObject jArrElement))
                throw new ArgumentException($"Wrong response. {jArr}");

            if (!jArrElement.ContainsKey("data") || !(jArrElement["data"] is JObject dataValue))
                throw new ArgumentException($"Wrong response. {jArrElement}");

            if (!dataValue.ContainsKey("searchFor") || !(dataValue["searchFor"] is JObject searchForValue))
                throw new ArgumentException($"Wrong response. {dataValue}");


            if (!searchForValue.ContainsKey("channels") || !(searchForValue["channels"] is JObject channelsValue))
                throw new ArgumentException($"Wrong response. {searchForValue}");

            if (!channelsValue.ContainsKey("edges") || !(channelsValue["edges"] is JArray edges))
                throw new ArgumentException($"Wrong response. {channelsValue}");

            foreach (var edgeToken in edges)
            {
                if (!(edgeToken is JObject edge))
                    throw new ArgumentException($"Wrong response. {edgeToken}");

                if (!edge.ContainsKey("item") || !(edge["item"] is JObject item))
                    throw new ArgumentException($"Wrong response. {edge}");

                var streamDto = TryParseItemForStream(item, out var streamInfo) ? new StreamDto(streamInfo) : null;
                if (!TryParseItemForUser(item, out var userInfo))
                    throw new ArgumentException();

                yield return new UserDto(userInfo, streamDto);
            }
        }

        private static bool TryParseItemForStream(
            JObject item,
            out (string Broadcaster, string Title, string PrewiewImageUrl, string GameName, int ViewersCount) result)
        {
            result = default;
            if (!item.TryGetValue("stream", out var streamJToken))
                return false;

            if (!item.TryGetValue("login", out var broadcaster))
                return false;

            if (!streamJToken.HasValues || !(streamJToken is JObject stream))
                return false;

            if (!item.ContainsKey("broadcastSettings") || !(item["broadcastSettings"] is JObject broadcastSettings))
                return false;

            if (!broadcastSettings.TryGetValue("title", out var title))
                return false;

            if (!stream.ContainsKey("game") || !(stream["game"] is JObject game))
                return false;

            if (!game.TryGetValue("name", out var gameName))
                return false;

            if (!stream.TryGetValue("previewImageURL", out var previewImageUrl))
                return false;

            if (!stream.TryGetValue("viewersCount", out var viewersCountJToken))
                return false;

            if (!int.TryParse(viewersCountJToken.ToString(), out var viewersCount))
                return false;

            result = (broadcaster.ToString(), title.ToString(), previewImageUrl.ToString(), gameName.ToString(),
                viewersCount);

            return true;
        }

        private static bool TryParseItemForUser(JObject item,
            out (string Name, string ProfileImageUrl, string UserDescription, int FollowersCount) result)
        {
            result = default;
            if (!item.TryGetValue("login", out var name))
                return false;

            if (!item.TryGetValue("profileImageURL", out var profileImageUrl))
                return false;

            if (!item.TryGetValue("description", out var userDescription))
                return false;

            if (!item.ContainsKey("followers") || !(item["followers"] is JObject followers))
                return false;

            if (!followers.TryGetValue("totalCount", out var totalCount))
                return false;

            if (!int.TryParse(totalCount.ToString(), out var followersCount))
                return false;

            result = (name.ToString(), profileImageUrl.ToString(), userDescription.ToString(), followersCount);
            return true;
        }
    }
}