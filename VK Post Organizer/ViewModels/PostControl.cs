using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using JetBrains.Annotations;
using Prism.Commands;
using Prism.Mvvm;
using vk.Models;
using vk.Models.UrlHelper;
using vk.Models.VkApi.Entities;
using vk.Utils;

namespace vk.ViewModels {
   [UsedImplicitly]
   public class PostControl : BindableBase {
      private bool _expanded;
      private PostMark _mark;
      private PostType _postType;
      public Post Post { get; private set; }
      public event EventHandler UploadRequested;

      public ObservableCollection<ImageItem> Images { get; private set; }

      public bool Expanded {
         get { return _expanded; }
         set {
            if (PostType == PostType.Missing) value = false;
            SetProperty(ref _expanded, value);
         }
      }

      public PostMark Mark {
         get { return _mark; }
         set { SetProperty(ref _mark, value); }
      }

      public ICommand OpenPost { get; set; }
      public ICommand ExpandToggleCommand { get; set; }
      public ICommand UploadAtThisDateCommand { get; set; }
      public ICommand RapidUploadModeCommand { get; set; }

      public bool IsExisting { get; set; }

      public bool IsRapidUploadModeEnabled { get; set; }
      public bool IsImageReady { get; set; }

      public PostType PostType {
         get { return _postType; }
         set { SetProperty(ref _postType, value); }
      }

      private void loadImages() {
         Images.AddRange(from attachment in Post.Attachments
                         where attachment.Type == "photo"
                         select attachment.ObtainPhotoUrl(ImageSize.Medium, new PhotoUrlObtainer()));

         Images.AddRange(from attachment in Post.Attachments
                         where attachment.Type == "doc" && (attachment.Document.Type == (int)DocType.Image || attachment.Document.Type == (int)DocType.Gif)
                         select attachment.ObtainDocumentPreview(ImageSize.Large, new DocumentPreviewUrlObtainer()));
      }

      private PostControl() {
         
      }

      public static async Task<PostControl> CreateAsync([NotNull] Post postReference) {
         if (postReference == null) {
            throw new ArgumentNullException(nameof(postReference));
         }

         var postControl = new PostControl();
         return await postControl.InitializeAsync(postReference);
      }

      private async Task<PostControl> InitializeAsync([NotNull] Post postReference) {
         if (postReference == null) {
            throw new ArgumentNullException(nameof(postReference));
         }

         Images = new ObservableCollection<ImageItem>();

         ExpandToggleCommand = new DelegateCommand(expandToggle,
            () => PostType != PostType.Missing).ObservesProperty(() => PostType);

         OpenPost = new DelegateCommand(openPostCommand,
            () => IsExisting == true && PostType != PostType.Missing).ObservesProperty(() => IsExisting);

         UploadAtThisDateCommand = new DelegateCommand(uploadAtThisDateCommandExecute,
            () => PostType == PostType.Missing).ObservesProperty(() => PostType);

         Post = postReference;

         //if copy history is null it's not a repost
         var prev = Post.CopyHistory?.FirstOrDefault();
         if (prev == null) {
            loadImages();
            PostType = PostType.Post;
         }
         else {
            //it's a repost
            var groupName = await GroupNameCache.GetGroupNameAsync(prev.OwnerId);

            PostType = PostType.Repost;
            Post.Text = $"{groupName.Substring(0, groupName.Length > 10 ? 10 : groupName.Length)}... {prev.Text}";
            Post.Attachments = prev.Attachments;

            loadImages();
         }
         return this;
      }

      private void uploadAtThisDateCommandExecute() {
         OnUploadRequested();
      }

      private void openPostCommand() {
         System.Diagnostics.Process.Start($"https://vk.com/wall{Post.OwnerId}_{Post.ID}");
      }

      private void expandToggle() {
         Expanded = !Expanded;
      }

      public void Clear() {
         Images.Clear();
      }

      public void Expand() {
         Expanded = true;
      }

      public void Collapse() {
         Expanded = false;
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