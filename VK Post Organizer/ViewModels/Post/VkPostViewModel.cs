using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using JetBrains.Annotations;
using Prism.Commands;
using vk.Models;
using vk.Models.UrlHelper;
using vk.Models.VkApi.Entities;

namespace vk.ViewModels {
   [UsedImplicitly]
   public class VkPostViewModel : PostViewModelBase {
      private PostMark _mark;
      public Post Post { get; private set; }
      public event EventHandler UploadRequested;

      public PostMark Mark {
         get { return _mark; }
         set { SetProperty(ref _mark, value); }
      }

      public ICommand OpenPost { get; set; }
      public ICommand UploadAtThisDateCommand { get; set; }

      public bool IsExisting { get; set; }

      protected override void loadPreviews() {
         PreviewImages.AddRange(from attachment in Post.Attachments
                         where attachment.Type == "photo"
                         select attachment.ObtainPhotoUrl(ImageSize.Medium, new PhotoUrlObtainer()));

         PreviewImages.AddRange(from attachment in Post.Attachments
                         where attachment.Type == "doc" && (attachment.Document.Type == (int)DocType.Image || attachment.Document.Type == (int)DocType.Gif)
                         select attachment.ObtainDocumentPreview(ImageSize.Large, new DocumentPreviewUrlObtainer()));

         CanExpand = PreviewImages.Any() && PostType != PostType.Missing;
      }

      private VkPostViewModel() {
         OpenPost = new DelegateCommand(openPostCommand,
               () => IsExisting == true && PostType != PostType.Missing)
            .ObservesProperty(() => IsExisting)
            .ObservesProperty(() => PostType);

         UploadAtThisDateCommand = new DelegateCommand(uploadAtThisDateCommandExecute,
               () => PostType == PostType.Missing)
            .ObservesProperty(() => PostType);
      }

      public static VkPostViewModel Create([NotNull] IPost postReference) {
         if (postReference == null) {
            throw new ArgumentNullException(nameof(postReference));
         }

         var postControl = new VkPostViewModel();
         return postControl.initialize(postReference);
      }

      private VkPostViewModel initialize([NotNull] IPost postReference) {
         if (postReference == null) {
            throw new ArgumentNullException(nameof(postReference));
         }

         var vkPost = postReference as Post;
         if (vkPost == null) {
            throw new InvalidCastException();
         }

         Post = vkPost;

         //if copy history is null it's not a repost
         var prev = Post.CopyHistory?.FirstOrDefault();
         if (prev == null) {
            loadPreviews();
            PostType = PostType.Post;
         }
         else {
            //it's a repost
            //var groupName = await GroupNameCache.GetGroupNameAsync(prev.OwnerId);
            PostType = PostType.Repost;
            //Post.Message = $"{groupName.Substring(0, groupName.Length > 10 ? 10 : groupName.Length)}... {prev.Message}";
            Post.Message = prev.Message;
            Post.OwnerId = prev.OwnerId;
            Post.Attachments = prev.Attachments;

            loadPreviews();
         }
         return this;
      }

      private void uploadAtThisDateCommandExecute() {
         OnUploadRequested();
      }

      private void openPostCommand() {
         System.Diagnostics.Process.Start($"https://vk.com/wall{Post.OwnerId}_{Post.ID}");
      }


      protected virtual void OnUploadRequested() {
         UploadRequested?.Invoke(this, EventArgs.Empty);
      }
   }

   public enum PostMark {
      Neutral,
      Good,
      Bad,
   }
}