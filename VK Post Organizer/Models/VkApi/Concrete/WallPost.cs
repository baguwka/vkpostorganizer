using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using vk.Models.VkApi.Entities;
using vk.Utils;

namespace vk.Models.VkApi {
   public class WallPostEventArgs {
      public int WallId { get; set; }
      public Attachments Attachments { get; set; }
      public string Message { get; set; }
      public int Date { get; set; }
      public bool Postponed { get; set; }
      public int PublishingDate { get; set; }
   }

   public class PhotosGetById {
      private readonly VkApi _api;

      public PhotosGetById(VkApi api) {
         _api = api;
      }

      public async Task<string> Get(string photo) {
         var parameters = VkParameters.New()
            .AddParameter("photos", photo);
         var result = await _api.ExecuteMethodAsync("photos.getById", parameters, CancellationToken.None);
         return result;
      }
   }

   [UsedImplicitly]
   public class WallPost {
      private readonly VkApi _api;

      public event EventHandler<WallPostEventArgs> PostSuccessful;

      public WallPost(VkApi api) {
         this._api = api;
      }

      public async Task<WallPostResponse> PostAsync(int wallId, string message, int date, [NotNull] Attachments attachments) {
         if (attachments == null) throw new ArgumentNullException(nameof(attachments));

         var parameters = makeAQuery(wallId, message, date, attachments);
         var result = await postAsync(parameters, CancellationToken.None);

         OnPostSuccessful(new WallPostEventArgs{
            WallId = wallId,
            Message = message,
            Date = date,
            Attachments = attachments,
            Postponed = true,
            PublishingDate = UnixTimeConverter.ToUnix(DateTimeOffset.Now.DateTime)
         });

         return result;
      }

      private async Task<WallPostResponse> postAsync(VkParameters parameters, CancellationToken ct) {
         var response = await _api.ExecuteMethodAsync("wall.post", parameters, ct).ConfigureAwait(false);
         return JsonConvert.DeserializeObject<WallPostResponse>(response);
      }

      private static VkParameters makeAQuery(int wallId, string message, int date, [NotNull] Attachments attachments) {
         if (attachments == null) throw new ArgumentNullException(nameof(attachments));
         if (message == null) {
            message = "";
         }

         var parameters = VkParameters.New()
            .AddParameter("owner_id", wallId)
            .AddParameter("filter", "postponed")
            .AddParameter("publish_date", date)
            .AddParameter("signed", 0)
            .AddParameter("from_group", 1)
            .AddParameter("message", message);

         parameters.AppendParameters(attachments.GetAsParameters());

         return parameters;
      }

      protected virtual void OnPostSuccessful(WallPostEventArgs e) {
         PostSuccessful?.Invoke(this, e);
      }
   }

   [UsedImplicitly]
   public class WallPostResponse {
      [JsonProperty(PropertyName = "response")]
      public WallPostInfo Content { get; set; }
   }

   [UsedImplicitly]
   public class WallPostInfo {
      [JsonProperty(PropertyName = "post_id")]
      public int PostID { get; set; }
   }
}