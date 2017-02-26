using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using vk.Models.VkApi;
using vk.Models.VkApi.Entities;

namespace vk.Models.History {
   [UsedImplicitly]
   public class HistoryController {
      private readonly IHistoryPublisher _historyPublisher;
      private readonly VkApiProvider _api;

      public HistoryController(IHistoryPublisher historyPublisher, VkApiProvider api) {
         _historyPublisher = historyPublisher;
         _api = api;
      }

      public void Observe() {
         _api.WallPost.PostSuccessful += onWallPostSuccessful;
      }

      public void Forget() {
         _api.WallPost.PostSuccessful -= onWallPostSuccessful;
      }

      private void onWallPostSuccessful(object sender, WallPostEventArgs info) {
         Task.Run(() => {
            try {
               var userid = 0;
               var users = _api.UsersGet.GetAsync();
               var user = users.Result?.Users?.FirstOrDefault();
               if (user != null) {
                  userid = user.ID;
               }

               var post = new HistoryPost {
                  OwnerId = userid,
                  WallId = info.WallId,
                  Message = info.Message,
                  PostponedToDate = info.Date,
                  IsRepost = false,
                  PublishingDate = info.PublishingDate,
                  AttachmentUrls = info.Attachments.Exposed.Select(attachment => attachment.Photo.GetLargest()).ToList()
               };
               _historyPublisher.LogAsync(post);
            }
            catch (VkException) {
               //ignore
            }
         });
      }
   }
}