﻿using System;
using System.Linq;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using vk.Models;
using vk.Models.UrlHelper;
using vk.Models.VkApi.Entities;

namespace vk.ViewModels {
   public class PostControl : BindableBase {
      private bool _expanded;
      private PostMark _mark;
      private PostType _postType;
      public Post Post { get; }

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

      public ICommand ExpandToggleCommand { get; set; }

      public bool IsExisting { get; set; }

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

         ExpandToggleCommand = new DelegateCommand(ExpandToggle);

         var prev = Post.CopyHistory?.FirstOrDefault();
         if (prev == null) {
            loadImages();
            PostType = PostType.Post;
            return;
         }

         PostType = PostType.Repost;
         Post.ID = prev.ID;
         Post.Text = prev.Text;
         Post.Attachments = prev.Attachments;

         loadImages();
      }

      private void ExpandToggle() {
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
   }

   public enum PostMark {
      Neutral,
      Good,
      Bad,
   }
}