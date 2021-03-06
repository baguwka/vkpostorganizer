using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using NLog;
using vk.Models.VkApi;
using vk.Models.VkApi.Entities;

namespace vk.Models.History {
   [UsedImplicitly]
   public class HistoryController
   {
      private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

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

      private void onWallPostSuccessful(object sender, WallPostInfo info) {
         Task.Run(async () => {
            try {
               var userid = 0;
               var users = await _api.UsersGet.GetAsync(QueryParameters.No());
               var user = users.Content?.FirstOrDefault();
               if (user != null) {
                  userid = user.ID;
               }

               var post = new HistoryPost {
                  OwnerId = userid,
                  WallId = info.OwnerId,
                  PostId = info.PostId,
                  Message = info.Message,
                  PostponedDateUnix = info.PostponedDate,
                  IsRepost = false,
                  Date = info.PublishingDate,
                  Attachments = info.Attachments.Exposed.Select(attachment => attachment.Photo.GetLargest()).ToList()
               };

               await _historyPublisher.LogAsync(post);
            }
            catch (VkException ex) {
               logger.Trace(ex, $"������ ��� ���������� ����� ������� ��� ����� {info.OwnerId}_{info.PostId}");
            }
         });
      }
   }
}