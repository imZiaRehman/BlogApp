using BlogApp.Data;
using BlogApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using BlogApp.ViewModels;
using System.Net.Mail;


namespace BlogApp.Repositories
{
    public class BlogRepository
    {
        private readonly BlogDbContext _context;
        public BlogRepository(BlogDbContext context)
        {
            _context = context;
        }

        public List<Post> GetPosts()
        {
            return _context.Posts.Include(p => p.Comments).ToList();
        }

        public PostViewModel GetPostById(int postId)
        {
            var postViewModel = _context.Posts
                .Include(p => p.user)
                .Include(p => p.Comments.Select(c => c.commentAttachments))  
                .Include(p => p.Attachments)
                .Include(p => p.Likes)
                .AsEnumerable()
                .Select(p => new PostViewModel
                {
                    PostId = p.PostId,
                    Title = p.Title,
                    Content = p.Content,
                    CreatedAt = p.CreatedAt,
                    UserId = p.UserId,
                    UserName = p.user.FirstName + p.user.LastName,
                    Attachments = p.Attachments,
                    Likes = p.Likes,
                    UserHasLiked = p.Likes.Any(l => l.UserId == p.UserId && l.PostId == p.PostId),

                    // Map comments for the post along with CommentAttachments
                    Comments = p.Comments.Select(c => new CommentViewModel
                    {
                        CommentId = c.CommentId,
                        CommentText = c.Content,
                        CreatedAt = c.CreatedAt,
                        UserName = p.user.FirstName + p.user.LastName,

                        // Map CommentAttachments for each Comment
                        commentAttachments = c.commentAttachments.Select(ca => new CommentAttachment
                        {
                            CommentAttachmentId = ca.CommentAttachmentId,
                            FileName = ca.FileName,
                            FilePath = ca.FilePath
                        }).ToList()

                    }).ToList()

                })
                .FirstOrDefault(p => p.PostId == postId);

            return postViewModel;
        }

        public void AddPost(Post post)
        {
            _context.Posts.Add(post);
            _context.SaveChanges();
        }

        public List<PostViewModel> GetAllPosts()
        {

            var postViewModels = _context.Posts
            .Include(p => p.user)
            .Select(p => new PostViewModel
            {
                PostId = p.PostId,
                Title = p.Title,
                Content = p.Content,
                CreatedAt = p.CreatedAt,
                UserId = p.UserId,
                UserName = p.user.FirstName + p.user.LastName,

            })
                .ToList();

            return postViewModels;

        }

        public void AddLike(int userId, int postId)
        {
            var like = new Like
            {
                UserId = userId,
                PostId = postId
            };

            _context.Likes.Add(like);
            _context.SaveChanges();
        }

        public bool HasUserLikedPost(int userId, int postId)
        {
            var hasLiked = _context.Likes.Any(l => l.PostId == postId && l.UserId == userId);
            return hasLiked;
        }

        public void UnlikePost(int userId, int postId)
        {
            var likeToRemove = _context.Likes.FirstOrDefault(l => l.PostId == postId && l.UserId == userId);

            if (likeToRemove != null)
            {
                _context.Likes.Remove(likeToRemove);
                _context.SaveChanges();
            }
        }

        public void AddComment(int userId,int postId ,string commentText)
        {
            var newComment = new Comment
            {
                UserId = userId,
                PostId = postId,
                Content = commentText,
                CreatedAt = DateTime.Now 
            };

            _context.Comments.Add(newComment);

            _context.SaveChanges();
        }

        public void AddCommentWithAttachment(int userId, int postId, string commentText, string attachmentUrl, string FileName)
        {
            var newComment = new Comment
            {
                UserId = userId,
                PostId = postId,
                Content = commentText,
                CreatedAt = DateTime.Now,
            };

            _context.Comments.Add(newComment);

            int commentId = newComment.CommentId;

            var newCommentAttachment = new CommentAttachment
            {
                FilePath = attachmentUrl,
                FileName = FileName,
                CommentId = commentId,
            };
            _context.CommentsAttachment.Add(newCommentAttachment);

            _context.SaveChanges();
        }


    }



}
