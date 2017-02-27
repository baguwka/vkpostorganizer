using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using JetBrains.Annotations;
using Prism.Commands;
using vk.Models;
using vk.Models.UrlHelper;
using vk.Models.VkApi.Entities;
using vk.Utils;

namespace vk.ViewModels {
   [UsedImplicitly]
   public class PostViewModel : PostViewModelBase {
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
      }

      private PostViewModel() {
         OpenPost = new DelegateCommand(openPostCommand,
               () => IsExisting == true && PostType != PostType.Missing)
            .ObservesProperty(() => IsExisting)
            .ObservesProperty(() => PostType);

         UploadAtThisDateCommand = new DelegateCommand(uploadAtThisDateCommandExecute,
               () => PostType == PostType.Missing)
            .ObservesProperty(() => PostType);
      }

      public static async Task<PostViewModel> CreateAsync([NotNull] Post postReference) {
         if (postReference == null) {
            throw new ArgumentNullException(nameof(postReference));
         }

         var postControl = new PostViewModel();
         return await postControl.initializeAsync(postReference);
      }

      private async Task<PostViewModel> initializeAsync([NotNull] Post postReference) {
         if (postReference == null) {
            throw new ArgumentNullException(nameof(postReference));
         }
         
         Post = postReference;

         //if copy history is null it's not a repost
         var prev = Post.CopyHistory?.FirstOrDefault();
         if (prev == null) {
            loadPreviews();
            PostType = PostType.Post;
         }
         else {
            //it's a repost
            var groupName = await GroupNameCache.GetGroupNameAsync(prev.OwnerId);

            PostType = PostType.Repost;
            Post.Text = $"{groupName.Substring(0, groupName.Length > 10 ? 10 : groupName.Length)}... {prev.Text}";
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

      protected override void expandToggle() {
         if (PostType == PostType.Missing) {
            Expanded = false;
            return;
         }

         Expanded = !Expanded;
      }

      public override void Expand() {
         if (PostType == PostType.Missing) {
            Expanded = false;
            return;
         }

         Expanded = true;
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