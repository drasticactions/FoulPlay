using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlaystationApp.Core.Entity;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PlaystationApp.Core.Manager
{
    public class MessageManager
    {
        public async Task<MessageGroupEntity> GetMessageGroup(string username, UserAccountEntity userAccountEntity)
        {
            try
            {
                var authenticationManager = new AuthenticationManager();
                var user = userAccountEntity.GetUserEntity();
                if (userAccountEntity.GetAccessToken().Equals("refresh"))
                {
                    await authenticationManager.RefreshAccessToken(userAccountEntity);
                }
                string url = string.Format("https://{0}-gmsg.np.community.playstation.net/groupMessaging/v1/users/{1}/messageGroups?fields=@default%2CmessageGroupId%2CmessageGroupDetail%2CtotalUnseenMessages%2CtotalMessages%2ClatestMessage&npLanguage={2}", user.Region, username, user.Language);
                // TODO: Fix this cheap hack to get around caching issue. For some reason, no-cache is not working...
                url += "&r=" + Guid.NewGuid();
                var theAuthClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userAccountEntity.GetAccessToken());
                request.Headers.CacheControl = new CacheControlHeaderValue { NoCache = true };
                var response = await theAuthClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(responseContent))
                {
                    return null;
                }
                var messageGroup = JsonConvert.DeserializeObject<MessageGroupEntity>(responseContent);
                return messageGroup;
            }
            catch (Exception)
            {
                return null;
            }
            
        }

        public async Task<Stream> GetMessageContent(string id, MessageEntity.Message message, UserAccountEntity userAccountEntity)
        {
            try
            {
                var authenticationManager = new AuthenticationManager();
                var user = userAccountEntity.GetUserEntity();
                if (userAccountEntity.GetAccessToken().Equals("refresh"))
                {
                    await authenticationManager.RefreshAccessToken(userAccountEntity);
                }
                var content = "image-data-0";
                string url = string.Format("https://{0}-gmsg.np.community.playstation.net/groupMessaging/v1/messageGroups/{1}/messages/{2}?contentKey={3}&npLanguage={4}", user.Region, id, message.messageUid, content, user.Language);
                var theAuthClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.CacheControl = new CacheControlHeaderValue { NoCache = true };
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userAccountEntity.GetAccessToken());
                var response = await theAuthClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStreamAsync();
                return responseContent;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> ClearMessages(MessageEntity messageEntity, UserAccountEntity userAccountEntity)
        {
            try
            {
                var authenticationManager = new AuthenticationManager();
                var user = userAccountEntity.GetUserEntity();
                if (userAccountEntity.GetAccessToken().Equals("refresh"))
                {
                    await authenticationManager.RefreshAccessToken(userAccountEntity);
                }
                var messageUids = new List<int>();
                messageUids.AddRange(messageEntity.messages.Where(o => o.seenFlag == false).Select(message => message.messageUid));
                if (messageUids.Count == 0) return true;
                string url = string.Format("https://{0}-gmsg.np.community.playstation.net/groupMessaging/v1/messageGroups/{1}/messages?messageUid={2}", user.Region, messageEntity.messageGroup.messageGroupId, string.Join(",", messageUids));
                var theAuthClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Put, url);
                request.Headers.CacheControl = new CacheControlHeaderValue { NoCache = true };
                request.Headers.Add("Origin", "http://psapp.dl.playstation.net");
                request.Headers.Add("Referer", "http://psapp.dl.playstation.net/psapp/6228351b09c436f44f1c53955c0a51ca/index.html");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userAccountEntity.GetAccessToken());
                request.Content = new StringContent("{\"seenFlag\":true}", Encoding.UTF8, "application/json");
                //request.Content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");
                var response = await theAuthClient.SendAsync(request);
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<MessageEntity> GetGroupConversation(string messageGroupId, UserAccountEntity userAccountEntity)
        {
            try
            {
                var authenticationManager = new AuthenticationManager();
                var user = userAccountEntity.GetUserEntity();
                if (userAccountEntity.GetAccessToken().Equals("refresh"))
                {
                    await authenticationManager.RefreshAccessToken(userAccountEntity);
                }
                var url = string.Format("https://{0}-gmsg.np.community.playstation.net/groupMessaging/v1/messageGroups/{1}/messages?fields=@default%2CmessageGroup%2Cbody&npLanguage={2}", user.Region, messageGroupId, user.Language);
                var theAuthClient = new HttpClient();
                // TODO: Fix this cheap hack to get around caching issue. For some reason, no-cache is not working...
                url += "&r=" + Guid.NewGuid();
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userAccountEntity.GetAccessToken());
                request.Headers.CacheControl = new CacheControlHeaderValue { NoCache = true };
                var response = await theAuthClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(responseContent))
                {
                    return null;
                }
                var messageGroup = JsonConvert.DeserializeObject<MessageEntity>(responseContent);
                return messageGroup;
            }
            catch (Exception)
            {
                return null;
            }
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
            catch (Exception)
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
                string test = await response.Content.ReadAsStringAsync();
                return response.IsSuccessStatusCode;

            }
            catch (Exception)
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
            catch (Exception)
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

            public int messageUid { get; set; }
        }
    }
}
