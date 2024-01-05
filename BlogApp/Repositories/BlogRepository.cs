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
            var user = (User)HttpContext.Current.Session["AuthenticatedUser"];

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
                    UserHasLiked = p.Likes.Any(l => l.UserId == user.UserId && l.PostId == p.PostId),
                    UserIdOfUserAccessingPost = user.UserId,
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
            .Where(p => p.PostStatus == Status.Live)
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

        public List<PostViewModel> GetAllPendingPosts()
        {

            var postViewModels = _context.Posts
            .Include(p => p.user)
            .Where(p => p.PostStatus == Status.PendingApproval)
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

        public List<PostViewModel> GetAllRepoetedPost()
        {
            var postViewModels = _context.Posts
            .Include(p => p.user)
            .Where(p => p.PostStatus == Status.Reported)
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
                PostId = postId,
                CreatedAt = DateTime.Now,
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
                CreatedAt = DateTime.Now,
                _CommentStatus = CommentStatus.Live
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
                _CommentStatus = CommentStatus.Live
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

        //Get New Post For DashBoard
        public List<PostViewModel> GetMostRecentPosts(int count)
        {
            var posts = _context.Posts
                .Include(p => p.user)
                .OrderByDescending(p => p.CreatedAt)
                .Take(count)
                .Select(p => new PostViewModel
                {
                    //Map Properties with Post View Model Entities
                    PostId = p.PostId,
                    Title = p.Title,
                    Content = p.Content,
                    CreatedAt = p.CreatedAt,
                    UserName = p.user.FirstName + " " + p.user.LastName,
                })
                .ToList();

            return posts;
        }

        //Get moser Recent Comments
        public List<CommentViewModel> GetMostRecentComments(int count)
        {
            var comments = _context.Comments
            .OrderByDescending(c => c.CreatedAt)
            .Take(count)
            //.Include(c => c.PostId)
            .Select(c => new CommentViewModel
            {
                CommentId = c.CommentId,
                CommentText = c.Content,
                CreatedAt = c.CreatedAt,
                PostId = c.PostId,
                UserId = c.UserId,
                UserName = _context.Users 
                    .Where(u => u.UserId == c.UserId)
                    .Select(u => u.FirstName)
                    .FirstOrDefault(),
                PostTitle = _context.Posts.Where(p => p.PostId  == c.PostId).Select(p => p.Title).FirstOrDefault(),
            })
            .ToList();
            return comments;
        }

        public List<LikeViewModel> GetMostRecentLikes(int count)
        {
            var likes = _context.Likes
                .OrderByDescending(l => l.CreatedAt)
                .Take(count)
                .Select(l => new LikeViewModel
                {

                    UserName = _context.Users.Where(u => u.UserId == l.UserId)
                    .Select(u => u.FirstName)
                    .FirstOrDefault(),

                    PostTitle = _context.Posts.Where(p => p.PostId == l.PostId).Select(p => p.Title).FirstOrDefault(),
                    CreatedAt = l.CreatedAt,
                    PostId = l.PostId
                })
                .ToList();

            return likes;
        }

        public void ApprovePost(int postId)
        {
            var post = _context.Posts.Find(postId);

            if (post != null)
            {
                post.PostStatus = Status.Live;

                // Save changes to the database
                _context.SaveChanges();
            }
        }

        public void RejectPost(int postId)
        {
            var post = _context.Posts.Find(postId);

            if (post != null)
            {
                post.PostStatus = Status.Rejected;
                _context.SaveChanges();
            }
        }

        public void ReportPost( int postId, int userIdWhoReport, string reportReason)
        {
            var post = _context.Posts.Find(postId);

            if (post != null)
            {
                // Create a new ReportPost instance
                var reportPost = new ReportPost
                {
                    PostId = postId,
                    UserId = userIdWhoReport,
                    ReportReason = reportReason
                };

                // Add the new report to the context and save changes
                _context.ReportPosts.Add(reportPost);
                _context.SaveChanges();
            }

        }


        public List<ReportPostViewModel> GetReportedDetails()
        {

            var reportedPosts = _context.ReportPosts
            .ToList();


            //Make an List of ReportPostViewModel
            var reportPostViewModels = reportedPosts.Select(rp => new ReportPostViewModel
            {
                Post = GetPostById(rp.PostId),
                PostId = rp.PostId,
                UserId = rp.UserId,
                Reason = rp.ReportReason,
                RepoeterName = _context.Users.Where(u => u.UserId == rp.UserId).Select(u => u.FirstName).FirstOrDefault()
            }).ToList();

            return reportPostViewModels;
        }

        public void DeleteReportedPost(int postId)
        {
            var reportedPosts = _context.ReportPosts.Where(rp => rp.PostId == postId).ToList();

            foreach (var reportedPost in reportedPosts)
            {
                // Remove each reported post entry
                _context.ReportPosts.Remove(reportedPost);
            }

            // Save changes to the database
            _context.SaveChanges();
        }

        public void AcceptReportToPost(int postId)
        {
            // Implement logic to update the post status or take necessary actions
            var post = _context.Posts.Find(postId);

            if (post != null)
            {
                post.PostStatus = Status.DeletedAfterReport; 

                _context.SaveChanges();
                DeleteReportedPost(postId);
            }
            else
            {
                throw new InvalidOperationException("Post not found.");
            }
        }

        public void RejectReportToPost(int postId)
        {
            // Implement logic to update the post status or take necessary actions
            var post = _context.Posts.Find(postId);

            if (post != null)
            {
                post.PostStatus = Status.Live;
                
                _context.SaveChanges();
                DeleteReportedPost(postId);

            }
            else
            {
                throw new InvalidOperationException("Post not found.");
            }
        }



    }



}
