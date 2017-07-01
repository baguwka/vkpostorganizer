using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using JetBrains.Annotations;
using Prism.Commands;
using vk.Models;
using vk.Models.VkApi.Entities;

namespace vk.ViewModels {
   public class HistoryPostViewModel : PostViewModelBase {
      private HistoryPost _post;
      private string _date;
      private string _publisherName;
      private int _postId;

      public ICommand OpenPostCommand { get; set; }

      private HistoryPostViewModel()
      {
         OpenPostCommand = new DelegateCommand(openPostCommand,
            () => PostId != 0)
            .ObservesProperty(() => PostId);
      }

      public HistoryPost Post {
         get { return _post; }
         set { SetProperty(ref _post, value); }
      }

      public string Date {
         get { return _date; }
         set { SetProperty(ref _date, value); }
      }

      public string PublisherName {
         get { return _publisherName; }
         set { SetProperty(ref _publisherName, value); }
      }

      public int PostId {
         get { return _postId; }
         set { SetProperty(ref _postId, value); }
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
         PostId = historyPost.PostId;
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

      private void openPostCommand()
      {
         if (Post.PostId == default(int))
         {
            MessageBox.Show("У выбранного Вами поста отстутсвует Id, его нельзя открыть в браузере.", "Нельзя открыть пост в браузере", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
         }

         var link = $"https://vk.com/wall{Post.WallId}_{PostId}";
         try
         {
            System.Diagnostics.Process.Start(link);
         }
         catch
         {
            MessageBox.Show("Не удалось открыть пост в браузере. Возможно в вашей ОС не настроен браузер по умолчанию." +
                            "\nСсылка на пост скопирована в Ваш буфер обмена.", "Ошибка при открытии поста.", MessageBoxButton.OK, MessageBoxImage.Information);
            Clipboard.SetText(link);
         }
      }
   }
}