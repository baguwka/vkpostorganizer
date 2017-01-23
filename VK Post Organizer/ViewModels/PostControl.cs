﻿using System;
using System.Linq;
using System.Windows.Input;
using JetBrains.Annotations;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using vk.Models;
using vk.Models.UrlHelper;
using vk.Models.VkApi.Entities;
using vk.Utils;
using vk.Views;

namespace vk.ViewModels {
   [UsedImplicitly]
   public class PostControl : BindableBase {
      private bool _expanded;
      private PostMark _mark;
      private PostType _postType;
      public Post Post { get; }
      public event EventHandler UploadRequested;

      public SmartCollection<ImageItem> Images { get; }

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
         set { _postType = value; }
      }

      private void loadImages() {
         foreach (var attachment in Post.Attachments) {
            if (attachment.Type == "photo") {
               Images.Add(new PhotoUrlObtainer().Obtain(attachment, ImageSize.Medium));
            }

            if (attachment.Type == "doc") {
               if (attachment.Document.Type == (int)DocType.Image || attachment.Document.Type == (int)DocType.Gif) {
                  Images.Add(new DocumentPreviewUrlObtainer().Obtain(attachment, ImageSize.Large));
               }
            }
         }
      }

      public PostControl(Post post) {
         Images = new SmartCollection<ImageItem>();
         Post = post;

         ExpandToggleCommand = new DelegateCommand(expandToggle);
         OpenPost = new DelegateCommand(openPostCommand);
         UploadAtThisDateCommand = new DelegateCommand(uploadAtThisDateCommandExecute);

         var prev = Post.CopyHistory?.FirstOrDefault();
         if (prev == null) {
            loadImages();
            PostType = PostType.Post;
            return;
         }

         var groupName = GroupNameCache.GetGroupName(prev.OwnerId);

         PostType = PostType.Repost;
         Post.Text = $"{groupName.Substring(0, groupName.Length > 10 ? 10 : groupName.Length)}... {prev.Text}";
         Post.Attachments = prev.Attachments;

         loadImages();
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