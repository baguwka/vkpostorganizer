using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using JetBrains.Annotations;
using vk.Models;
using vk.Models.VkApi.Entities;

namespace vk.ViewModels {
   public class HistoryPostViewModel : PostViewModelBase {
      private HistoryPost _post;
      private string _date;

      private HistoryPostViewModel() {
      }

      public HistoryPost Post {
         get { return _post; }
         set { SetProperty(ref _post, value); }
      }

      public string Date {
         get { return _date; }
         set { SetProperty(ref _date, value); }
      }

      public static HistoryPostViewModel Create([NotNull] IPost postReference) {
         if (postReference == null) {
            throw new ArgumentNullException(nameof(postReference));
         }

         var viewModel = new HistoryPostViewModel();
         return viewModel.initialize(postReference);
      }

      private HistoryPostViewModel initialize([NotNull] IPost postReference) {
         if (postReference == null) {
            throw new ArgumentNullException(nameof(postReference));
         }

         var historyPost = postReference as HistoryPost;
         if (historyPost == null) {
            throw new InvalidCastException();
         }

         Post = historyPost;
         Post.PropertyChanged += onPostPropertyChanged;
         loadPreviews();
         Date = $"{Post?.DateString} -> {Post?.PostponedDateString}";
         PostType = historyPost.IsRepost ? PostType.Repost : PostType.Post;
         return this;
      }

      protected override void loadPreviews() {
         PreviewImages.AddRange(Post.Attachments.Select(url => new ImageItem(url, url)));
         CanExpand = PreviewImages.Any();
         Expand();
      }

      private void onPostPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs) {
         Date = $"{Post?.DateString}   ->   {Post?.PostponedDateString}";
      }
   }
}