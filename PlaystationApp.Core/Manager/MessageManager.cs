using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using PlaystationApp.Core.Entity;

namespace PlaystationApp.Core.Manager
{
    public class MessageManager
    {
        public async Task<MessageGroupEntity> GetMessageGroup(string username, UserAccountEntity userAccountEntity)
        {
            var authenticationManager = new AuthenticationManager();
            var user = userAccountEntity.GetUserEntity();
            if (userAccountEntity.GetAccessToken().Equals("refresh"))
            {
                await authenticationManager.RefreshAccessToken(userAccountEntity);
            }
            string url = string.Format("https://{0}-gmsg.np.community.playstation.net/groupMessaging/v1/users/{1}/messageGroups?fields=@default%2CmessageGroupId%2CmessageGroupDetail%2CtotalUnseenMessages%2CtotalMessages%2ClatestMessage&npLanguage={2}", user.Region, username, user.Language);
            var theAuthClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userAccountEntity.GetAccessToken());
            var response = await theAuthClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            responseContent = "[" + responseContent + "]";
            var a = JArray.Parse(responseContent);
            var b = (JObject)a[0];
            var messageGroup = await MessageGroupEntity.Parse(b, userAccountEntity);
            return messageGroup;
        }

        public async Task<MessageEntity> GetConversation(List<string> usernames , UserAccountEntity userAccountEntity)
        {
            var authenticationManager = new AuthenticationManager();
            var user = userAccountEntity.GetUserEntity();
            if (userAccountEntity.GetAccessToken().Equals("refresh"))
            {
                await authenticationManager.RefreshAccessToken(userAccountEntity);
            }
            var url = string.Format("https://{0}-gmsg.np.community.playstation.net/groupMessaging/v1/messageGroups/~{1}/messages?fields=@default%2CmessageGroup%2Cbody&npLanguage={2}", user.Region, string.Join(",", usernames.ToArray()), user.Language);
            var theAuthClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userAccountEntity.GetAccessToken());
            var response = await theAuthClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            responseContent = "[" + responseContent + "]";
            var a = JArray.Parse(responseContent);
            var b = (JObject)a[0];
            var messages = await MessageEntity.Parse(b, userAccountEntity);
            return messages;
        
        
        }

        public async Task<bool> CreatePost(string messageUserId, string post,
            UserAccountEntity userAccountEntity)
        {
            try
            {
                var user = userAccountEntity.GetUserEntity();
                var authenticationManager = new AuthenticationManager();
                const string boundary = "abcdefghijklmnopqrstuvwxyz";
                var theAuthClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, string.Format("https://{0}-gmsg.np.community.playstation.net/groupMessaging/v1/messageGroups/{1}/messages", user.Region, messageUserId));
                var form = new MultipartContent("mixed", boundary);

                if (userAccountEntity.GetAccessToken().Equals("refresh"))
                {
                    await authenticationManager.RefreshAccessToken(userAccountEntity);
                }
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userAccountEntity.GetAccessToken());

                var messageJson = new SendMessage
                {
                    message = new Message()
                    {
                        body = post,
                        fakeMessageUid = 1384958573288,
                        messageKind = 1
                    }
                };
                var json = JsonConvert.SerializeObject(messageJson);
                var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
                stringContent.Headers.Add("Content-Description", "message");
                form.Add(stringContent);
                request.Content = form;
                HttpResponseMessage response = await theAuthClient.SendAsync(request);
                await response.Content.ReadAsStringAsync();
                return response.IsSuccessStatusCode;

            }
            catch (WebException e)
            {
                { return false; }
            }
        }

        public async Task<bool> CreatePostWithMedia(string messageUserId, string post, String path, byte[] fileStream,
            UserAccountEntity userAccountEntity)
        {
            try
            {
                var user = userAccountEntity.GetUserEntity();
                var authenticationManager = new AuthenticationManager();
                const string boundary = "abcdefghijklmnopqrstuvwxyz";
                var theAuthClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, string.Format("https://{0}-gmsg.np.community.playstation.net/groupMessaging/v1/messageGroups/{1}/messages", user.Region, messageUserId));
                var form = new MultipartContent("mixed", boundary);

                if (userAccountEntity.GetAccessToken().Equals("refresh"))
                {
                    await authenticationManager.RefreshAccessToken(userAccountEntity);
                }
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userAccountEntity.GetAccessToken());

                var messageJson = new SendMessage
                {
                    message = new Message()
                    {
                        body = post,
                        fakeMessageUid = 1384958573288,
                        messageKind = 3
                    }
                };
                string json = JsonConvert.SerializeObject(messageJson);
                var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
                stringContent.Headers.Add("Content-Description", "message");
                form.Add(stringContent);
                Stream stream = new MemoryStream(fileStream);
                var t = new StreamContent(stream);
                string fileName = "testtesttesttest.jpg";
                var s = Path.GetExtension(path);
                if (s != null && s.Equals(".png"))
                {
                    t.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
                }
                else
                {
                    var extension = Path.GetExtension(path);
                    if (extension != null && (extension.Equals(".jpg") || extension.Equals(".jpeg")))
                    {
                        t.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
                    }
                    else
                    {
                        t.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/gif");
                    }
                }
                t.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
                t.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                t.Headers.Add("Content-Description", "image-data-0");
                t.Headers.Add("Content-Transfer-Encoding", "binary");
                t.Headers.ContentLength = stream.Length;
                form.Add(t);
                request.Content = form;
                HttpResponseMessage response = await theAuthClient.SendAsync(request);
                await response.Content.ReadAsStringAsync();
                return response.IsSuccessStatusCode;

            }
            catch (WebException e)
            {
                { return false; }
            }
        }

        public async Task<bool> CreatePostWithAudio(string messageUserId, string post, byte[] fileStream,
           UserAccountEntity userAccountEntity)
        {
            try
            {
                var authenticationManager = new AuthenticationManager();
                const string boundary = "abcdefghijklmnopqrstuvwxyz";
                var theAuthClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, string.Format("https://jp-gmsg.np.community.playstation.net/groupMessaging/v1/messageGroups/{0}/messages", messageUserId));
                var form = new MultipartContent("mixed", boundary);

                if (userAccountEntity.GetAccessToken().Equals("refresh"))
                {
                    await authenticationManager.RefreshAccessToken(userAccountEntity);
                }
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userAccountEntity.GetAccessToken());

                var messageJson = new SendMessage
                {
                    message = new Message()
                    {
                        body = post,
                        fakeMessageUid = 1384958573288,
                        messageKind = 1011
                    }
                };
                string json = JsonConvert.SerializeObject(messageJson);
                var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
                stringContent.Headers.Add("Content-Description", "message");
                form.Add(stringContent);
                Stream stream = new MemoryStream(fileStream);
                var t = new StreamContent(stream);
                t.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                t.Headers.Add("Content-Description", "voice-data-0");
                t.Headers.Add("Content-Type", "audio/3gpp");
                t.Headers.Add("Content-Transfer-Encoding", "binary");
                t.Headers.Add("Content-Voice-Data-Playback-Time", "14");
                t.Headers.ContentLength = stream.Length;
                form.Add(t);
                request.Content = form;
                HttpResponseMessage response = await theAuthClient.SendAsync(request);
                await response.Content.ReadAsStringAsync();
                return response.IsSuccessStatusCode;

            }
            catch (WebException e)
            {
                { return false; }
            }
        }

        public class SendMessage
        {
            public Message message { get; set; }
        }

        public class Message
        {
            public string body { get; set; }

            public long fakeMessageUid { get; set; }

            public int messageKind  {get; set; }
        }
    }
}
